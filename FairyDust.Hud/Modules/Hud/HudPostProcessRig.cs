using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Modules.Hud;

/// <summary>Renders the HUD to a texture through a camera, then displays it as overlay UI.</summary>
internal sealed class HudPostProcessRig
{
    private const string CameraObjectName = "FairyDust_HudPostProcessCamera";
    private const string DisplayObjectName = "FairyDust_HudPostProcessDisplay";

    private readonly Transform parent;

    private Camera hudCamera;
    private RenderTexture hudTexture;
    private RawImage displayImage;
    private int textureWidth;
    private int textureHeight;
    private bool effectsEnabled;
    private bool contentVisible = true;
    private bool loggedUpdateFailure;
    private bool loggedModeFailure;
    private bool loggedInvalidBinding;
    private bool loggedTextureFailure;
    private bool loggedDisplayFailure;

    public HudPostProcessRig(Transform parent)
    {
        this.parent = parent;
    }

    public Camera Camera => hudCamera;

    public string DebugSummary
    {
        get
        {
            string textureState = hudTexture == null
                ? "rt=null"
                : $"rt={textureWidth}x{textureHeight},created={hudTexture.IsCreated()}";
            string cameraState = hudCamera == null
                ? "camera=null"
                : $"camera=ok,target={(hudCamera.targetTexture == null ? "screen" : "rt")},depth={hudCamera.depth:0.##},clear={hudCamera.clearFlags}";
            string displayState = displayImage == null
                ? "display=null"
                : $"display=ok,active={displayImage.gameObject.activeSelf},texture={(displayImage.texture == null ? "null" : "set")}";

            return $"effects={effectsEnabled}, postLayer=disabled, {cameraState}, {textureState}, {displayState}";
        }
    }

    public void Build(RectTransform displayCanvas)
    {
        BuildCamera();
        BuildDisplay(displayCanvas);
        ApplyRenderMode();
    }

    public void Update()
    {
        try
        {
            if (effectsEnabled)
            {
                EnsureRenderTexture();
                ValidateRenderBinding();
            }
        }
        catch (Exception ex)
        {
            if (!loggedUpdateFailure)
            {
                loggedUpdateFailure = true;
                MelonLoader.MelonLogger.Warning("[FairyDust.Hud] HUD RLPro render texture update failed: " + ex);
            }
        }
    }

    public void SetEffectsEnabled(bool enabled)
    {
        string phase = "set flag";
        try
        {
            effectsEnabled = enabled;
            phase = "apply render mode";
            ApplyRenderMode();

            phase = "post-process layer skipped";
        }
        catch (Exception ex)
        {
            if (!loggedModeFailure)
            {
                loggedModeFailure = true;
                MelonLoader.MelonLogger.Warning("[FairyDust.Hud] HUD RLPro mode switch failed during '" + phase + "': " + ex);
            }
        }
    }

    public void SetContentVisible(bool visible)
    {
        try
        {
            contentVisible = visible;
            ApplyDisplayVisibility();
        }
        catch (Exception ex)
        {
            if (!loggedModeFailure)
            {
                loggedModeFailure = true;
                MelonLoader.MelonLogger.Warning("[FairyDust.Hud] HUD render display visibility failed: " + ex);
            }
        }
    }

    public void Dispose()
    {
        SetEffectsEnabled(false);

        if (hudCamera != null)
        {
            hudCamera.targetTexture = null;
        }

        if (displayImage != null)
        {
            displayImage.texture = null;
        }

        if (hudTexture != null)
        {
            hudTexture.Release();
            UnityEngine.Object.Destroy(hudTexture);
            hudTexture = null;
        }

        displayImage = null;
        hudCamera = null;
    }

    private void BuildCamera()
    {
        var cameraGo = new GameObject(CameraObjectName);
        cameraGo.transform.SetParent(parent, false);
        cameraGo.layer = HudDockLayout.HudRenderLayer;

        hudCamera = cameraGo.AddComponent<Camera>();
        hudCamera.clearFlags = CameraClearFlags.Depth;
        hudCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        hudCamera.cullingMask = 1 << HudDockLayout.HudRenderLayer;
        hudCamera.orthographic = true;
        hudCamera.orthographicSize = 540f;
        hudCamera.nearClipPlane = -10f;
        hudCamera.farClipPlane = 10f;
        hudCamera.depth = HudDockLayout.HudCameraDepth;
        hudCamera.allowHDR = false;
        hudCamera.allowMSAA = false;
    }

    private void BuildDisplay(RectTransform displayCanvas)
    {
        var displayGo = new GameObject(DisplayObjectName);
        displayGo.transform.SetParent(displayCanvas, false);
        displayGo.layer = 5;

        var displayRt = displayGo.AddComponent<RectTransform>();
        displayRt.anchorMin = Vector2.zero;
        displayRt.anchorMax = Vector2.one;
        displayRt.offsetMin = Vector2.zero;
        displayRt.offsetMax = Vector2.zero;

        displayImage = displayGo.AddComponent<RawImage>();
        displayImage.color = Color.white;
        displayImage.raycastTarget = false;
        displayGo.SetActive(false);
    }

    private void ApplyRenderMode()
    {
        string phase = "start";
        try
        {
            if (effectsEnabled)
            {
                phase = "ensure render texture";
                EnsureRenderTexture();
            }
            else if (hudCamera != null)
            {
                phase = "clear camera target";
                hudCamera.targetTexture = null;
                hudCamera.orthographicSize = Mathf.Max(64, Screen.height) * 0.5f;
                if (displayImage != null)
                {
                    phase = "clear display texture";
                    displayImage.texture = null;
                }
            }

            if (displayImage != null)
            {
                phase = "set display active";
                ApplyDisplayVisibility();
            }
        }
        catch (Exception ex)
        {
            if (!loggedModeFailure)
            {
                loggedModeFailure = true;
                MelonLoader.MelonLogger.Warning("[FairyDust.Hud] HUD RLPro render mode failed during '" + phase + "': " + ex);
            }
        }
    }

    private void ApplyDisplayVisibility()
    {
        if (displayImage != null)
        {
            displayImage.gameObject.SetActive(effectsEnabled && contentVisible);
        }
    }

    private void EnsureRenderTexture()
    {
        string phase = "read screen size";
        try
        {
            int width = Mathf.Max(64, Screen.width);
            int height = Mathf.Max(64, Screen.height);
            if (hudTexture != null && textureWidth == width && textureHeight == height)
            {
                phase = "bind existing texture";
                BindRenderTexture(width, height);
                return;
            }

            if (hudTexture != null)
            {
                phase = "release old texture";
                hudTexture.Release();
                UnityEngine.Object.Destroy(hudTexture);
            }

            phase = "create texture";
            textureWidth = width;
            textureHeight = height;
            hudTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            hudTexture.name = "FairyDust_HudPostProcessTexture";
            hudTexture.filterMode = FilterMode.Bilinear;
            hudTexture.wrapMode = TextureWrapMode.Clamp;
            hudTexture.useMipMap = false;
            hudTexture.autoGenerateMips = false;
            hudTexture.Create();

            phase = "bind new texture";
            BindRenderTexture(width, height);
        }
        catch (Exception ex)
        {
            if (!loggedTextureFailure)
            {
                loggedTextureFailure = true;
                MelonLoader.MelonLogger.Warning("[FairyDust.Hud] HUD RLPro render texture failed during '" + phase + "': " + ex);
            }
        }
    }

    private void BindRenderTexture(int width, int height)
    {
        string phase = "bind camera";
        try
        {
            if (hudCamera != null)
            {
                hudCamera.targetTexture = hudTexture;
                hudCamera.aspect = width / (float)height;
                hudCamera.orthographicSize = height * 0.5f;
            }

            phase = "bind display";
            if (displayImage != null)
            {
                displayImage.texture = hudTexture;
            }
        }
        catch (Exception ex)
        {
            if (!loggedDisplayFailure)
            {
                loggedDisplayFailure = true;
                MelonLoader.MelonLogger.Warning("[FairyDust.Hud] HUD RLPro render texture binding failed during '" + phase + "': " + ex);
            }
        }
    }

    private void ValidateRenderBinding()
    {
        if (loggedInvalidBinding)
        {
            return;
        }

        try
        {
            if (effectsEnabled && hudCamera != null && hudTexture != null && hudCamera.targetTexture != hudTexture)
            {
                loggedInvalidBinding = true;
                MelonLoader.MelonLogger.Warning("[FairyDust.Hud] HUD RLPro invalid binding: camera target is not the HUD render texture. " + DebugSummary);
            }
        }
        catch (Exception ex)
        {
            if (!loggedInvalidBinding)
            {
                loggedInvalidBinding = true;
                MelonLoader.MelonLogger.Warning("[FairyDust.Hud] HUD RLPro binding validation failed: " + ex);
            }
        }
    }

}

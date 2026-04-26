using FairyDust.Hud.Configuration;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace FairyDust.Hud.Modules.Hud;

/// <summary>Screen Space Overlay shell: bottom-left dock, module slots.</summary>
internal sealed class GameplayHudHost
{
    private const string RigObjectName = "FairyDust_HudRig";
    private const string CanvasObjectName = "FairyDust_GameHud";
    private const string DockObjectName = "FairyDust_HudDock";

    private readonly MelonMod hostMod;
    private readonly List<IHudModule> modules = new();

    private GameObject hudRig;
    private GameObject canvasRoot;
    private Canvas hudCanvas;
    private CanvasScaler hudCanvasScaler;
    private RectTransform dockRoot;

    public GameplayHudHost(MelonMod hostMod)
    {
        this.hostMod = hostMod;
    }

    public void Register(IHudModule module) => modules.Add(module);

    public void Initialize()
    {
        BuildCanvasAndDock();

        for (int i = 0; i < modules.Count; i++)
        {
            IHudModule module = modules[i];
            RectTransform slot = CreateModuleSlot(dockRoot, module.SlotObjectName);
            var ctx = new HudModuleContext(slot, hostMod);
            module.OnAttach(ctx);
        }
    }

    public void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        WorldSceneGate.OnSceneWasLoaded(buildIndex, sceneName);
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].OnSceneWasLoaded(buildIndex, sceneName);
        }
    }

    public void OnLateUpdate()
    {
        if (!Config.Values.Enabled)
        {
            SetCanvasActive(false);
            return;
        }

        if (!WorldSceneGate.IsActiveGameplayScene)
        {
            SetCanvasActive(false);
            return;
        }

        SetCanvasActive(true);
        UpdateDockLayout();

        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].OnLateUpdate();
        }
    }

    private void SetCanvasActive(bool on)
    {
        if (hudRig != null)
        {
            hudRig.SetActive(on);
        }
    }

    private void UpdateDockLayout()
    {
        if (dockRoot == null)
        {
            return;
        }

        dockRoot.anchoredPosition = new Vector2(HudDockLayout.MarginLeft, HudDockLayout.MarginBottom);

        var vlg = dockRoot.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            vlg.spacing = HudDockLayout.ModuleStackSpacing;
        }

        LayoutRebuilder.MarkLayoutForRebuild(dockRoot);
    }

    private void BuildCanvasAndDock()
    {
        hudRig = new GameObject(RigObjectName);
        UnityEngine.Object.DontDestroyOnLoad(hudRig);

        canvasRoot = new GameObject(CanvasObjectName);
        canvasRoot.transform.SetParent(hudRig.transform, false);

        var canvas = canvasRoot.AddComponent<Canvas>();
        hudCanvas = canvas;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = null;
        canvas.sortingOrder = 100;
        canvas.pixelPerfect = false;

        var rootRt = canvasRoot.GetComponent<RectTransform>();
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.pivot = new Vector2(0.5f, 0.5f);
        rootRt.offsetMin = Vector2.zero;
        rootRt.offsetMax = Vector2.zero;

        var scaler = canvasRoot.AddComponent<CanvasScaler>();
        hudCanvasScaler = scaler;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        var ray = canvasRoot.AddComponent<GraphicRaycaster>();
        ray.blockingObjects = GraphicRaycaster.BlockingObjects.None;

        var dockGo = new GameObject(DockObjectName);
        dockGo.transform.SetParent(canvasRoot.transform, false);
        dockRoot = dockGo.AddComponent<RectTransform>();
        dockRoot.anchorMin = new Vector2(0f, 0f);
        dockRoot.anchorMax = new Vector2(0f, 0f);
        dockRoot.pivot = new Vector2(0f, 0f);
        dockRoot.anchoredPosition = new Vector2(HudDockLayout.MarginLeft, HudDockLayout.MarginBottom);

        var fitter = dockGo.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var vlg = dockGo.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.LowerLeft;
        vlg.childControlWidth = false;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;
        vlg.spacing = HudDockLayout.ModuleStackSpacing;
        vlg.padding = new RectOffset(0, 0, 0, 0);
        vlg.reverseArrangement = false;
    }

    private static RectTransform CreateModuleSlot(RectTransform dock, string objectName)
    {
        var slotGo = new GameObject(objectName);
        slotGo.transform.SetParent(dock, false);
        var rt = slotGo.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot = new Vector2(0f, 0f);

        slotGo.AddComponent<LayoutElement>();
        var cg = slotGo.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        return rt;
    }
}

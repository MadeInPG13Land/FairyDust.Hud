using UnityEngine;

namespace FairyDust.Hud.Modules.Status.Entities;

internal readonly struct StatusRowSnapshot
{
    public StatusRowSnapshot(
        StatusRowKind kind,
        string id,
        string label,
        float ratio,
        int sortOrder,
        string valueText = null,
        bool warning = false)
    {
        Kind = kind;
        Id = id;
        Label = label;
        Ratio = Mathf.Clamp01(ratio);
        SortOrder = sortOrder;
        ValueText = valueText;
        Warning = warning;
    }

    public StatusRowKind Kind { get; }

    public string Id { get; }

    public string Label { get; }

    public float Ratio { get; }

    public int SortOrder { get; }

    public string ValueText { get; }

    public bool Warning { get; }
}

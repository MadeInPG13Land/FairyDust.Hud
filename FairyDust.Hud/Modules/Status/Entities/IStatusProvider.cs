namespace FairyDust.Hud.Modules.Status.Entities;

internal interface IStatusProvider
{
    string Id { get; }

    int SortOrder { get; }

    bool IsEnabled { get; }

    bool TryGetRow(StatusProviderContext context, out StatusRowSnapshot row);
}

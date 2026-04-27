using FairyDust.Hud.Modules.Status.Entities;

namespace FairyDust.Hud.Modules.Status.Services;

internal sealed class StatusBoardService
{
    private readonly IStatusProvider[] providers;

    public StatusBoardService(IEnumerable<IStatusProvider> providers)
    {
        this.providers = providers.OrderBy(provider => provider.SortOrder).ToArray();
    }

    public IReadOnlyList<IStatusProvider> Providers => providers;

    public int Capacity => providers.Length;

    public void CollectRows(StatusProviderContext context, List<StatusRowSnapshot> rows)
    {
        rows.Clear();
        for (int i = 0; i < providers.Length; i++)
        {
            IStatusProvider provider = providers[i];
            if (!provider.IsEnabled)
            {
                continue;
            }

            if (provider.TryGetRow(context, out StatusRowSnapshot row))
            {
                rows.Add(row);
            }
        }

        rows.Sort((left, right) => left.SortOrder.CompareTo(right.SortOrder));
    }
}

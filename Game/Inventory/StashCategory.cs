namespace Game.Inventory;

/// <summary>
/// The 4 starter Stash tabs (Etapa 13 — MASTER_HANDOFF seção 25-27:
/// "Stash cedo no dev: General/Equipment/Currency/Unique, depois
/// busca/ordenação/tabs"). See <see cref="Stash"/> for how an item is
/// assigned to one of these automatically.
/// </summary>
public enum StashCategory
{
    General,
    Equipment,
    Currency,
    Unique,
}

namespace Game.Items;

/// <summary>
/// Raridades iniciais (MASTER_HANDOFF seção 18-24): Normal/Magic/Rare/Unique.
/// Valores explícitos porque isso eventualmente é serializado em save games
/// (Etapa 22) — um enum sem valores fixos pode mudar de número se a ordem
/// dos itens mudar no código, corrompendo saves antigos silenciosamente.
/// Deixar buracos (10, 20, 30...) para caber Relic/Ancient/Mythic/Corrupted
/// no futuro sem renumerar os já existentes (mesma exigência do
/// MASTER_HANDOFF: "arquitetura deve permitir isso sem reescrever").
/// </summary>
public enum ItemRarity
{
    Normal = 0,
    Magic = 10,
    Rare = 20,
    Unique = 30,
}

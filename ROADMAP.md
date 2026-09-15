# ROADMAP.md — Mystery Absolut

Ordem das etapas. Regra: uma etapa por vez, compilando e funcionando ao
final de cada uma, depois PARAR. Não pular etapas nem implementar sistemas
de etapas futuras antecipadamente.

- [x] **Etapa 0 — Fundação**: estrutura mínima de pastas, documentação
      permanente (este conjunto de arquivos), namespaces, cena inicial,
      bootstrap. *(implementado nesta branch; verificação local pendente —
      ver CURRENT_STATUS.md)*
- [x] **Etapa 1 — Player Movement**: `CharacterBody2D`, movimento 8
      direções com diagonal normalizada, colisão, câmera, mapa de teste
      simples. *(implementado nesta branch; verificação local pendente —
      ver CURRENT_STATUS.md)*
- [ ] **Etapa 2 — Combat Foundation**: `Combatant`, `Health`, `Damage`,
      `Hitbox`, `Hurtbox`, Basic Attack, Dummy de teste.
- [ ] **Etapa 3 — Dodge / Block**: dodge, i-frame, cooldown, block, base de
      parry.
- [ ] **Etapa 4 — Primeiro inimigo**: IA Idle/Chase/Attack/Dead.
- [ ] **Etapa 5 — Loot básico**: `DropTable`, `LootGenerator`,
      `GroundItem`, pickup.
- [ ] **Etapa 6 — Item System**: `ItemBaseDefinition`, `ItemInstance`,
      Rarity, ItemLevel, Tags.
- [ ] **Etapa 7 — Inventory**: stack, pickup, remove, UI básica.
- [ ] **Etapa 8 — Equipment**: todos os slots (Weapon, Offhand, Helmet,
      Chest, Gloves, Boots, Amulet, Ring 1, Ring 2).
- [ ] **Etapa 9 — Affixes**: Prefix/Suffix, tiers, pools, weighted rolls.
- [ ] **Etapa 10 — Crafting**: primeiros `CraftingEffect`s (equivalentes a
      Transmutation...Chaos), nomes temporários somente em dados.
- [ ] **Etapa 11 — Loot Presentation**: labels, rarity visual, glow, beam,
      sons, partículas básicas.
- [ ] **Etapa 12 — Loot Filter**: rules, conditions, actions, show/hide,
      font, glow, beam, sound.
- [ ] **Etapa 13 — Stash**: General/Equipment/Currency/Unique.
- [ ] **Etapa 14 — Primeiro Companion**: follow, HP, stats, equipamento,
      combate.
- [ ] **Etapa 15 — Companion AI**: Follow/Attack/Retreat/Regroup.
- [ ] **Etapa 16 — Dialogue**: sistema data-driven.
- [ ] **Etapa 17 — Choices**: Choice/Condition/Action/Consequences,
      WorldState.
- [ ] **Etapa 18 — Relationships**: Affinity, RelationshipState, Memories.
- [ ] **Etapa 19 — Quests**: QuestDefinition/QuestState/Objectives/Rewards.
- [ ] **Etapa 20 — Primeira Dungeon**.
- [ ] **Etapa 21 — Primeiro Boss**: telegraphs, ataques, fases, loot.
- [ ] **Etapa 22 — Save / Load**.
- [ ] **Etapa 23 — Loot Filter Editor**.
- [ ] **Etapa 24 — Vertical Slice**: vila pequena → NPCs → companheiro →
      decisão narrativa → área externa → combate → loot → equipar → stash
      → craft simples → dungeon → companheiro reage à decisão → boss →
      loot especial → retorno à vila → consequência da escolha. Deve
      provar que Combate + Itemização + Personagens + Escolhas funcionam
      juntos.

Contexto completo de cada pilar/sistema está em MASTER_CONTEXT.md e no
MASTER_HANDOFF salvo no projeto Claude vinculado a este repositório.

# ROADMAP.md — Mystery Absolut

Ordem das etapas. Regra: uma etapa por vez, compilando e funcionando ao
final de cada uma, depois PARAR. Não pular etapas nem implementar sistemas
de etapas futuras antecipadamente.

- [x] **Etapa 0 — Fundação**: estrutura mínima de pastas, documentação
      permanente (este conjunto de arquivos), namespaces, cena inicial,
      bootstrap. **Verificada localmente pelo usuário (vídeo conferido) —
      concluída de verdade.**
- [x] **Etapa 1 — Player Movement**: `CharacterBody2D`, movimento 8
      direções com diagonal normalizada, colisão, câmera, mapa de teste
      simples. **Verificada localmente pelo usuário (vídeo conferido) —
      concluída de verdade.**
- [x] **Etapa 2 — Combat Foundation**: `Combatant`, `Health`, `DamageInfo`,
      `Hitbox`, `Hurtbox`, `PlayerBasicAttack`, `Dummy` de teste.
      **Verificada localmente pelo usuário (vídeo conferido).**
- [x] **Etapa 3 — Dodge / Block**: `PlayerDodge` (dash + i-frame +
      cooldown), `PlayerBlock` (redução de dano + base de parry via
      `Hurtbox.Parried`). **Verificada localmente pelo usuário (vídeo
      conferido).**
- [x] **Etapa 4 — Primeiro inimigo**: IA Idle/Chase/Attack/Recover/Dead
      (`EnemyController`), reaproveitando Health/Combatant/Hitbox/Hurtbox
      do player sem alterá-los. **Verificada localmente pelo usuário
      (vídeo conferido).**
- [x] **Etapa 5 — Loot básico**: `DropTable`, `LootGenerator`,
      `GroundItem`, pickup automático ao encostar. **Verificada localmente
      pelo usuário (vídeo conferido).**
- [x] **Etapa 6 — Item System**: `ItemBaseDefinition`, `ItemInstance`,
      `ItemRarity`, `ItemLevel`, `Tags`, catálogo em
      `Data/Items/items.json`. **Verificada localmente pelo usuário
      (vídeo conferido — compilou, JSON carregou, itens corretos).**
- [x] **Etapa 7 — Inventory**: `Inventory` (stack/add/remove), integração
      com loot (GroundItem/LootGenerator agora usam ItemInstance real),
      `InventoryUI` básica (tecla I). **Verificada localmente pelo
      usuário (vídeo conferido — painel abre, mostra raridade/stack
      corretamente).**
- [x] **Etapa 8 — Equipment**: todos os slots (Weapon, Offhand, Helmet,
      Chest, Gloves, Boots, Amulet, Ring1, Ring2) via `Equipment`
      (Game/Inventory), equipar/desequipar pela `InventoryUI`.
      **Verificada localmente pelo usuário (vídeo conferido — arma
      equipada no slot Weapon, com raridade e afixo corretos).**
- [x] **Etapa 9 — Affixes**: `AffixDefinition`/`AffixInstance`/
      `AffixDatabase`/`AffixRoller` — Prefix/Suffix, tiers, pools por tag,
      pesos, catálogo em `Data/Items/affixes.json`. **Verificada
      localmente pelo usuário (vídeo conferido — afixo "+1.1 Dano
      Físico" exibido corretamente num item [Magic]).**
- [x] **Etapa 10 — Crafting**: `MakeMagicEffect`/`RerollMagicModifiersEffect`/
      `AddModifierEffect`/`UpgradeToRareEffect` (Game/Crafting), ligados a
      4 itens de currency via `CraftingEffectId` (dado, não hardcode).
      *(compilou e as 4 currencies aparecem corretamente no inventário no
      vídeo da Etapa 8+9+10, mas nenhum "[usar]" foi clicado nele —
      aplicar um efeito de crafting de verdade segue sem confirmação
      visual, ver CURRENT_STATUS.md)*
- [x] **Etapa 11 — Loot Presentation**: label, cor por raridade, contorno,
      glow pulsante (Tween) e beam para Rare/Unique, partículas
      (`CpuParticles2D`) para Unique — tudo em `GroundItem`/
      `LootPresentation`. Som fica de fora (sem assets de áudio ainda,
      ver ARCHITECTURE.md). *(implementado nesta branch; verificação
      local ainda pendente — ver CURRENT_STATUS.md/HANDOFF.md)*
- [x] **Etapa 12 — Loot Filter**: `LootFilterRule`/`LootFilterDatabase`
      avaliando `Data/Loot/loot_filter.json` (condições: tags, raridade,
      item level, currency; ações: show/hide, cor, tamanho de fonte,
      contorno/glow/beam/partículas) — primeira regra que bate vence,
      sem regra = defaults da Etapa 11. *(implementado nesta branch;
      verificação local ainda pendente — ver CURRENT_STATUS.md/HANDOFF.md)*
- [x] **Etapa 13 — Stash**: `Stash`/`StashCategory` (General/Equipment/
      Currency/Unique), guardar/retirar pela `StashUI` (tecla T,
      independente do Inventário). *(implementado nesta branch;
      verificação local ainda pendente — ver CURRENT_STATUS.md/HANDOFF.md)*
- [x] **Etapa 14 — Primeiro Companion**: follow, HP, stats, equipamento,
      combate. *(implementado nesta branch; verificação local ainda pendente)*
- [x] **Etapa 15 — Companion AI**: Follow/Attack/Retreat/Regroup.
- [x] **Etapa 16 — Dialogue**: sistema data-driven.
- [x] **Etapa 17 — Choices**: Choice/Condition/Action/Consequences,
      WorldState.
- [x] **Etapa 18 — Relationships**: Affinity, RelationshipState, Memories.
- [x] **Etapa 19 — Quests**: QuestDefinition/QuestState/Objectives/Rewards.
- [x] **Etapa 20 — Primeira Dungeon**.
- [x] **Etapa 21 — Primeiro Boss**: telegraphs, ataques, fases, loot.
- [x] **Etapa 22 — Save / Load**.
- [x] **Etapa 23 — Loot Filter Editor**.
- [x] **Etapa 24 — Vertical Slice**: vila pequena → NPCs → companheiro →
      decisão narrativa → área externa → combate → loot → equipar → stash
      → craft simples → dungeon → companheiro reage à decisão → boss →
      loot especial → retorno à vila → consequência da escolha. Deve
      provar que Combate + Itemização + Personagens + Escolhas funcionam
      juntos.

### Fase 5: Expansão de RPG e Sistemas Avançados (As Etapas Além)
- [ ] **Etapa 25 — Status do Jogador e Nivelamento**: XP, Level, Força/Destreza/Inteligência e HP/Mana Escaláveis.
- [ ] **Etapa 26 — Habilidades Ativas (Skills)**: Adição de Magias e Habilidades com cooldowns e custo de Mana.
- [ ] **Etapa 27 — Árvore de Talentos (Passivas)**: Interface para gastar pontos ganhos ao subir de nível, conectando nós.
- [ ] **Etapa 28 — Mercadores (Shop)**: NPCs na Vila que vendem itens e compram seus itens em troca de Ouro.
- [ ] **Etapa 29 — Geração Procedural de Dungeons**: Dungeons que mudam de layout a cada visita (Salas Aleatórias).
- [ ] **Etapa 30 — Áudio (SFX e Música)**: Integração do sistema de AudioStreamPlayer do Godot para ataques, loot e música ambiente.

Contexto completo de cada pilar/sistema está em MASTER_CONTEXT.md e no
MASTER_HANDOFF salvo no projeto Claude vinculado a este repositório.

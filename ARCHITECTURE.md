# ARCHITECTURE.md — Mystery Absolut

Como os sistemas implementados até agora funcionam. Atualizar este arquivo
sempre que uma etapa mudar ou adicionar arquitetura (não apenas conteúdo).

## Estrutura de pastas atual

```
res://
├── Mystery Absolut.csproj      # projeto C#/.NET (Godot.NET.Sdk)
├── project.godot
├── Scenes/
│   └── Main.tscn                # cena principal (run/main_scene)
├── Game/
│   ├── Core/
│   │   └── InputBootstrap.cs    # autoload: registra InputMap em código
│   ├── Actors/
│   │   ├── Player/
│   │   │   ├── Player.tscn
│   │   │   ├── PlayerController.cs
│   │   │   ├── PlayerBasicAttack.cs
│   │   │   ├── PlayerDodge.cs
│   │   │   └── PlayerBlock.cs
│   │   └── Enemies/
│   │       ├── Enemy.tscn
│   │       └── EnemyController.cs
│   ├── Combat/
│   │   ├── DamageInfo.cs
│   │   ├── Health.cs
│   │   ├── Combatant.cs
│   │   ├── Hurtbox.cs
│   │   ├── Hitbox.cs
│   │   ├── Cooldown.cs
│   │   ├── Dummy.tscn
│   │   └── Dummy.cs
│   ├── Loot/
│   │   ├── DropTable.cs
│   │   ├── LootGenerator.cs
│   │   ├── GroundItem.cs
│   │   ├── GroundItem.tscn
│   │   ├── LootPresentation.cs       # Etapa 11
│   │   ├── LootFilterRule.cs         # Etapa 12
│   │   ├── LootFilterColor.cs        # Etapa 12
│   │   ├── LootFilterResult.cs       # Etapa 12
│   │   └── LootFilterDatabase.cs     # Etapa 12
│   ├── Items/
│   │   ├── ItemRarity.cs
│   │   ├── ItemBaseDefinition.cs
│   │   ├── ItemInstance.cs
│   │   ├── ItemDatabase.cs
│   │   ├── EquipmentSlot.cs          # Etapa 8
│   │   ├── EquipmentSlotCategory.cs  # Etapa 8
│   │   ├── AffixType.cs              # Etapa 9
│   │   ├── AffixDefinition.cs        # Etapa 9
│   │   ├── AffixInstance.cs          # Etapa 9
│   │   ├── AffixDatabase.cs          # Etapa 9
│   │   └── AffixRoller.cs            # Etapa 9
│   ├── Inventory/
│   │   ├── Inventory.cs
│   │   ├── Equipment.cs              # Etapa 8
│   │   ├── Stash.cs                  # Etapa 13
│   │   └── StashCategory.cs          # Etapa 13
│   ├── Crafting/                     # Etapa 10
│   │   ├── ICraftingEffect.cs
│   │   ├── MakeMagicEffect.cs
│   │   ├── RerollMagicModifiersEffect.cs
│   │   ├── AddModifierEffect.cs
│   │   ├── UpgradeToRareEffect.cs
│   │   ├── CraftingEffectRegistry.cs
│   │   └── CraftingService.cs
│   └── World/
│       └── TestWorld.tscn       # sala de teste: chão, paredes, obstáculos, dummy, inimigo
├── UI/
│   ├── InventoryUI.cs            # Etapa 7 (+ Etapa 8/10): inventário, equipar, craftar
│   ├── InventoryUI.tscn
│   ├── StashUI.cs                # Etapa 13: guardar/retirar do baú
│   └── StashUI.tscn
└── Data/
    ├── Items/
    │   ├── items.json            # Etapa 6 (+ 8/10: equipSlotCategory, craftingEffectId)
    │   └── affixes.json          # Etapa 9: catálogo de AffixDefinition
    └── Loot/
        └── loot_filter.json      # Etapa 12: regras do Loot Filter
```

Pastas do ROADMAP futuro (Narrative/, Relationships/, Quests/, World/Save,
Audio/, Tests/, Tools/) ainda não existem — serão criadas apenas quando a
etapa correspondente precisar delas (ver seção 55/65 do MASTER_HANDOFF).

## Namespaces

- `Game.Core` — infraestrutura transversal (hoje: bootstrap de Input).
- `Game.Actors.Player` — controlador do jogador e seus componentes de
  combate (ataque, dodge, block).
- `Game.Actors.Enemies` — controlador(es) de inimigo e sua IA. Desde a
  Etapa 4: `EnemyController` (estado único, ver seção própria abaixo).
- `Game.Combat` — sistemas de combate genéricos, reutilizáveis por
  qualquer actor (player, dummy, inimigos, futuros bosses): Health,
  Combatant, Hitbox, Hurtbox, DamageInfo, Cooldown.
- `Game.Loot` — geração, pickup e apresentação de loot físico no mundo.
  Desde a Etapa 5: `DropTable`, `LootGenerator`, `GroundItem`. Desde a
  Etapa 11: `LootPresentation` (visual por raridade). Desde a Etapa 12:
  `LootFilterRule`/`LootFilterColor`/`LootFilterResult`/`LootFilterDatabase`
  (regras data-driven que decidem show/hide e estilo final).
- `Game.Items` — o modelo de dados de item (ItemBase → ItemInstance →
  Rarity), carregado de `Data/Items/items.json`. Desde a Etapa 6:
  `ItemRarity`, `ItemBaseDefinition`, `ItemInstance`, `ItemDatabase`.
- `Game.Inventory` — posse/armazenamento de itens. Desde a Etapa 7:
  `Inventory` (stack, add, remove). Desde a Etapa 8: `Equipment` (slots).
  Desde a Etapa 13: `Stash`/`StashCategory` (baú com 4 abas).
- `Game.Crafting` — desde a Etapa 10: `ICraftingEffect` e implementações,
  `CraftingEffectRegistry`, `CraftingService`.
- `Game.UI` — interface (HUD, painéis). Desde a Etapa 7: `InventoryUI`
  (estendida na Etapa 8/10 para equipar/desequipar/craftar). Desde a
  Etapa 13: `StashUI` (painel separado, tecla T).
  **Nota de inconsistência deliberada:** a pasta `UI/` é raiz do projeto
  (irmã de `Game/`, decidido assim já na Etapa 0), não `Game/UI/` — mas o
  namespace segue a regra geral `Game.<Área>` mesmo assim, para manter
  consistência lógica com o resto do código. Ou seja, aqui (só aqui)
  namespace e caminho de pasta não se correspondem 1:1; isso é intencional
  e não um erro de digitação.

Novos sistemas devem seguir o mesmo padrão: `Game.<Área>` (ex:
`Game.Crafting`, `Game.Narrative`). Evitar namespace global.

## Fluxo de cena (bootstrap)

```
Scenes/Main.tscn (run/main_scene)
    └── instância de Game/World/TestWorld.tscn
            └── instância de Game/Actors/Player/Player.tscn
```

`Main.tscn` hoje é apenas um `Node2D` que instancia `TestWorld` — não há
lógica de bootstrap própria ainda (sem gerenciador de estado, sem loading
screen). Isso é intencional: a Etapa 1 não precisa disso. Quando uma etapa
futura precisar de transição de cenas / gerenciamento de estado global, um
verdadeiro "Bootstrap manager" deve ser introduzido em `Game.Core` — não
antes.

## InputMap (Game/Core/InputBootstrap.cs)

Autoload singleton (registrado em `project.godot` → `[autoload]`) que
registra `move_up`, `move_down`, `move_left`, `move_right` via
`InputMap.AddAction` / `InputMap.ActionAddEvent` no `_Ready()`, usando o
enum `Godot.Key` (W/A/S/D + setas).

**Decisão deliberada:** os bindings de teclado NÃO foram escritos à mão
como blocos `[input]` em `project.godot`. Esse formato guarda cada tecla
como um `InputEventKey` serializado com códigos numéricos, e escrever isso
manualmente sem o editor Godot aberto para validar é frágil (erro silencioso
— tecla errada ou ação vazia não gera erro de compilação, só bug de
gameplay). Registrar em código com o enum `Key` é igualmente simples,
igualmente "data-driven o suficiente" para bindings, e elimina essa classe
de erro. Se no futuro isso incomodar (ex: querer que o jogador remapeie
teclas), migrar para Project Settings → Input Map é uma etapa isolada e de
baixo risco.

Desde a Etapa 2/3, `attack`, `dodge` e `block` também são registrados aqui
(Space / C / X — escolhas arbitrárias, fáceis de remapear depois;
deliberadamente teclas simples, não modificadoras como Shift/Ctrl, para
não depender de um nome de enum `Key` menos certo). Desde a Etapa 7,
`inventory` (tecla I) abre/fecha `InventoryUI`. Desde a Etapa 13, `stash`
(tecla T) abre/fecha `StashUI` — ação separada de `inventory` de propósito,
pra dar pra ter os dois painéis abertos ao mesmo tempo. `skill_1` e
`interact` continuam só reservados (comentados), sem implementação.

## Sistemas de combate (Game/Combat) — Etapa 2

Arquitetura em componentes pequenos e desacoplados (seção 43 do
MASTER_HANDOFF: "não colocar tudo dentro de PlayerController"):

- **`Health.cs`** (`Node`): `CurrentHealth`/`MaxHealth`, `TakeDamage(DamageInfo)`,
  sinais `Damaged(amount, currentHealth)` e `Died()`. Não sabe nada sobre
  hitboxes, input, IA ou loot.
- **`DamageInfo.cs`**: struct simples (Amount + Source). Existe pra que
  qualquer sistema futuro (skills, status effects, crafting) possa gerar
  dano da mesma forma, sem depender de como ele é aplicado.
- **`Combatant.cs`** (`Node`): marcador/agregador — só guarda `Team`
  (Player/Enemy/Neutral) e uma referência ao `Health` irmão (convenção:
  nó irmão chamado `Health`). Ainda não é usado por nenhum outro sistema
  (não há IA nem player-vs-enemy ainda), mas existe agora pra não precisar
  reabrir todo actor mais tarde.
- **`Hurtbox.cs`** (`Area2D`): "esta parte do actor pode ser atingida".
  Recebe `DamageInfo` e decide o que fazer com base em três propriedades
  que outros componentes setam: `Invulnerable` (dodge), `ParryWindowActive`
  (block, ver abaixo) e `DamageReductionRatio` (block). Não sabe quem está
  atacando nem nada sobre input.
- **`Hitbox.cs`** (`Area2D`, `Monitoring=false` por padrão): "isto causa
  dano enquanto ativo". Quem possui o ataque chama `Activate()`/`Deactivate()`.
  Rastreia quais Hurtboxes já atingiu na ativação atual (`_alreadyHit`) pra
  um único golpe não acertar o mesmo alvo em vários frames físicos
  seguidos enquanto as áreas se sobrepõem.
- **`Cooldown.cs`**: timer reutilizável simples, não é `Node` — quem usa
  (`PlayerBasicAttack`, `PlayerDodge`) chama `Tick(delta)` no próprio
  `_Process`. Evita criar mais nós só pra contar tempo.
- **`Dummy.tscn` / `Dummy.cs`**: alvo parado só pra provar que
  Health/Hitbox/Hurtbox funcionam ponta a ponta. Sem IA, sem morte de
  verdade, sem loot — isso é Etapa 4+. Dá feedback visual mínimo
  (`Modulate.A` cai conforme perde HP) e imprime no console; não é um
  sistema de "Hit Feedback" de verdade (esse é item futuro separado).
  Instanciado em `TestWorld.tscn` na posição `(200, -150)`.

**Camadas de física usadas:** para não interferir na colisão física já
validada (player/paredes/obstáculos ficam na layer/mask padrão = 1),
Hitbox/Hurtbox usam a layer 2 à parte: toda Hurtbox tem
`collision_layer=2, collision_mask=0`; toda Hitbox tem
`collision_layer=0, collision_mask=2`. Ou seja, só hitboxes "enxergam"
hurtboxes, e nada mais colide com essa layer.

## Player — ataque, dodge, block (Game/Actors/Player) — Etapa 2/3

- **`PlayerController.cs`** ganhou `FacingDirection` (última direção de
  movimento não-nula — usada por ataque/dodge como direção, já que não
  existe mira separada ainda) e `IsDodging`/`DodgeVelocity`.
  **Decisão importante:** a autoridade sobre `Velocity`/`MoveAndSlide()`
  continua 100% aqui, mesmo durante o dodge. `PlayerDodge` só seta essas
  duas propriedades (intenção); nunca chama `MoveAndSlide()` ele mesmo.
  Ter dois scripts chamando `MoveAndSlide()` no mesmo frame físico do
  mesmo corpo geraria comportamento brigando entre si — centralizar evita
  essa classe de bug.
- **`PlayerBasicAttack.cs`**: lê a ação `attack`, posiciona o
  `AttackHitbox` (Area2D irmã, filha de Player) na direção de
  `FacingDirection * AttackRange`, ativa por `AttackDuration` (0.15s) e
  aplica `CooldownDuration` (0.45s) via `Cooldown`.
- **`PlayerDodge.cs`**: dash na direção do input atual (ou `FacingDirection`
  se parado) por `DodgeDuration` (0.18s) a `DodgeSpeed` (520 px/s), com
  i-frame (`Hurtbox.Invulnerable = true`) durante a janela, e cooldown de
  0.6s. Só a "base" pedida na Etapa 3 — sem animação/trail.
- **`PlayerBlock.cs`**: segurar `block` reduz dano recebido em
  `BlockDamageReduction` (50% por padrão) via `Hurtbox.DamageReductionRatio`.
  **Base de parry:** nos primeiros `ParryWindowDuration` (0.15s) depois de
  apertar `block`, um hit é totalmente anulado e reportado via
  `Hurtbox.Parried` (sinal), em vez de sofrer a redução parcial do block
  normal — distinção que uma etapa futura pode usar pra stagger/contra-ataque
  no atacante, sem tocar neste arquivo. Não implementa stagger nem
  contra-ataque agora (esses dependem de sistemas que ainda não existem).

## Player (Game/Actors/Player)

- `Player.tscn`: `CharacterBody2D` (motion_mode = Floating, apropriado
  para top-down sem gravidade) com `CollisionShape2D` (círculo, raio 16),
  um `Polygon2D` como placeholder visual (quadrado azul) e um `Camera2D`
  filho — a câmera segue o player automaticamente por herdar a transform do
  pai, sem código extra.
- `PlayerController.cs`: controla apenas input/direção/movimento
  (`_PhysicsProcess` → `Input.GetVector` → `Velocity` → `MoveAndSlide()`).
  Sem HP, inventário, loot, quests, crafting ou diálogo — por design (ver
  MASTER_CONTEXT.md, regra do PlayerController).
  `Input.GetVector` já limita o vetor resultante a magnitude ≤ 1, então o
  movimento diagonal não fica mais rápido que o cardinal — não é preciso
  normalizar manualmente.
  `MoveSpeed` é `[Export]` (220 px/s por padrão) para poder ajustar pelo
  Inspector sem recompilar.
  Desde a Etapa 4, `_Ready()` chama `AddToGroup("player")` — é assim que
  `EnemyController` (mira/alvo de IA) e `GroundItem` (pickup) reconhecem o
  player sem precisar de uma referência cravada manualmente no editor.
  Escolhido em vez de escrever `groups=[...]` à mão em `Player.tscn` pelo
  mesmo motivo dos bindings de Input: um nome de grupo errado em código
  não compila (erro visível); o mesmo erro escrito à mão no `.tscn` falha
  silenciosamente.

## Inimigos e IA (Game/Actors/Enemies) — Etapa 4

- **`EnemyController.cs`** (`CharacterBody2D`): IA deliberadamente simples
  — um `enum State { Idle, Chase, Attack, Recover, Dead }` com um
  `switch` dentro de `_PhysicsProcess`, sem framework de state machine
  (seção 47 do MASTER_HANDOFF: "começar simples"; Patrol/Flee/Summon/
  PhaseTransition ficam para quando houver um segundo tipo de inimigo que
  realmente precise disso).
  - `Idle → Chase`: quando o player entra em `DetectionRange` (180px).
  - `Chase → Idle`: se o player sair de `LeashRange` (260px) — evita o
    inimigo perseguir pra sempre pra fora da área dele.
  - `Chase → Attack`: quando a distância até o player fica ≤ `AttackRange`
    (34px); nesse ponto o inimigo para, ativa o próprio `AttackHitbox`
    (reaproveitando `Hitbox.cs` de `Game.Combat`, sem nenhum código de
    combate novo) por `AttackDuration` (0.2s) e entra em `Recover`
    (0.5s) antes de poder atacar de novo ou voltar a perseguir.
  - Reaproveita **exatamente** os mesmos componentes de combate do player
    (`Health`, `Combatant`, `Hurtbox`, `Hitbox`) sem nenhuma alteração
    nesses arquivos — esse é o retorno de ter desacoplado `Game.Combat`
    de `PlayerController` já na Etapa 2: um inimigo é só mais um actor
    que possui os mesmos componentes.
  - Feedback visual (ainda placeholder, mesma ressalva do `Dummy`):
    `Modulate.A` do `Polygon2D` cai proporcionalmente ao HP restante ao
    tomar dano (`Health.Damaged`), e fica cinza semi-transparente ao
    morrer (`Health.Died`), desligando `_PhysicsProcess` e o `Hitbox`.
  - Ao morrer, chama `LootGenerator.Generate(...)` (ver seção Loot
    abaixo) na própria posição, contra o `GetParent()` (a cena raiz onde
    o inimigo foi instanciado) — `LootGenerator` é `GetNodeOrNull`, então
    um inimigo sem esse nó filho simplesmente não dropa nada, sem erro.
- **`Enemy.tscn`**: mesma estrutura de nós que `Player.tscn` para os
  componentes de combate (`CollisionShape2D` círculo r=16, `Placeholder`
  laranja, `Health` MaxHealth=40, `Combatant` Team=1, `Hurtbox`
  layer=2/mask=0, `AttackHitbox` layer=0/mask=2/monitoring=false), mais um
  nó `LootGenerator` com `GroundItemScene` apontando para
  `Game/Loot/GroundItem.tscn`.

## Loot básico (Game/Loot) — Etapa 5

Sem nenhuma referência a itens reais ainda — `ItemBaseDefinition`/
`ItemInstance`/Rarity só existem na Etapa 6+. Por enquanto "loot" é só uma
pickup genérica: o objetivo desta etapa é o cano drop → objeto no mundo →
pickup, não o conteúdo do item.

- **`DropTable.cs`**: classe C# simples (não `Resource` — mesma decisão de
  risco já tomada para `DamageInfo`: um `Resource` customizado exigiria
  `sub_resource` escrito à mão em `.tscn` sem o editor aberto pra
  validar). Guarda `DropChance`/`MinDrops`/`MaxDrops` e decide via
  `Roll(Random)` quantos drops genéricos acontecem (0 se a chance falhar).
  Quem possui o inimigo (`EnemyController`) expõe esses números como
  `[Export]` e monta um `DropTable` novo a cada morte.
- **`LootGenerator.cs`** (`Node`): único ponto do código que sabe
  instanciar `GroundItem` — futuras mudanças (variedade real de itens,
  cenas por raridade, efeitos de apresentação) mexem só neste arquivo, não
  em todo actor que possa dropar loot. `Generate(table, worldPosition,
  parent)` faz o roll e spawna cada `GroundItem` com um pequeno offset
  aleatório (até 20px) pra não empilhar múltiplos drops exatamente no
  mesmo pixel.
- **`GroundItem.cs`** (`Area2D`): pickup automático — `BodyEntered` checa
  `IsInGroup("player")` (mesmo grupo que `PlayerController` registra desde
  esta etapa) e, se for o player, emite o sinal `Collected` (preparado
  para um futuro sistema de Inventário/Etapa 7 escutar, sem precisar
  mudar este arquivo), imprime no console e `QueueFree()`.
  **Decisão deliberada:** usa a layer/mask de física padrão (1) — a
  mesma do player/paredes/obstáculos — em vez de uma layer dedicada,
  porque o que restringe a coleta ao player é o `IsInGroup("player")` no
  código, não a camada de colisão; nada mais no jogo hoje precisa
  interagir com loot no chão.
- **`GroundItem.tscn`**: `Area2D` raiz, `CollisionShape2D` (círculo,
  raio 8), `Polygon2D` amarelo como placeholder visual.

## Sistema de itens (Game/Items + Data/Items) — Etapa 6

Implementa só o que a Etapa 6 pede — ItemBaseDefinition, ItemInstance,
Rarity, ItemLevel, Tags — nada de Affixes (Etapa 9) ainda.

- **`ItemRarity.cs`**: enum `Normal/Magic/Rare/Unique` com valores
  explícitos e espaçados (0, 10, 20, 30) — deixa buraco pra caber
  `Relic/Ancient/Mythic/Corrupted` (mencionados no MASTER_HANDOFF como
  extensão futura) sem renumerar os existentes, e evita que uma raridade
  usada num save (Etapa 22, futuro) mude de valor silenciosamente se a
  ordem do enum for editada.
- **`ItemBaseDefinition.cs`**: classe C# simples (não Resource — mesma
  decisão de risco já repetida em DamageInfo/DropTable: um Resource
  customizado exigiria `sub_resource` escrito à mão em `.tscn`/`.tres` sem
  o editor aberto pra validar). Id/Name/Description/Tags/Stackable/
  MaxStackSize. Deliberadamente **sem** campo de slot de equipamento —
  isso é Etapa 8; por enquanto `Tags` (ex: "weapon", "sword", "melee",
  "currency") já descreve o que o item é, seguindo a lista de tags do
  MASTER_HANDOFF seção 18-24.
- **`ItemDatabase.cs`**: singleton estático simples (não autoload —
  nada aqui precisa de `_Ready()`/árvore de cena, então um Node só pra
  isso seria cerimônia desnecessária) que carrega
  `res://Data/Items/items.json` uma vez, via `Godot.FileAccess` (não
  `System.IO` — só `Godot.FileAccess` funciona tanto no editor quanto
  numa build exportada, onde `res://` vira parte do `.pck`) +
  `System.Text.Json`. JSON foi escolhido em vez de `.tres` exatamente
  pelo mesmo motivo de `ItemBaseDefinition` não ser Resource: é texto
  simples, sem risco de serialização Godot malformada, e ainda assim
  satisfaz a regra "dados criam conteúdo" (MASTER_HANDOFF seção 37-40) —
  conteúdo de item vive fora do código, só isso já muda sem recompilar.
- **`ItemInstance.cs`**: item concreto (dropado, no inventário, ou
  futuramente equipado) — `InstanceId` (Guid), `BaseId` (aponta pro
  `ItemBaseDefinition` por id, não copia os dados), `Rarity`,
  `ItemLevel`, `StackCount`. `ItemLevel` existe no modelo agora porque a
  Etapa 6 pediu, mas **nada ainda lê esse valor pra limitar affixes** —
  isso só passa a importar na Etapa 9. Sem sistema de nível de
  inimigo/área ainda, então quem cria uma instância (hoje: só
  `LootGenerator`) usa um valor fixo — ver seção do LootGenerator abaixo.
- **`Data/Items/items.json`**: catálogo inicial com 5 bases genéricas (2
  armas, 1 peça de armadura, 1 "currency" e 1 "consumable" — nomes e
  efeitos totalmente placeholder, sem nenhuma lógica de crafting/consumo
  implementada ainda, só existem como itens que podem ser encontrados e
  guardados).

## Inventário (Game/Inventory) — Etapa 7

- **`Inventory.cs`** (`Node`): componente-filho do Player (mesma
  composição de Health/Combatant/Hurtbox — nó chamado "Inventory" em
  `Player.tscn`). `Capacity` (`[Export]`, 20 por padrão) slots.
  `AddItem(ItemInstance)`: se o `ItemBaseDefinition` é `Stackable`,
  tenta somar numa stack existente do mesmo `BaseId` que ainda tenha
  espaço (`MaxStackSize`); senão, cria um slot novo se houver espaço.
  Tudo ou nada — não faz split parcial de uma pickup entre duas stacks
  (mantém a lógica simples; nada ainda dropa quantidades grandes o
  bastante pra isso importar na prática). Retorna `false` sem alterar
  nada se não houver espaço, e quem chamou decide o que fazer (ver
  `GroundItem` abaixo). `RemoveItem(instanceId, amount)`: remove até
  `amount` unidades, retorna quanto foi de fato removido; existe como
  API pronta pra Equipment/Crafting/consumíveis (etapas futuras)
  chamarem — nada no jogo ainda chama `RemoveItem` sozinho. Emite o
  sinal `InventoryChanged` (sem parâmetros, mesma convenção de
  `Health.Died`) em qualquer alteração, pra UI só precisar re-renderizar
  a lista inteira em vez de rastrear diffs.
- **Integração com loot (`GroundItem.cs`, `LootGenerator.cs`
  atualizados)**: `GroundItem` ganhou uma propriedade `Payload`
  (`ItemInstance`, não `[Export]` — é um objeto C# puro, o Inspector não
  teria como editá-lo mesmo). `LootGenerator` agora sorteia uma
  `ItemBaseDefinition` (uniforme entre todo o `ItemDatabase` — ainda não
  há pool por inimigo, isso é território de "DropTable com pesos/pools"
  do MASTER_HANDOFF seção 41-42, não implementado agora) e uma
  `ItemRarity` (Normal 70% / Magic 25% / Rare 5% — **Unique deliberadamente
  excluída**: um Unique de verdade é um item específico com stats fixos
  definidos à mão, não "base genérica + tag Unique", e esse sistema não
  existe ainda), monta um `ItemInstance` com `ItemLevel` fixo em 1
  (placeholder documentado — sem sistema de nível de inimigo/área) e
  passa pro `GroundItem.Payload` antes de instanciar. Ao encostar,
  `GroundItem` busca `Inventory` no player (`GetNodeOrNull<Inventory>("Inventory")`,
  mesma convenção de busca por nome de nó já usada em outros lugares) e
  só desaparece (`QueueFree`) se `AddItem` retornar `true` — inventário
  cheio significa que o item continua no chão, sem lógica extra
  necessária.
- **`UI/InventoryUI.cs` + `InventoryUI.tscn`**: "UI básica" pedida pela
  Etapa 7 — um painel `Panel`/`VBoxContainer` com uma `Label` por item
  (nome + `[Raridade]` quando não-Normal + `xN` quando stack > 1),
  alternado por `Input.IsActionPressed("inventory")` (tecla `I`,
  registrada agora em `InputBootstrap.cs` — a ação já existia reservada
  desde a Etapa 1). Construído inteiramente em código (`new Panel {...}`)
  em vez de nós `Control` escritos à mão no `.tscn` — mesma razão de
  sempre: anchors/margins precisos em `.tscn` cru são exatamente o tipo
  de coisa que não dá pra validar sem o editor Godot aberto, enquanto
  `new Panel { Position = ... }` em C# é checado em tempo de compilação.
  Vive como `CanvasLayer` irmão de `TestWorld` em `Scenes/Main.tscn` (não
  filho do Player) pra renderizar em espaço de tela independente da
  câmera, e encontra o `Inventory` do player em tempo de execução via
  grupo `"player"` (mesmo padrão do `EnemyController`), adiado por um
  frame (`Callable.From(() => ...).CallDeferred()`) pra não depender da
  ordem de instanciação entre `TestWorld`/`Player` e `InventoryUI`.
  Sem grid, sem ícones, sem drag-and-drop, sem equipar/dropar pela UI —
  nada disso foi pedido ainda (Equipment é Etapa 8, Stash com
  busca/tabs é Etapa 13).

## Equipamento (Game/Inventory/Equipment.cs) — Etapa 8

`Equipment` (componente-filho do Player, mesma composição de `Inventory`)
guarda um `ItemInstance` por `EquipmentSlot` — os 9 slots pedidos pelo
MASTER_HANDOFF (Weapon/Offhand/Helmet/Chest/Gloves/Boots/Amulet/Ring1/Ring2).

- **Dois enums, não um**: `EquipmentSlot` (os 9 slots concretos do
  personagem) e `EquipmentSlotCategory` (o que um `ItemBaseDefinition`
  *é* — `Weapon/Offhand/.../Ring`, 8 categorias). Existem separados por
  causa do anel: há dois slots de anel concretos mas só uma categoria
  "Ring" — um item anel não deveria precisar dizer "eu sou pro Ring1" ou
  "Ring2" especificamente. `Equipment.ResolveSlot` resolve a categoria
  Ring pro primeiro slot vazio (Ring1, senão Ring2; se os dois já
  estiverem ocupados, substitui Ring1 — regra determinística, documentada
  no código).
- **`Equip(instance, inventory)`**: exige que o item já esteja no
  `Inventory` passado (não puxa de lugar nenhum sozinho). Se o slot já
  tiver algo, tenta devolver pro inventário primeiro — se não houver
  espaço lá, a operação inteira é abortada e nada muda (sem troca
  parcial, sem item perdido). `Unequip(slot, inventory)` é o inverso.
- **Escopo deliberadamente limitado**: equipar/desequipar só move o
  `ItemInstance` entre `Inventory` e `Equipment` — **não** altera nenhum
  stat do jogador ainda (dano, defesa, etc.). Ligar uma arma equipada ao
  `PlayerBasicAttack.Damage`, por exemplo, exigiria um sistema de "stats
  efetivos calculados a partir do equipamento + afixos" que não foi
  pedido por nenhuma etapa do ROADMAP ainda — decisão documentada, não
  esquecida.
- **UI**: `InventoryUI` (ver seção própria abaixo) transformou cada item
  equipável do inventário num `Button` — clicar equipa; a seção
  "Equipado" mostra os 9 slots (vazios ou com botão pra desequipar).

## Afixos (Game/Items/Affix*.cs + Data/Items/affixes.json) — Etapa 9

- **`AffixDefinition`**: o mesmo padrão JSON-não-Resource de
  `ItemBaseDefinition`, carregado por `AffixDatabase`
  (`Data/Items/affixes.json`, 8 entradas: 4 prefix, 4 suffix, cobrindo
  tags "weapon"/"armor"/"boots"/"caster" e duas universais —
  `RequiredTags` vazio = "qualquer item equipável"). Cada entrada tem
  `Tier` (1 = melhor, convenção do gênero), `RequiredItemLevel` (hoje
  sempre 1, porque `ItemLevel` ainda é o placeholder fixo da Etapa 6/7 —
  o campo existe pra quando houver nível de verdade), `MinValue`/
  `MaxValue` (faixa do roll) e `Weight` (peso pro sorteio ponderado).
- **`AffixInstance`**: afixo já rolado num `ItemInstance` específico —
  aponta pro `AffixDefinition` por id (mesmo padrão de ponteiro-não-cópia
  de `ItemInstance.BaseId`) + o valor sorteado. `ToDisplayText()` formata
  usando o `NameTemplate` da definição (ex: `"+{0} Dano Físico"` vira
  `"+4 Dano Físico"`).
- **`AffixRoller`**: toda a lógica de sorteio num lugar só (usado tanto
  pelo `LootGenerator` quanto pelos `CraftingEffect`s da Etapa 10, pra
  não duplicar a regra de cap/pool ponderado em dois lugares).
  **Regras simplificadas e documentadas como tal** (o MASTER_HANDOFF só
  pede "Prefix/Suffix, tiers, pools, weighted rolls", não um balanceamento
  final): Normal = 0 afixos; Magic cap 2 (1 prefix + 1 suffix); Rare cap
  4 (2 prefix + 2 suffix). Um Magic recém-dropado rola 1 afixo, um Rare
  rola 2 — mas o tipo (prefix/suffix) de cada rolagem depende de qual
  pool ainda tem espaço, então não é garantido "sempre 1 de cada" num
  Rare, só "até 2 de cada, no máximo 4 total". Nunca rola o mesmo afixo
  duas vezes no mesmo item.
- **Integração com loot**: `LootGenerator.RollItemInstance` agora chama
  `AffixRoller.RollInitialAffixes` depois de decidir a raridade. **Bug
  encontrado e corrigido nesta etapa**: bases `Stackable` (currency/
  consumível) antes podiam sortear raridade Magic/Rare também (foi
  exatamente o que apareceu no vídeo da Etapa 6+7: "Fragmento de Poeira
  Antiga [Magic] x2") — não fazia sentido, currency não tem raridade no
  sentido de itemização. Agora `RollItemInstance` força `ItemRarity.Normal`
  pra qualquer base `Stackable`, então só itens equipáveis rolam
  raridade/afixos.

## Crafting (Game/Crafting) — Etapa 10

Implementa "primeiros CraftingEffects... nomes temporários somente em
dados" (MASTER_HANDOFF seção 37-40), usando os próprios nomes de classe
sugeridos pelo documento.

- **`ICraftingEffect`**: `CanApply(target, targetDefinition)` /
  `Apply(...)`. Quatro implementações, cada uma sem saber qual item de
  currency específico a aciona:
  - `MakeMagicEffect`: Normal → Magic, rola o afixo natural (1).
  - `RerollMagicModifiersEffect`: exige Magic, rerola os afixos existentes.
  - `AddModifierEffect`: exige Magic/Rare com espaço sob o cap da
    raridade, adiciona mais um afixo.
  - `UpgradeToRareEffect`: exige Magic, vira Rare e rola mais um afixo
    (mantém os que já tinha).
- **`ItemBaseDefinition.CraftingEffectId`** (novo campo, Etapa 10): liga
  um item de currency (`items.json`) a um efeito por id de string
  (`"make_magic"`, etc.) — é a "CraftingCurrencyDefinition" do
  MASTER_HANDOFF, resolvida via `CraftingEffectRegistry`. Isso é
  deliberado pra nunca precisar de `if (item.Id == "dust_fragment")` em
  lugar nenhum do código (regra central da seção 37-40) — trocar de nome
  ou reequilibrar qual currency faz o quê é só editar o JSON.
  Catálogo atual: `dust_fragment` → make_magic, `shard_reforging` →
  reroll_magic, `crystal_amplification` → add_modifier, `orb_ascension` →
  upgrade_to_rare.
- **`CraftingService.TryApply(currencyDefinition, target, targetDefinition)`**:
  ponto de entrada único. Deliberadamente **não mexe em nenhum
  Inventory** — só muta o `ItemInstance` alvo e retorna se deu certo;
  consumir a currency (remover 1 unidade de onde ela estava) é
  responsabilidade de quem chamou. Mantém a classe reutilizável (UI hoje,
  uma futura bancada de crafting depois) sem acoplar a uma fonte
  específica de onde a currency vem.
- **UI é um placeholder deliberado, documentado como tal**: não existe
  nenhuma etapa numerada pedindo uma UI de crafting de verdade (seleção
  de dois itens, preview, etc. — isso seria território de UI avançada
  não solicitada). `InventoryUI` faz uma versão mínima só pra o sistema
  ser testável: cada currency vira um botão "[usar]" que aplica seu
  efeito automaticamente **no primeiro item elegível ainda no
  inventário** (busca simples, sem seleção manual). Se nada for elegível,
  imprime uma mensagem e não consome a currency.

## Apresentação de loot (Game/Loot/LootPresentation.cs) — Etapa 11

Implementa "label, cor, borda, glow, partículas, beam... Rare com visual
moderado, Unique com visual grande" (MASTER_HANDOFF seção 28-36) — **sem
som**, ver abaixo por quê.

- **`LootPresentation.Style`**: struct só-leitura (Color + 4 bools:
  ShowOutline/ShowGlow/ShowBeam/ShowParticles) por `ItemRarity`, num
  `Dictionary` estático. Normal só ganha cor+label; Magic ganha contorno;
  Rare ganha contorno+glow+beam ("moderado"); Unique ganha tudo, incluindo
  partículas ("grande") — mesmo sem `LootGenerator` sortear Unique ainda
  (ver seção Afixos), o estilo já existe pronto.
- **Deliberadamente código, não JSON**, diferente de items/affixes: só 4
  raridades fixas, valores de direção de arte/placeholder (mais perto dos
  caps hardcoded de `AffixRoller` do que de conteúdo real). A Etapa 12
  (Loot Filter) é a camada data-driven de verdade por cima disso.
- **`GroundItem.ApplyPresentation()`** (chamado em `_Ready()`, só quando
  `Payload != null`): monta os nós filhos **inteiramente em código**, não
  no `.tscn` — mesmo motivo de sempre (`InventoryUI`): múltiplos tipos de
  nó novos (`Label`, `CpuParticles2D`) e posicionamento preciso são mais
  seguros como atribuição de propriedade em C# do que como blocos `.tscn`
  escritos à mão sem o editor pra validar. Nenhum desses nós precisa de
  `ext_resource`/`sub_resource` (só propriedades simples), então
  `GroundItem.tscn` em si não mudou — `load_steps` continua o mesmo.
  - **Label**: nome + `xN` se empilhável, cor = cor da raridade, acima do
    item.
  - **Outline**: `Polygon2D` levemente maior atrás do item (`ZIndex=-1`).
  - **Glow**: `Polygon2D` maior e semi-transparente atrás de tudo
    (`ZIndex=-2`), alpha pulsando para sempre via `Tween`
    (`CreateTween().BindNode(glow)` — `BindNode` é essencial aqui: sem
    ele, o Tween continuaria tentando animar o nó depois de
    `QueueFree()` na coleta do item e gera erro em runtime; com
    `BindNode`, o Godot mata o Tween sozinho quando o nó é liberado).
  - **Beam**: coluna fina semi-transparente subindo a partir do item,
    mais alta/larga para Unique que para Rare.
  - **Partículas**: `CpuParticles2D` (não `GpuParticles2D` — não precisa
    de `ParticleProcessMaterial`, é só propriedades simples no próprio
    nó, risco bem menor pra código nunca compilado aqui) configurado
    inteiramente em código, `Emitting=true`, subindo devagar.
- **Som deliberadamente fora desta etapa, documentado como não-objetivo**:
  não existe nenhum asset de áudio neste repositório, e não há como
  importar/validar um sem o editor Godot aberto. Um `AudioStreamPlayer2D`
  sem `Stream` seria só código morto. Quando existir áudio real, o lugar
  natural é dentro de `ApplyPresentation`, ao lado dos outros efeitos.

## Loot Filter (Game/Loot/LootFilter*.cs + Data/Loot/loot_filter.json) — Etapa 12

Implementa "rules com conditions... e actions..." inspirado no FilterBlade
(MASTER_HANDOFF seção 28-36) — a camada data-driven de verdade por cima da
Etapa 11.

- **`LootFilterRule`**: uma regra = condições (`Tags`, `RarityIn`,
  `MinItemLevel`/`MaxItemLevel`, `IsCurrency` — todas opcionais, null/vazio
  = "não importa") + ações (`Show` obrigatório; `FontColor`, `FontSize`,
  `ShowOutline`/`OutlineColor`, `ShowGlow`/`GlowColor`, `ShowBeam`/
  `BeamColor`, `ShowParticles` — todas opcionais, null = "herda o default
  da Etapa 11 pra essa raridade"). `Matches(instance, definition)` checa
  todas as condições setadas.
  - **Condições propositalmente NÃO implementadas** (documentado, não
    esquecido): base type/class/slot/affixes/tiers/quantidade/quest
    item/sockets — "class" e "sockets" não existem como sistemas ainda,
    "slot" duplicaria `EquipmentSlotCategory` sem nenhuma regra
    precisando disso ainda, e condições por afixo pedem uma
    mini-linguagem de query própria. Nada disso vale a pena inventar
    especulativamente pra um filtro que hoje só precisa lidar com 10
    bases e 8 afixos.
  - **Ações propositalmente NÃO implementadas**: Sound/SoundVolume (sem
    assets de áudio, mesma razão da Etapa 11) e MinimapIcon/MinimapColor
    (não existe nenhum sistema de minimapa ainda).
- **`LootFilterColor`**: cor JSON-friendly como `{r,g,b,a}` (doubles), não
  string hex — evita depender de uma API de parsing de `Color(string)`
  nunca exercitada neste projeto, quando o construtor de 4 floats já é
  comprovadamente seguro (usado em todo `.tscn` existente).
- **`LootFilterDatabase`**: mesmo padrão singleton-lazy +
  `Godot.FileAccess` + `System.Text.Json` de `ItemDatabase`/
  `AffixDatabase`, carregando `Data/Loot/loot_filter.json`.
  `Evaluate(instance, definition)`: percorre as regras **em ordem, a
  primeira que bater vence** (convenção FilterBlade/ARPG real); se
  nenhuma bater, retorna exatamente o default da Etapa 11 (sempre
  mostrado). Arquivo ausente/vazio não é erro — só significa "sem regras
  customizadas".
- **`Data/Loot/loot_filter.json`** (4 regras, nesta ordem): mostra
  currency (label dourado) → mostra consumível (label verde) → mostra
  Rare/Unique com contorno+glow+beam → **esconde Normal** (a regra
  "catch-all" que sobra pra bases equipáveis comuns). Isso demonstra o
  comportamento clássico de loot filter (esconder lixo branco) com os
  dados que o jogo já tem, sem precisar de item level/afixos reais ainda.
  Magic não tem regra própria — cai no fallback da Etapa 11 (sempre
  visível, contorno azul).
- **"Hide" = invisível, mas continua coletável**: `GroundItem.Visible =
  result.Show`. `Area2D`/física em Godot é independente de
  `CanvasItem.Visible` — esconder um item não desliga sua colisão, então
  `OnBodyEntered` continua disparando normalmente. Documentado
  explicitamente porque é uma escolha de design (não um bug) — a
  alternativa ("Hide" = não spawna o item) mudaria `LootGenerator`, não
  só a apresentação, e não foi pedida.
- **`GroundItem.ApplyPresentation()`** agora chama
  `LootFilterDatabase.Instance.Evaluate(...)` em vez de ler
  `LootPresentation.GetStyle(...)` diretamente — a Etapa 11 continua
  existindo por baixo (é o fallback), só que agora com uma camada de
  regras data-driven na frente.

## Stash (Game/Inventory/Stash.cs + UI/StashUI.cs) — Etapa 13

Implementa "Stash cedo no dev: General/Equipment/Currency/Unique"
(MASTER_HANDOFF seção 25-27).

- **`StashCategory`**: enum com as 4 abas (`General`/`Equipment`/
  `Currency`/`Unique`).
- **`Stash`** (`Node`, componente-filho do Player — mesmo padrão de
  composição que `Inventory`/`Equipment`): guarda um
  `Dictionary<StashCategory, List<ItemInstance>>`, uma lista por aba, com
  `CapacityPerCategory` (`[Export]`, default 30) cada.
  - **`ResolveCategory(instance, definition)`**: decide a aba
    automaticamente a partir de dados que o item já tem — `Rarity ==
    Unique` vence primeiro (um Unique equipável ainda vai pra aba Unique,
    não Equipment), depois `IsEquipable` → Equipment, depois
    `IsCraftingCurrency` → Currency, resto → General. Nenhum check por
    id de item, mesma regra central de sempre.
  - **`Store(instance, inventory)`** / **`Retrieve(instance, inventory)`**:
    mesma segurança "checa espaço antes de remover da origem" que
    `Equipment.Equip`/`Unequip` — se o destino não tem espaço, nada muda
    e o item fica onde estava.
- **Localização deliberadamente provisória, documentada como tal**: um
  Stash de verdade pertence à conta/save do jogador, não ao personagem
  em si — mas não existe base/vila, nem persistência (Etapa 22), nem
  conceito de múltiplos personagens ainda, então não há outro lugar
  sensato pra guardar esse estado agora. A API pública não assume nada
  sobre *onde* isso mora, então mover pra um autoload salvo depois é uma
  realocação, não uma reescrita.
- **`StashUI`**: painel separado do `InventoryUI` (tecla `stash` = T,
  independente de `inventory` = I — dá pra ter os dois abertos ao mesmo
  tempo), mostrando o inventário atual (clique = guardar) e as 4 abas do
  baú (clique num item guardado = retirar). Mesmo padrão 100%-código de
  `InventoryUI`, mesma razão (`.tscn` cru não valida sem o editor).
- **Sem busca/ordenação/tabs de UI de verdade** — isso é explicitamente
  "depois" no próprio MASTER_HANDOFF; `StashUI` só lista cada categoria
  na ordem de inserção.
- **Acessível de qualquer lugar, a qualquer momento**: não existe
  gatekeeping por localização/NPC ainda (isso é território de uma etapa
  de vila/mundo futura, não pedida agora).

## TestWorld (Game/World/TestWorld.tscn)

Sala fechada para testar movimentação/colisão/combate: `Floor` (visual,
sem colisão), `Walls` (4 `StaticBody2D` formando o perímetro),
`Obstacles` (2 `StaticBody2D` soltos na sala), uma instância de `Dummy`
em `(200, -150)` (Etapa 2, alvo parado) e, desde a Etapa 4, uma instância
de `Enemy` em `(-200, 150)` (quadrado laranja, persegue/ataca o player e
dropa loot ao morrer). Tudo com `Polygon2D` como placeholder visual —
nenhuma arte final é esperada nesta etapa.

## project.godot — o que foi adicionado

- `[application] run/main_scene="res://Scenes/Main.tscn"`
- `[autoload] InputBootstrap="*res://Game/Core/InputBootstrap.cs"`

Todo o resto (`config/features`, `[dotnet]`, `[display]`, `[physics]`,
`[rendering]`) foi preservado exatamente como estava — nenhuma configuração
pré-existente do projeto foi alterada.

## Pendência conhecida de tooling

A Etapa 0+1 foi escrita e commitada a partir de um ambiente sem o editor
Godot e sem o SDK .NET instalados, e foi **verificada depois pelo usuário
localmente** (vídeo conferido nesta sessão: player aparece, move nas 8
direções, colide com paredes/obstáculos, câmera segue — bate exatamente
com o `TestWorld.tscn` descrito acima). A Etapa 2+3 (combate/dodge/block)
foi escrita no mesmo ambiente sem Godot/dotnet e **verificada depois pelo
usuário** (segundo vídeo conferido). A Etapa 4+5 (inimigo/loot) também foi
**verificada depois pelo usuário** (terceiro vídeo, conferido quadro a
quadro nesta sessão: IA persegue/ataca/morre, loot dropa e é coletado —
ver HANDOFF.md Entrada 6 para o detalhe exato do que foi confirmado e do
único ponto que continua sem confirmação visual, dano no player). A Etapa
6+7 (itens/inventário) também foi **verificada pelo usuário** (quarto
vídeo — compilou, JSON carregou, itens e stacking corretos no painel; foi
nesse vídeo que se percebeu o bug de currency sorteando raridade,
corrigido nesta etapa — ver seção "Afixos" acima). A Etapa 8+9+10
(equipamento/afixos/crafting) também foi **verificada pelo usuário**
(quinto vídeo — compilou limpo (0 erros no painel de saída do Godot),
arma equipada aparece no slot Weapon com raridade/afixo corretos, e o bug
de raridade em currency da Entrada 7 se confirmou corrigido; a aplicação
de um efeito de crafting via botão "[usar]" não foi clicada nesse vídeo,
então segue sem confirmação visual — ver HANDOFF.md Entrada 8). A Etapa
11+12+13 (apresentação de loot/loot filter/stash) foi escrita da mesma
forma, sem Godot/dotnet neste ambiente, e **ainda não foi aberta no Godot
nem testada** — ver CURRENT_STATUS.md/HANDOFF.md para o checklist exato
pendente; esta é a etapa com mais superfície nova de API nunca exercitada
por um compilador real neste projeto (`Tween`/`CpuParticles2D`/`Label`
como filho de `Area2D`, todos verificados via busca antes de escrever,
não por experiência prévia no próprio código). O `.csproj`
segue o template mínimo que o próprio Godot 4.7 gera
(`Godot.NET.Sdk/4.7.2`, `net8.0`); o `.sln` não foi criado à mão — o Godot
gera/repara isso automaticamente ao abrir o projeto ou via
**Project → Tools → C# → Create C# Solution**.

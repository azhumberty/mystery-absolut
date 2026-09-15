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
│   │   └── Player/
│   │       ├── Player.tscn
│   │       └── PlayerController.cs
│   └── World/
│       └── TestWorld.tscn       # sala de teste: chão, paredes, obstáculos
├── UI/                           # reservado, vazio (ver UI/README.md)
└── Data/                         # reservado, vazio (ver Data/README.md)
```

Pastas do ROADMAP futuro (Combat/, Items/, Crafting/, Inventory/, Loot/,
Narrative/, Relationships/, Quests/, World/Save, Audio/, Tests/, Tools/)
ainda não existem — serão criadas apenas quando a etapa correspondente
precisar delas (ver seção 55/65 do MASTER_HANDOFF).

## Namespaces

- `Game.Core` — infraestrutura transversal (hoje: bootstrap de Input).
- `Game.Actors.Player` — controlador do jogador.

Novos sistemas devem seguir o mesmo padrão: `Game.<Área>` (ex:
`Game.Combat`, `Game.Items`, `Game.Inventory`, `Game.Loot`,
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

Nomes de ação reservados para etapas futuras (`attack`, `skill_1`, `dodge`,
`block`, `interact`, `inventory`) estão comentados no próprio
`InputBootstrap.cs`, não implementados.

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

## TestWorld (Game/World/TestWorld.tscn)

Sala fechada apenas para testar movimentação/colisão: `Floor` (visual,
sem colisão), `Walls` (4 `StaticBody2D` formando o perímetro) e
`Obstacles` (2 `StaticBody2D` soltos na sala). Tudo com `Polygon2D` como
placeholder visual — nenhuma arte final é esperada nesta etapa.

## project.godot — o que foi adicionado

- `[application] run/main_scene="res://Scenes/Main.tscn"`
- `[autoload] InputBootstrap="*res://Game/Core/InputBootstrap.cs"`

Todo o resto (`config/features`, `[dotnet]`, `[display]`, `[physics]`,
`[rendering]`) foi preservado exatamente como estava — nenhuma configuração
pré-existente do projeto foi alterada.

## Pendência conhecida de tooling

Este handoff foi escrito e commitado a partir de um ambiente sem o editor
Godot e sem o SDK .NET instalados (não foi possível rodar `dotnet build`
nem abrir o projeto no Godot para validar visualmente). O `.csproj` segue
exatamente o template mínimo que o próprio Godot 4.7 gera
(`Godot.NET.Sdk/4.7.2`, `net8.0`), mas o `.sln` não foi criado à mão — o
Godot gera/repara isso automaticamente ao abrir o projeto ou via
**Project → Tools → C# → Create C# Solution**. Ver CURRENT_STATUS.md para
a lista exata do que ainda precisa ser verificado localmente.

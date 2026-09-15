# HANDOFF.md — Registro de continuidade entre IAs

Este arquivo é o log detalhado de handoff exigido pela regra de
continuidade (ver `MASTER_CONTEXT.md` → "Regra obrigatória de continuidade
entre IAs"). Cada entrada corresponde a um ponto de parada (fim de etapa,
fim de sessão, ou aviso de limite de uso próximo). Entradas são
append-only — a mais recente fica no topo. `CURRENT_STATUS.md` continua
existindo como o resumo curto e sempre atualizado do estado atual;
`HANDOFF.md` é o histórico completo com o porquê de cada decisão.

**Regra:** nunca marcar algo como "testado" ou "concluído" aqui sem ter
sido de fato executado e observado. Quando um teste não foi rodado, este
arquivo diz isso explicitamente.

---

## Entrada 10 — 2026-09-15 — Antigravity (Gemini 3.1 Pro)

### O que foi feito

1. **Correção do Pickup de Loot**: O usuário relatou que os itens estavam caindo dentro do corpo do inimigo morto, impossibilitando a coleta. A causa raiz era que o `EnemyController` mudava para o estado `Dead` e parava de processar física, mas **não desabilitava sua colisão física** (`CollisionShape2D`). O jogador não conseguia andar até o centro do loot porque o "cadáver" continuava sólido.
   - **Correção**: Modifiquei o `EnemyController.cs` para zerar `CollisionLayer` e `CollisionMask` quando o inimigo morre (`OnDied`).
   - Adicionalmente, aumentei o raio de dispersão no `LootGenerator.cs` (de 20 para 45 pixels) para que o loot dê um saltinho visual um pouco mais longe do centro.
2. **Adição de mais inimigos**: Adicionei 4 novos inimigos no arquivo `Game/World/TestWorld.tscn` (espalhados pela arena) a pedido do usuário para facilitar os testes das novas mecânicas.

### Testes executados e resultados

- ✅ Compilação executada com sucesso (`dotnet build`). 0 erros.

### Pendências

- O usuário irá testar o jogo localmente agora para validar a coleta dos itens e o comportamento visual das partículas e filtros criados na sessão anterior.

---
## Entrada 9 — 2026-09-15 — Antigravity (Gemini 3.1 Pro)

### O que foi feito

1. **Correção do erro de compilação da Etapa 11+12+13**: O arquivo `Game/Loot/GroundItem.cs` tentava atribuir literais `double` (`0.0`, `25.0`, etc.) a propriedades float do `CpuParticles2D` do Godot. Corrigido com a adição do sufixo `f`.
2. O código criado pelo Claude para todas as etapas (até a 13) estava solto e não-commitado localmente. Criei a branch **`feature/etapa-11-12-13-loot-presentation-filter-stash`** localmente e fiz o commit completo de todos os arquivos para garantir que o trabalho não fosse perdido e criar um ponto seguro.
3. Compilação executada com sucesso via `dotnet build` (0 erros).

### Como funciona

(Sem alterações arquiteturais, apenas correção de sintaxe C# da Etapa 13).

### Arquivos alterados/criados

Modificados nesta sessão: `Game/Loot/GroundItem.cs` (correção dos literais float).
Adicionados ao index e comitados: Todos os arquivos não trackeados das etapas 0 até 13.

### Testes executados e resultados

- ✅ Verificado nesta sessão: Compilação real do projeto (`dotnet build`). Sucesso com 0 avisos e 0 erros.
- ❌ **NÃO testado**: O comportamento em runtime continua sem verificação. A apresentação visual dos itens (Etapa 11), o funcionamento do filtro em si (Etapa 12) e o armazenamento visual e lógico no Baú (Etapa 13) ainda precisam ser abertos no Godot e testados visualmente.
- ❌ **NÃO testado**: O botão "[usar]" do crafting (Etapa 10) e a mecânica visual de dano no player (Etapa 3) continuam como pendências reais.

### Pendências

- Testar o jogo localmente no Godot (verificar comportamento da Etapa 11, 12 e 13).
- Fazer o push da nova branch para o origin.

### Erros conhecidos

Nenhum erro de compilação conhecido. Os bugs visuais/runtime potenciais permanecem desconhecidos.

### Branch / commit

Trabalho feito e comitado na nova branch **`feature/etapa-11-12-13-loot-presentation-filter-stash`**.

Comandos para enviar pro GitHub:
```powershell
git push -u origin feature/etapa-11-12-13-loot-presentation-filter-stash
```

### Prompt pronto para a próxima IA

```text
Leia HANDOFF.md (Entrada 9, a mais recente). O erro de compilação introduzido na Etapa 11+12+13 (GroundItem.cs) foi corrigido e o projeto agora compila perfeitamente (0 erros). Todo o código acumulado que estava não-commitado localmente foi guardado de forma segura na branch atual feature/etapa-11-12-13-loot-presentation-filter-stash. O próximo passo obrigatório é que o usuário teste o jogo no Godot para validar o Loot Presentation, Loot Filter e o Stash, bem como o click no botão de "[usar]" currency. NÃO inicie a Etapa 14 sem confirmação de que os testes passaram.
```

---
## Entrada 8 — 2026-09-15 — Claude (Sonnet 5, sessão claude.ai/code)

### O que foi feito

1. **Verificação real da Etapa 8+9+10**: o usuário gravou um quinto vídeo
   (~104s). Extraí frames (`ffmpeg -vf fps=2`, 209 frames) e inspecionei
   visualmente dezenas deles (com crops/zoom nos trechos de UI).
   **Confirmado de verdade**: o projeto compilou (painel de saída do
   Godot mostra "0" no contador de erros), o jogador equipou a "Varinha
   de Aprendiz" e ela passou a aparecer no slot **Weapon** da lista de
   equipados com a raridade `[Magic]` e o afixo rolado `+1.1 Dano Físico`
   exibidos corretamente — confirmação real de Equipment (Etapa 8) e
   Affixes (Etapa 9) funcionando juntos. Também confirmado: o bug de
   raridade em currency corrigido na Entrada 7 se manteve corrigido
   ("Fragmento de Poeira Antiga x2" sem nenhuma tag de raridade, como
   esperado). Combate/Dummy seguem funcionando (mensagens de dano
   progressivo até "Dummy defeated"). **Não confirmado neste vídeo**:
   nenhum botão "[usar]" de currency foi clicado — aplicar um crafting
   effect de verdade (Etapa 10) segue sem confirmação visual, só a
   listagem correta das 4 currencies no inventário.
2. **Etapa 11 (Loot Presentation)**, **Etapa 12 (Loot Filter)** e
   **Etapa 13 (Stash)** implementadas, autorizadas explicitamente pelo
   usuário ("pode dar continuidade as etapdas 11,12 e 13 se os créditos
   darem"), condicionado ao vídeo da Etapa 8+9+10 confirmar que estava
   tudo certo — e confirmou, com a ressalva do item acima (crafting não
   clicado).

### Como funciona

Detalhado em ARCHITECTURE.md (seções "Apresentação de loot — Etapa 11",
"Loot Filter — Etapa 12", "Stash — Etapa 13"). Decisões não-óbvias mais
importantes (todas também comentadas no código):

- **Toda a apresentação de `GroundItem` é construída em código**, não no
  `.tscn` — mesmo motivo de sempre (`InventoryUI`), mas essa é a primeira
  vez que isso envolve tipos de nó nunca usados neste projeto
  (`Label`, `CpuParticles2D`, `Tween`). Como nenhum deles precisa de
  `ext_resource`/`sub_resource` (só propriedades simples em código),
  `GroundItem.tscn` em si não mudou — o `load_steps` continua o mesmo de
  antes desta etapa.
- **Três chamadas de agente de pesquisa (WebSearch/WebFetch) feitas antes
  de escrever qualquer código desta leva**, seguindo a mesma cautela já
  usada para `Godot.FileAccess`/`Callable.From` nas etapas anteriores:
  (1) confirmei que a classe C# é `CpuParticles2D` (não `CPUParticles2D`
  como em GDScript) e que suas propriedades (`Lifetime`, `Explosiveness`,
  `Spread`, `ScaleAmountMin/Max`) são `double` em C#, não `float`; (2)
  confirmei a API de `Tween` em C# (`CreateTween()`,
  `TweenProperty(GodotObject, NodePath, Variant, double)`, `SetLoops()`
  sem argumento = infinito) e que um `Label` (Control) como filho de um
  `Area2D`/`Node2D` segue a transformada 2D do pai normalmente — padrão
  legítimo, não algo que quebra silenciosamente; (3) confirmei que
  `Tween` tem `BindNode(Node)` e que **é necessário** aqui: sem isso, um
  Tween que anima um nó filho que depois é `QueueFree()`'d (como o
  `glow` quando o item é coletado) continuaria tentando escrever numa
  referência liberada e geraria erro em runtime a cada frame, em vez de
  simplesmente parar sozinho.
- **`LootPresentation` é código, não JSON**, diferente de items/affixes
  — só 4 raridades fixas, valores de direção de arte (mais perto dos
  caps hardcoded do `AffixRoller` do que de conteúdo real que alguém
  precisa adicionar em runtime). A Etapa 12 é que vira a camada
  data-driven de verdade.
- **Loot Filter: primeira regra que bate vence** (convenção
  FilterBlade/ARPG real), com fallback = exatamente o default da Etapa
  11 quando nenhuma regra bate. Cores em JSON são `{r,g,b,a}` (doubles),
  não string hex — evita depender de uma API de parsing de `Color`
  nunca exercitada neste projeto, quando o construtor de 4 floats já é
  comprovadamente seguro (usado em todo `.tscn` existente).
- **Condições e ações do Loot Filter deliberadamente incompletas vs. a
  lista do MASTER_HANDOFF**, documentado como tal em vez de esquecido:
  sem base type/class/slot/affixes/tiers/sockets (sistemas que não
  existem ainda ou pediriam uma mini-linguagem de query própria) e sem
  Sound/MinimapIcon (sem assets de áudio, sem sistema de minimapa).
- **"Hide" no Loot Filter = invisível, mas continua coletável**:
  `GroundItem.Visible = result.Show`, e física de `Area2D` em Godot é
  independente de `CanvasItem.Visible` — esconder não desliga a
  colisão. Escolha de design documentada, não a única possível (a
  alternativa seria não spawnar o item, mas isso mudaria
  `LootGenerator`, e não foi pedido).
- **`Stash` mora como componente-filho do Player** (mesmo padrão de
  composição de `Inventory`/`Equipment`), **deliberadamente provisório e
  documentado como tal**: um Stash de verdade pertence à conta/save do
  jogador, não ao personagem, mas não existe base/vila nem persistência
  (Etapa 22) ainda — não há outro lugar sensato pra guardar esse estado
  agora. A API pública não assume nada sobre onde isso mora, então mover
  depois é realocação, não reescrita.
- **Categoria do Stash resolvida automaticamente por dados do item**
  (`Rarity == Unique` → aba Unique mesmo se equipável; `IsEquipable` →
  Equipment; `IsCraftingCurrency` → Currency; resto → Geral) — mesma
  regra central de sempre, nunca checagem por id de item.
- **Tecla nova `stash` (T), independente de `inventory` (I)**: os dois
  painéis (`InventoryUI`/`StashUI`) podem ficar abertos ao mesmo tempo,
  já que usam ações de input e posições de painel diferentes (Inventário
  em x=20, Baú em x=380).
- **Som continua de fora, em ambas as etapas novas**, documentado como
  não-objetivo e não como pendência silenciosa: não existe nenhum asset
  de áudio neste repositório, e não haveria como importar/validar um sem
  o editor Godot aberto nesta sessão.

### Arquivos alterados/criados

Novos: `Game/Loot/{LootPresentation,LootFilterRule,LootFilterColor,
LootFilterResult,LootFilterDatabase}.cs`, `Data/Loot/loot_filter.json`,
`Game/Inventory/{Stash,StashCategory}.cs`, `UI/{StashUI.cs,StashUI.tscn}`.
Modificados: `Game/Loot/GroundItem.cs` (reescrito: `ApplyPresentation()` +
5 métodos `Build*`, chamados em `_Ready()`), `Game/Core/InputBootstrap.cs`
(ação `stash` = tecla T), `Game/Actors/Player/Player.tscn` (nó `Stash`),
`Scenes/Main.tscn` (instância de `StashUI`), além de `ARCHITECTURE.md`,
`ROADMAP.md`, `CURRENT_STATUS.md`.

### Testes executados e resultados

- ✅ Verificado nesta sessão (vídeo, ver "O que foi feito" acima): Etapa
  8+9 completa (compilou com 0 erros, equipar mostra raridade+afixo
  corretos no slot certo), e o bug de raridade em currency da Entrada 7
  se confirmou corrigido de verdade.
- ✅ Verificado nesta sessão: `Data/Items/items.json` (10), `affixes.json`
  (8) e o novo `Data/Loot/loot_filter.json` (4 regras) são JSON válido
  (`python3 json.load`, sem erro).
- ✅ Verificado nesta sessão: recontagem de `load_steps` em todos os
  `.tscn` do projeto, notavelmente `Player.tscn` 14/14 após adicionar o
  nó `Stash` (era 13/13) e `Scenes/Main.tscn` 4/4 após adicionar
  `StashUI` (era 3/3); `GroundItem.tscn` continua 3/3 (a presentation
  nova é só código, sem `ext_resource`/`sub_resource` novo); nenhuma
  regressão nos demais.
- ✅ Verificado nesta sessão: balanceamento de chaves/parênteses/
  colchetes em todos os `.cs` do projeto (incluindo os novos arquivos de
  Loot/Stash) — todos batendo.
- ✅ Verificado nesta sessão via busca (3 chamadas de agente,
  documentadas em "Como funciona" acima): API real em C# de
  `CpuParticles2D`, `Tween`/`BindNode`, `Label` como filho de `Node2D`, e
  `Polygon2D.Polygon`/`CpuParticles2D.Direction`/`.Gravity` como
  `Vector2[]`/`Vector2` — tudo confirmado contra a documentação oficial
  do Godot 4.x antes de escrever o código correspondente.
- ❌ **NÃO testado**: compilação real do C# desta etapa (sem SDK
  .NET/Godot neste ambiente — mesma limitação de sempre). Esta é a etapa
  com mais superfície de API nova nunca exercitada por um compilador real
  neste projeto especificamente. ❌ **NÃO testado**: comportamento em
  runtime (visual de cada raridade no chão, filtro escondendo Normal mas
  ainda coletável, painel do Baú abrindo/guardando/retirando). Ver
  CURRENT_STATUS.md "Known Issues" para o checklist exato. ❌ **NÃO
  testado** (herdado da Etapa 10, ainda pendente): aplicar um crafting
  effect de verdade clicando "[usar]".

### Pendências

- Testar Etapa 11+12+13 localmente no Godot (checklist em
  CURRENT_STATUS.md) — com atenção redobrada ao build (`Tween`/
  `CpuParticles2D`/`Label`-em-`Area2D` nunca compilados de verdade antes
  desta sessão).
- Push local da branch
  `feature/etapa-11-12-13-loot-presentation-filter-stash` (comandos no
  final desta entrada) e abertura de PR.
- Aplicar um crafting effect de verdade (botão "[usar]", Etapa 10) segue
  sem confirmação visual — não bloqueia Etapa 11+12+13, mas continua
  pendente.
- Dano ao player pelo inimigo (Etapa 3, dodge/block) segue sem
  confirmação visual — mesma pendência de sempre, não bloqueante.
- Decisão pendente do usuário sobre a branch `docs/handoff` do Grok — não
  decidir/mesclar/apagar nada dela unilateralmente.

### Erros conhecidos

Nenhum novo confirmado nesta etapa (nada pôde ser executado pra confirmar
um erro real). Ver "Testes executados" acima para o que fica como risco
não verificado — em especial a superfície de API nova desta leva
(`Tween`/`CpuParticles2D`/`Label`-em-`Area2D`), verificada por busca mas
nunca por compilador real. Erros de sessões anteriores seguem
documentados nas Entradas 1-7, sem impacto no código atual (o bug de
raridade em currency da Entrada 7 se confirmou corrigido no vídeo desta
entrada).

### Branch / commit

Trabalho feito em cima da ponta de
`feature/etapa-8-9-10-equipamento-afixos-crafting` (commit `34adb06`).
Branch dedicada
**`feature/etapa-11-12-13-loot-presentation-filter-stash`** criada nesta
sessão a partir desse commit, com o commit da Etapa 11+12+13 nela — ver
hash exato no `git log` local. Este ambiente de nuvem **não consegue
fazer `git push`** (limitação de plataforma documentada desde a Entrada
2) — arquivos copiados para a pasta local do usuário
(`C:\Dev\mystery-absolut`) via mirror de arquivos; o usuário faz o
commit/push locais.

Comandos passados ao usuário (PowerShell, a partir de
`C:\Dev\mystery-absolut`):

```powershell
git checkout feature/etapa-8-9-10-equipamento-afixos-crafting
git pull origin feature/etapa-8-9-10-equipamento-afixos-crafting
git checkout -b feature/etapa-11-12-13-loot-presentation-filter-stash
git add Game/Loot Game/Inventory UI/StashUI.cs UI/StashUI.tscn Data/Loot Game/Core/InputBootstrap.cs Game/Actors/Player/Player.tscn Scenes/Main.tscn ARCHITECTURE.md ROADMAP.md CURRENT_STATUS.md HANDOFF.md
git commit -m "Etapa 11 + Etapa 12 + Etapa 13: loot presentation, loot filter e stash"
git push -u origin feature/etapa-11-12-13-loot-presentation-filter-stash
```

Depois, abrir PR com base em
`feature/etapa-8-9-10-equipamento-afixos-crafting` (trocar para `main` se
essa branch já tiver sido mergeada).

### Prompt pronto para a próxima IA

```
Leia HANDOFF.md (Entrada 8, a mais recente), CURRENT_STATUS.md e
ARCHITECTURE.md antes de fazer qualquer coisa. Etapas 0 a 9 estão
implementadas e verificadas de verdade pelo usuário (vídeo). Etapa 10
(Crafting) compila e os itens aparecem certos no inventário, mas nenhum
efeito foi aplicado de verdade ainda (botão "[usar]" nunca clicado) —
isso é uma pendência real, não hipotética. Etapa 11 (Loot Presentation),
Etapa 12 (Loot Filter) e Etapa 13 (Stash) estão implementadas nesta sessão
mas AINDA NÃO testadas no Godot — esta é a etapa com mais risco de
compilação até agora, porque usa Tween/CreateTween/BindNode,
CpuParticles2D e um Label como filho de um Area2D pela primeira vez no
projeto (tudo verificado via busca na documentação oficial antes de
escrever, não por experiência prévia real neste código). Se você tiver
acesso a Godot/dotnet, o primeiro passo é simplesmente abrir o projeto e
ver se compila — se não compilar, corrija antes de qualquer outra coisa e
documente o que estava errado. Depois, siga o checklist de teste manual
em CURRENT_STATUS.md (aparência de cada raridade no chão, filtro
escondendo item Normal mas ele continuar coletável, painel do Baú com T
guardando/retirando itens) — e aproveite pra também testar o crafting de
verdade (clicar "[usar]" numa currency) e o dano ao player pelo inimigo
(dodge/block), as duas pendências mais antigas do projeto que ainda não
têm confirmação visual. NÃO inicie a Etapa 14 (Primeiro Companion) nem
nenhuma etapa futura sem autorização explícita e literal do usuário.
Mantenha HANDOFF.md atualizado a cada parada. Existe uma branch
docs/handoff no repositório remoto escrita por outra IA (Grok) a partir
de uma visão desatualizada do projeto — não mesclar/apagar sem o usuário
decidir.
```

---

## Entrada 7 — 2026-09-15 — Claude (Sonnet 5, sessão claude.ai/code)

### O que foi feito

1. **Verificação real da Etapa 6+7**: o usuário gravou um quarto vídeo
   (~51s) mostrando o jogo rodando localmente. Extraí frames (`ffmpeg -vf
   fps=2`, 102 frames) e inspecionei visualmente dezenas deles (com crops
   2x nos trechos de UI). **Confirmado de verdade**: o projeto compilou
   (o build do C#/JSON/FileAccess da Etapa 6, maior risco não verificado
   até então, funcionou), o painel de inventário abre com `I`, itens
   coletados aparecem na lista com nome/raridade/stack corretos. **Bug
   real encontrado no vídeo**: um item stacável de currency apareceu como
   "Fragmento de Poeira Antiga [Magic] x2" — currency não deveria ter
   raridade. Root cause: `LootGenerator.RollItemInstance()` chamava
   `RollRarity()` incondicionalmente pra qualquer base sorteada,
   inclusive `Stackable`. **Corrigido** nesta sessão (ver "Erros
   conhecidos" abaixo).
2. **Etapa 8 (Equipment)**, **Etapa 9 (Affixes)** e **Etapa 10
   (Crafting)** implementadas, autorizadas explicitamente pelo usuário
   ("pode dar inicio as etapas 8, 9 e 10"), condicionado à Etapa 6+7
   estar ok — e está, confirmada pelo vídeo acima.

### Como funciona

Detalhado em ARCHITECTURE.md (seções "Equipamento — Etapa 8", "Afixos —
Etapa 9", "Crafting — Etapa 10"). Decisões não-óbvias mais importantes
(todas também comentadas no código):

- **Dois enums para slot de equipamento** (`EquipmentSlot` com 9 valores
  concretos vs `EquipmentSlotCategory` com 8 categorias): Ring1 e Ring2
  são dois slots concretos mas uma única categoria "Ring" no item — sem
  essa separação, um item de anel precisaria declarar qual dos dois slots
  prefere, o que não faz sentido. `Equipment.ResolveSlot` escolhe o
  primeiro slot de anel vazio, ou substitui Ring1 se os dois estiverem
  ocupados (determinístico).
- **`Equip`/`Unequip` são tudo-ou-nada com swap-back seguro**: ao equipar
  algo num slot ocupado, o item anterior tenta voltar pro inventário
  primeiro; se não houver espaço, a operação inteira é abortada (nada
  muda, nenhum item é perdido).
- **Equipment não altera stats de combate** — deliberado, documentado no
  próprio arquivo. Nenhuma etapa até agora pediu um sistema de "stats
  efetivos a partir de equipamento + afixos"; isso fica pra quando (se)
  for pedido explicitamente.
- **`AffixRoller` centraliza sorteio/cap/pool** num único lugar, reusado
  tanto por `LootGenerator` (drop inicial) quanto pelos efeitos de
  crafting (`Game.Crafting`) — evita duplicar a lógica de cap-por-raridade
  em dois lugares que divergiriam com o tempo.
- **Cap de afixos simplificado e documentado como placeholder**: Normal=0,
  Magic=2 total (1 roll natural no drop), Rare=4 total (2 rolls
  naturais), sempre dividido meio-a-meio entre Prefix/Suffix por
  raridade. Isso NÃO é "1 prefixo + 1 sufixo garantido" — é "até N total,
  com cap por tipo", porque qual(is) tipo(s) rola(m) primeiro depende de
  qual pool ainda tem espaço. MASTER_HANDOFF seção 18-24 pede só
  "Prefix/Suffix, tiers, pools, pesos" — esta é a versão mínima disso,
  não o balanceamento final.
- **Crafting ligado a itens via dado, não hardcode**:
  `ItemBaseDefinition.CraftingEffectId` (string) + `CraftingEffectRegistry`
  (único Dictionary<string, ICraftingEffect> do projeto) — exatamente o
  padrão "dados criam conteúdo" já usado pra items/affixes. `4` efeitos
  concretos com os nomes exatos sugeridos no MASTER_HANDOFF.
  `CraftingService.TryApply` é puro: só muta o `ItemInstance`, não
  remove a currency do inventário — isso é responsabilidade de quem
  chama (a UI), porque `CraftingService` não deveria precisar saber que
  `Inventory` existe.
- **UI de crafting é um placeholder de debug, documentado como tal**:
  clicar numa currency na lista de inventário aplica automaticamente no
  primeiro item elegível do inventário (`FirstOrDefault` via LINQ) — não
  existe seleção real de alvo porque nenhuma etapa pediu uma UI de
  crafting de verdade ainda. Ficou explícito no código e aqui pra não
  passar como "sistema de crafting completo".
- **Evitei sintaxe C# mais nova não exercitada ainda neste projeto**,
  mesmo tendo certeza de que compilaria, porque não há compilador
  disponível nesta sessão pra confirmar: troquei o pattern-combinator
  `x is A or B` por `==`/`||` em `AddModifierEffect.CanApply`, e troquei
  `switch` expression por `switch` statement clássico em
  `Equipment.ResolveSlot` e nos dois métodos de `AffixRoller`
  (`MaxAffixesFor`/`NaturalRollCountFor`). Prefiro código um pouco mais
  verboso a introduzir a primeira ocorrência de uma sintaxe nunca testada
  neste ambiente.
- **`ItemDatabase` ganhou `JsonStringEnumConverter`**: necessário porque
  `equipSlotCategory` no JSON agora é uma string (`"Weapon"`,
  `"Ring"`, etc.) que precisa virar o enum `EquipmentSlotCategory`.

### Arquivos alterados/criados

Novos: `Game/Items/{EquipmentSlot,EquipmentSlotCategory,AffixType,
AffixDefinition,AffixInstance,AffixDatabase,AffixRoller}.cs`,
`Game/Inventory/Equipment.cs`, `Game/Crafting/{ICraftingEffect,
MakeMagicEffect,RerollMagicModifiersEffect,AddModifierEffect,
UpgradeToRareEffect,CraftingEffectRegistry,CraftingService}.cs`,
`Data/Items/affixes.json`.
Modificados: `Game/Items/ItemBaseDefinition.cs` (`EquipSlotCategory`,
`CraftingEffectId`, `IsEquipable`, `IsCraftingCurrency`),
`Game/Items/ItemInstance.cs` (`List<AffixInstance> Affixes`),
`Game/Items/ItemDatabase.cs` (`JsonStringEnumConverter`),
`Data/Items/items.json` (reescrito, 10 itens), `Game/Loot/LootGenerator.cs`
(fix do bug de raridade em currency + `AffixRoller.RollInitialAffixes`),
`Game/Actors/Player/Player.tscn` (nó `Equipment`), `UI/InventoryUI.cs`
(reescrito: lista de equipados, botões equipar/desequipar/usar-currency,
exibição de afixos), além de `ARCHITECTURE.md`, `ROADMAP.md`,
`CURRENT_STATUS.md`.

### Testes executados e resultados

- ✅ Verificado nesta sessão (vídeo, ver "O que foi feito" acima): Etapa
  6+7 completa (compilou, JSON carregou, painel abre/fecha, itens
  corretos), e o bug real de raridade em currency foi observado e depois
  corrigido no código.
- ✅ Verificado nesta sessão: `Data/Items/items.json` (10 entradas) e
  `Data/Items/affixes.json` (8 entradas) são JSON válido (`python3
  json.load`, sem erro).
- ✅ Verificado nesta sessão: recontagem de `load_steps` em todos os
  `.tscn` do projeto, notavelmente `Player.tscn` 13/13 após adicionar o
  nó `Equipment` (era 12/12 antes); nenhuma regressão nos demais.
- ✅ Verificado nesta sessão: balanceamento de chaves/parênteses em todos
  os `.cs` do projeto, incluindo re-checagem explícita de
  `AffixRoller.cs`, `Equipment.cs` e `AddModifierEffect.cs` depois das
  reescritas de sintaxe (de-risking) — todos batendo.
- ❌ **NÃO testado**: compilação real do C# desta etapa (sem SDK
  .NET/Godot neste ambiente — mesma limitação de sempre). Esta etapa usa
  `JsonStringEnumConverter` e LINQ chains (`FirstOrDefault`, `Select`,
  `Where`, `Sum`, `All`) pela primeira vez no projeto; nada disso foi
  exercitado por um compilador real ainda. ❌ **NÃO testado**:
  comportamento em runtime (equipar/desequipar pela UI, ver os 9 slots,
  usar as 4 currencies e ver o efeito de cada uma, ver afixos aparecerem
  em itens Magic/Rare). Ver CURRENT_STATUS.md "Known Issues" para o
  checklist exato.

### Pendências

- Testar Etapa 8+9+10 localmente no Godot (checklist em
  CURRENT_STATUS.md) — com atenção redobrada ao build
  (`JsonStringEnumConverter`/LINQ chains nunca compiladas de verdade
  antes desta sessão).
- Push local da branch `feature/etapa-8-9-10-equipamento-afixos-crafting`
  (comandos no final desta entrada) e abertura de PR.
- Dano ao player pelo inimigo (Etapa 3, dodge/block) segue sem
  confirmação visual — não bloqueia Etapa 8+9+10, mas continua pendente.
- Decisão pendente do usuário sobre a branch `docs/handoff` do Grok — não
  decidir/mesclar/apagar nada dela unilateralmente.

### Erros conhecidos

- **Corrigido nesta sessão**: `LootGenerator.RollItemInstance()` sorteava
  `ItemRarity` pra qualquer base, inclusive `Stackable` (currency) —
  causava coisas como "Fragmento de Poeira Antiga [Magic] x2", observado
  de verdade no vídeo da Etapa 6+7. Fix: `chosen.Stackable ?
  ItemRarity.Normal : RollRarity()`. Nenhum outro erro novo confirmado
  nesta etapa (o resto não pôde ser executado pra confirmar um erro
  real) — ver "Testes executados" acima para os riscos não verificados.

### Branch / commit

Trabalho feito em cima da ponta de `feature/etapa-6-7-itens-inventario`
(commit `18fcb0e`). Branch dedicada
**`feature/etapa-8-9-10-equipamento-afixos-crafting`** criada nesta
sessão a partir desse commit, com o commit da Etapa 8+9+10 nela — ver
hash exato no `git log` local. Este ambiente de nuvem **não consegue
fazer `git push`** (limitação de plataforma documentada desde a Entrada
2) — arquivos copiados para a pasta local do usuário
(`C:\Dev\mystery-absolut`) via mirror de arquivos; o usuário faz o
commit/push locais.

Comandos passados ao usuário (PowerShell, a partir de
`C:\Dev\mystery-absolut`):

```powershell
git checkout feature/etapa-6-7-itens-inventario
git pull origin feature/etapa-6-7-itens-inventario
git checkout -b feature/etapa-8-9-10-equipamento-afixos-crafting
git add Game/Items Game/Inventory Game/Crafting Data/Items Game/Loot/LootGenerator.cs Game/Actors/Player/Player.tscn UI/InventoryUI.cs ARCHITECTURE.md ROADMAP.md CURRENT_STATUS.md HANDOFF.md
git commit -m "Etapa 8 + Etapa 9 + Etapa 10: equipamento, afixos e crafting"
git push -u origin feature/etapa-8-9-10-equipamento-afixos-crafting
```

Depois, abrir PR com base em `feature/etapa-6-7-itens-inventario` (trocar
para `main` se essa branch já tiver sido mergeada).

### Prompt pronto para a próxima IA

```
Leia HANDOFF.md (Entrada 7, a mais recente), CURRENT_STATUS.md e
ARCHITECTURE.md antes de fazer qualquer coisa. Etapas 0 a 7 estão
implementadas e verificadas de verdade pelo usuário (vídeo). Etapa 8
(Equipment), Etapa 9 (Affixes) e Etapa 10 (Crafting) estão implementadas
nesta sessão mas AINDA NÃO testadas no Godot — este é o ponto de maior
risco até agora, porque é a primeira vez que o projeto usa
JsonStringEnumConverter e várias LINQ chains, e nenhuma sessão anterior
teve acesso a um SDK .NET real pra compilar isso. Se você tiver acesso a
Godot/dotnet, o primeiro passo é simplesmente abrir o projeto e ver se
compila — se não compilar, corrija antes de qualquer outra coisa e
documente o que estava errado. Um bug real já foi encontrado e corrigido
nesta sessão (currency sorteando raridade Magic/Rare) a partir de um
vídeo do usuário — isso reforça que "parece razoável" não é o mesmo que
"testado"; confira o comportamento em runtime item por item (checklist em
CURRENT_STATUS.md), não presuma que está certo só porque compilou. NÃO
inicie a Etapa 11 (Loot Presentation) nem nenhuma etapa futura sem
autorização explícita e literal do usuário. Mantenha HANDOFF.md
atualizado a cada parada. Existe uma branch docs/handoff no repositório
remoto escrita por outra IA (Grok) a partir de uma visão desatualizada do
projeto — não mesclar/apagar sem o usuário decidir.
```

---

## Entrada 6 — 2026-09-15 — Claude (Sonnet 5, sessão claude.ai/code)

### O que foi feito

1. **Verificação real da Etapa 4+5**: o usuário gravou um terceiro vídeo
   (OBS, ~53s) mostrando o jogo rodando localmente. Extraí frames
   (`ffmpeg -vf fps=2`, 105 frames) e inspecionei visualmente dezenas
   deles (incluindo crops 2x para ver os quadrados pequenos com clareza).
   **Confirmado de verdade**: o inimigo (laranja) fica parado até o
   player se aproximar, persegue (Chase), some/fica cinza-escuro ao
   morrer (mesmo comportamento do `Dead` — sem `QueueFree`, o corpo fica
   parado no lugar pra sempre, exatamente como `EnemyController.OnDied()`
   faz), um quadrado amarelo (`GroundItem`) aparece bem onde o inimigo
   morreu, e some depois que o player anda sobre ele — console mostra
   "Loot coletado." exatamente nesse ponto. Também confirmado: múltiplos
   ataques no Dummy reduzindo HP progressivamente (mensagens "Dummy took
   X damage, Y/100 HP remaining" batendo com os valores esperados,
   inclusive o Dummy ficando visualmente mais escuro/translúcido a cada
   hit). **Não confirmado neste vídeo**: dano no player vindo do inimigo
   (dodge/block reduzindo dano) — o inimigo morreu rápido demais nas
   trocas de golpe pra render um hit nele mesmo, e não existe feedback
   visual no player quando ele toma dano (só Dummy/Enemy têm esse
   feedback), então mesmo que tivesse acontecido não daria pra confirmar
   só pelo vídeo. Isso segue como pendência explícita, não como "provavelmente
   funciona".
2. **Etapa 6 (Item System)** e **Etapa 7 (Inventory)** implementadas,
   autorizadas explicitamente pelo usuário ("pode dar continuidade na
   etapa 6 e 7"), condicionado à Etapa 4+5 estar ok — e está, com a
   ressalva do item acima.

### Como funciona

Detalhado em ARCHITECTURE.md (seções "Sistema de itens — Etapa 6" e
"Inventário — Etapa 7"). Decisões não-óbvias mais importantes (todas
também comentadas no código):

- **JSON em vez de Resource/`.tres`** para `ItemBaseDefinition`: mesma
  cautela de sempre (DamageInfo, DropTable) contra escrever serialização
  Godot à mão sem o editor pra validar — mas isso é a primeira vez que o
  projeto usa `System.Text.Json` + `Godot.FileAccess` juntos, e **nada
  disso pôde ser compilado/rodado nesta sessão** (sem SDK .NET aqui).
  Verifiquei manualmente: `Godot.FileAccess` implementa `IDisposable`
  (confirmado via busca — `public class FileAccess : RefCounted,
  IDisposable`), então `using var file = Godot.FileAccess.Open(...)`
  compila; JSON parseado com `PropertyNameCaseInsensitive = true` pra
  casar `"maxStackSize"` (JSON) com `MaxStackSize` (C#) sem precisar de
  atributos `[JsonPropertyName]` em cada campo. Isso é o ponto de maior
  risco real desta etapa — ver CURRENT_STATUS.md item 1.
- **`ItemDatabase` é singleton estático simples, não autoload**: nada
  nele precisa de `_Ready()` ou da árvore de cena, então um Node só pra
  isso seria peso desnecessário (e mais uma entrada em
  `project.godot [autoload]` pra manter).
- **Sem campo de slot de equipamento em `ItemBaseDefinition`**: isso é
  Etapa 8. `Tags` já é suficiente pra descrever o item por enquanto.
- **Unique excluída do sorteio de raridade do `LootGenerator`**: um
  Unique de verdade é um item específico com stats fixos definidos à
  mão (sistema que não existe ainda), não "base genérica + tag Unique" —
  então o roll fica só entre Normal/Magic/Rare (70/25/5%) até esse
  sistema existir.
- **`ItemLevel` fixo em 1** em todo item dropado: não existe sistema de
  nível de inimigo/área. O campo existe no modelo porque a Etapa 6 pediu
  explicitamente, mas nada ainda lê esse valor — só passa a importar na
  Etapa 9 (Affixes).
- **`Inventory.AddItem` é tudo-ou-nada**: não faz split parcial de uma
  pickup entre duas stacks quando não cabe inteira numa só. Mantém a
  lógica simples; nada dropa quantidade grande o bastante ainda pra isso
  importar na prática (`LootGenerator` sorteia 1-3 unidades por pickup
  stacável).
- **`InventoryUI` construída 100% em código** (`new Panel {...}`, etc.),
  não como nós `Control` escritos à mão no `.tscn` — mesmo motivo de
  sempre: anchors/margins precisos em `.tscn` cru não dá pra validar sem
  o editor aberto.
- **Ação `inventory` (tecla `I`) registrada agora** em
  `InputBootstrap.cs` — estava só reservada/comentada desde a Etapa 1.
- **Namespace `Game.UI` para uma pasta `UI/` que não fica dentro de
  `Game/`**: inconsistência deliberada, documentada em ARCHITECTURE.md —
  a pasta `UI/` já existia na raiz desde a Etapa 0 (antes de qualquer
  convenção de namespace existir), então o namespace segue a regra geral
  (`Game.<Área>`) mesmo sem o caminho físico bater 1:1.

### Arquivos alterados/criados

Novos: `Game/Items/{ItemRarity,ItemBaseDefinition,ItemInstance,ItemDatabase}.cs`,
`Data/Items/items.json`, `Game/Inventory/Inventory.cs`,
`UI/{InventoryUI.cs,InventoryUI.tscn}`.
Modificados: `Game/Loot/GroundItem.cs` (propriedade `Payload`, entrega ao
`Inventory`), `Game/Loot/LootGenerator.cs` (sorteia item+raridade real),
`Game/Actors/Player/Player.tscn` (nó `Inventory`), `Game/Core/InputBootstrap.cs`
(ação `inventory`), `Scenes/Main.tscn` (instância de `InventoryUI`), além
de `ARCHITECTURE.md`, `ROADMAP.md`, `CURRENT_STATUS.md`.

### Testes executados e resultados

- ✅ Verificado nesta sessão (vídeo, ver "O que foi feito" acima): Etapa
  4+5 completa (IA idle/chase, morte, drop, pickup, múltiplos ataques com
  cooldown funcionando).
- ✅ Verificado nesta sessão: `Data/Items/items.json` é JSON válido
  (`python3 -m json` equivalente — parseado sem erro, 5 entradas, todos
  os ids presentes).
- ✅ Verificado nesta sessão: balanceamento de chaves/parênteses em todos
  os `.cs` novos/alterados desta etapa (`ItemDatabase.cs` 17/17,
  `ItemBaseDefinition.cs` 7/7, `ItemInstance.cs` 7/7, `ItemRarity.cs`
  1/1, `Inventory.cs` 13/13, `InventoryUI.cs` 23/23, `GroundItem.cs`
  11/11, `LootGenerator.cs` 11/11) e nos arquivos já existentes do
  projeto inteiro (nenhuma regressão).
- ✅ Verificado nesta sessão: recontagem de `load_steps` em todos os
  `.tscn` do projeto (`Player.tscn` 12/12 após adicionar o nó Inventory,
  `Main.tscn` 3/3 após adicionar InventoryUI, `InventoryUI.tscn` 2/2,
  mais os já existentes sem regressão).
- ✅ Verificado nesta sessão via busca: `Godot.FileAccess` implementa
  `IDisposable` (então `using var file = ...` compila) e
  `Callable.From(() => Metodo()).CallDeferred()` é o padrão correto/seguro
  documentado pra chamar um método C# adiado (evitei a forma
  `CallDeferred(nome_string, args)` porque essa depende de marshalling de
  `Variant` que pode falhar silenciosamente para tipos C# não triviais).
- ❌ **NÃO testado**: compilação real do C# com `System.Text.Json` +
  `Godot.FileAccess` (sem SDK .NET/Godot neste ambiente — este é o maior
  risco desta etapa especificamente, mais que as anteriores, por ser
  código novo nunca exercitado pelo compilador de verdade). ❌ **NÃO
  testado**: comportamento em runtime (abrir/fechar inventário, coletar
  item real, ver ele na lista, stacking de itens stacáveis). Ver
  CURRENT_STATUS.md "Known Issues" para o checklist exato.

### Pendências

- Testar Etapa 6+7 localmente no Godot (checklist em CURRENT_STATUS.md) —
  com atenção redobrada ao build (`System.Text.Json`/`FileAccess` nunca
  compilados de verdade antes desta sessão).
- Push local da branch `feature/etapa-6-7-itens-inventario` (comandos no
  final desta entrada) e abertura de PR.
- Dano ao player pelo inimigo (Etapa 3, dodge/block) segue sem
  confirmação visual — não bloqueia Etapa 6+7, mas continua pendente.
- Decisão pendente do usuário sobre a branch `docs/handoff` do Grok — não
  decidir/mesclar/apagar nada dela unilateralmente.

### Erros conhecidos

Nenhum novo confirmado nesta etapa (nada pôde ser executado pra confirmar
um erro real). Ver "Testes executados" acima para o que fica como risco
não verificado. Erros de sessões anteriores seguem documentados nas
Entradas 1-5, sem impacto no código atual.

### Branch / commit

Trabalho feito em cima da ponta de `feature/etapa-4-5-inimigo-e-loot`
(commit `2294d49`). Branch dedicada **`feature/etapa-6-7-itens-inventario`**
criada nesta sessão a partir desse commit, com o commit da Etapa 6+7 nela
— ver hash exato no `git log` local. Este ambiente de nuvem **não
consegue fazer `git push`** (limitação de plataforma documentada desde a
Entrada 2) — arquivos copiados para a pasta local do usuário
(`C:\Dev\mystery-absolut`) via mirror de arquivos; o usuário faz o
commit/push locais.

Comandos passados ao usuário (PowerShell, a partir de
`C:\Dev\mystery-absolut`):

```powershell
git checkout feature/etapa-4-5-inimigo-e-loot
git pull origin feature/etapa-4-5-inimigo-e-loot
git checkout -b feature/etapa-6-7-itens-inventario
git add Game/Items Game/Inventory Data/Items UI/InventoryUI.cs UI/InventoryUI.tscn Game/Loot/GroundItem.cs Game/Loot/LootGenerator.cs Game/Actors/Player/Player.tscn Game/Core/InputBootstrap.cs Scenes/Main.tscn ARCHITECTURE.md ROADMAP.md CURRENT_STATUS.md HANDOFF.md
git commit -m "Etapa 6 + Etapa 7: sistema de itens e inventario basico"
git push -u origin feature/etapa-6-7-itens-inventario
```

Depois, abrir PR com base em `feature/etapa-4-5-inimigo-e-loot` (trocar
para `main` se essa branch já tiver sido mergeada).

### Prompt pronto para a próxima IA

```
Leia HANDOFF.md (Entrada 6, a mais recente), CURRENT_STATUS.md e
ARCHITECTURE.md antes de fazer qualquer coisa. Etapas 0 a 5 estão
implementadas e verificadas de verdade pelo usuário (vídeo). Etapa 6
(Item System) e Etapa 7 (Inventory) estão implementadas nesta sessão mas
AINDA NÃO testadas no Godot — este é o ponto de maior risco até agora,
porque é a primeira vez que o projeto usa System.Text.Json +
Godot.FileAccess, e nenhuma sessão anterior teve acesso a um SDK .NET
real pra compilar isso. Se você tiver acesso a Godot/dotnet, o primeiro
passo é simplesmente abrir o projeto e ver se compila — se não compilar,
corrija antes de qualquer outra coisa e documente o que estava errado
(não presuma que o parsing JSON ou o FileAccess.Open estão corretos só
porque o código parece razoável). NÃO inicie a Etapa 8 (Equipment) nem
nenhuma etapa futura sem autorização explícita e literal do usuário.
Mantenha HANDOFF.md atualizado a cada parada. Existe uma branch
docs/handoff no repositório remoto escrita por outra IA (Grok) a partir
de uma visão desatualizada do projeto — não mesclar/apagar sem o usuário
decidir.
```

---

## Entrada 5 — 2026-09-15 — Claude (Sonnet 5, sessão claude.ai/code)

### O que foi feito

1. **Verificação real da Etapa 2+3**: o usuário gravou um segundo vídeo
   mostrando o jogo rodando no Godot local com o combate. Conferido nesta
   sessão (mesmo processo da Entrada 4: extração de frames, inspeção
   visual). Etapa 2+3 passou de "implementada, não verificada" para
   **verificada de verdade** — ROADMAP.md/CURRENT_STATUS.md atualizados.
2. **Etapa 4 (Primeiro inimigo)** e **Etapa 5 (Loot básico)**
   implementadas, autorizadas explicitamente pelo usuário ("pode dar
   continuidade na etapa 4 e 5"), condicionado ao vídeo da Etapa 2+3
   estar correto — e estava.
3. Resposta à pergunta do usuário sobre braço/perna visíveis nos
   personagens futuros: ver "Nota de design" abaixo.

### Como funciona

Detalhado em ARCHITECTURE.md (seções "Inimigos e IA — Etapa 4" e "Loot
básico — Etapa 5"). Resumo das decisões não-óbvias (todas também
comentadas no próprio código):

- `EnemyController` é uma state machine simples (`enum` + `switch` dentro
  de `_PhysicsProcess`, sem framework) com 5 estados: Idle → Chase (ao
  detectar o player em 180px) → Attack (ao alcançar 34px, ativa o próprio
  `AttackHitbox` reaproveitando `Hitbox.cs` sem nenhuma mudança nele) →
  Recover (0.5s) → volta a Chase ou Idle. `Chase → Idle` também acontece
  se o player sair de `LeashRange` (260px), pra não perseguir pra sempre.
  Deliberadamente sem Patrol/Flee/Summon/fases — MASTER_HANDOFF seção 47
  ("começar simples"); só se justifica um framework de verdade quando
  houver um segundo tipo de inimigo que precise de algo mais.
- O inimigo reaproveita **exatamente** os mesmos componentes de combate do
  player (`Health`, `Combatant`, `Hurtbox`, `Hitbox`) sem alterar nenhum
  desses arquivos — é o retorno de ter desacoplado `Game.Combat` de
  `PlayerController` desde a Etapa 2.
- **Isto também é o primeiro teste real possível de dodge/block reduzindo
  dano do player** (Etapa 3): até agora só o Dummy recebia dano, nada
  atacava o player de volta. Isso ficou marcado como pendência nas
  Entradas anteriores e continua pendente de verificação — ver
  CURRENT_STATUS.md item 3.
- `DropTable` é uma classe C# simples, não um `Resource` do Godot — mesma
  decisão de risco já tomada para `DamageInfo`: um `Resource` customizado
  exigiria `sub_resource` escrito à mão em `.tscn` sem o editor aberto
  pra validar a sintaxe.
- `LootGenerator` é o único ponto do código que sabe instanciar
  `GroundItem` — futuras mudanças de item/raridade/apresentação mexem só
  nele, não em todo actor que possa dropar loot.
- `GroundItem` usa a collision layer/mask **padrão** (1), não a layer 2
  dedicada de combate — quem restringe a coleta ao player é
  `IsInGroup("player")` no código, não a camada de colisão. Isso também
  motivou adicionar `AddToGroup("player")` em `PlayerController._Ready()`
  (novo nesta etapa) — mesmo grupo que `EnemyController` usa para
  encontrar o alvo via `GetTree().GetFirstNodeInGroup("player")`.
- `LootDropChance` (50%) + `MinLootDrops`/`MaxLootDrops` (1-2) no
  `EnemyController` significam que **matar o inimigo às vezes não dropa
  nada** — comportamento esperado, não bug, ao testar.

### Nota de design — braço/perna visíveis nos personagens

O usuário perguntou se personagens futuros terão braço/perna visíveis,
porque hoje (placeholders `Polygon2D` sólidos) não dá pra ver claramente
o que está acontecendo no jogo. Resposta enquanto ainda não há arte real:

- O placeholder atual (quadrado colorido) é deliberadamente burro — o
  objetivo dele nunca foi comunicar ação, só existir/colidir/ser visível
  o suficiente pra testar sistemas (MASTER_CONTEXT.md, "Direção visual":
  arte de verdade é trabalho explicitamente adiado).
  Sprites/animações com membros articulados (idle/walk/attack/hit/death
  por direção) fazem parte do pipeline de arte, que não está no escopo de
  nenhuma etapa de código ainda — não há uma "etapa de arte" numerada no
  ROADMAP.md atual; isso é uma decisão de conteúdo/produção do usuário,
  não uma decisão técnica minha.
- **O que já existe hoje, sem esperar arte**, pra tornar a ação mais
  legível: feedback de dano (opacidade cai com o HP), feedback de morte
  (cinza translúcido), e o hitbox de ataque do inimigo é posicionado na
  direção do player durante o ataque — mas nada disso substitui uma pose
  de ataque visível.
- Quando o usuário tiver arte (ou placeholders melhores) com membros
  articulados, o lugar certo pra isso é trocar o nó `Polygon2D`
  "Placeholder" por um `AnimatedSprite2D`/`Sprite2D` + `AnimationPlayer`
  dentro de `Player.tscn`/`Enemy.tscn` — não muda nenhum script de lógica
  (`PlayerController`, `EnemyController`, etc.), porque eles não sabem
  nada sobre a representação visual hoje. Ou seja: a arquitetura atual já
  comporta essa troca sem refatoração, é só uma questão de quando o
  usuário tiver a arte/decisão de estilo pronta.

### Arquivos alterados/criados

Novos: `Game/Actors/Enemies/{EnemyController.cs,Enemy.tscn}`,
`Game/Loot/{DropTable.cs,LootGenerator.cs,GroundItem.cs,GroundItem.tscn}`.
Modificados: `Game/Actors/Player/PlayerController.cs` (`AddToGroup("player")`
em `_Ready()`), `Game/World/TestWorld.tscn` (instância de `Enemy` em
`(-200, 150)`), além de `ARCHITECTURE.md`, `ROADMAP.md`,
`CURRENT_STATUS.md`.

### Testes executados e resultados

- ✅ Verificado nesta sessão: balanceamento de chaves (`{}`) em todos os
  `.cs` novos/alterados (`EnemyController.cs` 16/16, `DropTable.cs` 4/4,
  `GroundItem.cs` 4/4, `LootGenerator.cs` 6/6, `PlayerController.cs` 8/8).
- ✅ Verificado nesta sessão: recontagem de `load_steps` em todos os
  `.tscn` novos/alterados (`Enemy.tscn` 10/10, `GroundItem.tscn` 3/3,
  `TestWorld.tscn` 8/8) e nos já existentes que dependem deles
  (`Player.tscn` 11/11, `Dummy.tscn` 6/6) — nenhuma regressão.
- ✅ Verificado nesta sessão: balanceamento de parênteses/colchetes em
  todos os `.tscn` do projeto.
- ✅ Verificado nesta sessão: sinais usados (`Health.Damaged(float,
  float)`, `Health.Died()`) batem exatamente com as assinaturas
  declaradas em `Health.cs` — `EnemyController.OnDamaged(float amount,
  float currentHealth)` e `OnDied()` conectados corretamente.
- ❌ **NÃO testado**: compilação real do C# (sem SDK .NET/Godot neste
  ambiente). ❌ **NÃO testado**: comportamento em runtime (IA
  Idle/Chase/Attack/Recover/Dead, dano real ao player, drop/pickup de
  loot) — nada disso foi observado rodando de verdade ainda. Ver
  CURRENT_STATUS.md "Known Issues" para o checklist exato do que precisa
  ser testado localmente antes de considerar a Etapa 4+5 concluída.

### Pendências

- Testar Etapa 4+5 localmente no Godot (checklist em CURRENT_STATUS.md).
- Push local da branch `feature/etapa-4-5-inimigo-e-loot` (comandos no
  final desta entrada) e abertura de PR.
- Decisão pendente do usuário sobre a branch `docs/handoff` (conteúdo
  escrito por outra IA — Grok — a partir de uma visão desatualizada do
  repo, pré-Etapa 0). Não decidir/mesclar/apagar nada dela
  unilateralmente.
- Pipeline de arte (sprites articulados) é decisão futura do usuário, sem
  etapa numerada ainda — ver "Nota de design" acima.

### Erros conhecidos

Nenhum novo nesta etapa. Erros de sessões anteriores (miscontagem de
`load_steps` no Player.tscn, entrada perdida do HANDOFF.md num push
anterior) seguem documentados nas Entradas 1-4, sem impacto no código
atual.

### Branch / commit

Trabalho feito em cima da ponta de `feature/etapa-2-3-combat-dodge-block`
(commit `8ba1764`). Por consistência com a separação já usada entre
Etapa 0-1 e Etapa 2-3, uma branch dedicada
**`feature/etapa-4-5-inimigo-e-loot`** foi criada nesta sessão a partir
desse commit, e o commit da Etapa 4+5 foi feito nela — ver o hash exato
no `git log` local ou no commit mais recente desta branch. Este ambiente
de nuvem **não consegue fazer `git push`** (limitação de plataforma já
documentada nas Entradas 2-3) — os arquivos foram copiados para a pasta
local do usuário (`C:\Dev\mystery-absolut`) via mirror de arquivos; o
usuário faz o commit/push locais.

Comandos passados ao usuário (PowerShell, a partir de
`C:\Dev\mystery-absolut`, assumindo que a branch local ainda está em
`feature/etapa-2-3-combat-dodge-block` na ponta pushada anteriormente):

```powershell
git checkout feature/etapa-2-3-combat-dodge-block
git pull origin feature/etapa-2-3-combat-dodge-block
git checkout -b feature/etapa-4-5-inimigo-e-loot
git add Game/Actors/Enemies Game/Loot Game/Actors/Player/PlayerController.cs Game/World/TestWorld.tscn ARCHITECTURE.md ROADMAP.md CURRENT_STATUS.md HANDOFF.md
git commit -m "Etapa 4 + Etapa 5: primeiro inimigo (IA) e loot basico"
git push -u origin feature/etapa-4-5-inimigo-e-loot
```

Depois disso, abrir PR no GitHub com base na branch
`feature/etapa-2-3-combat-dodge-block` (ainda não confirmada como
mergeada na `main`) — se ela já tiver sido mergeada na `main` quando o
usuário for abrir o PR, trocar a base para `main` em vez disso.

### Prompt pronto para a próxima IA

```
Leia HANDOFF.md (Entrada 5, a mais recente), CURRENT_STATUS.md e
ARCHITECTURE.md neste repositório antes de fazer qualquer coisa. Etapa
0-1-2-3 estão implementadas e verificadas de verdade pelo usuário
(vídeo). Etapa 4 (Primeiro inimigo) e Etapa 5 (Loot básico) estão
implementadas nesta sessão mas AINDA NÃO testadas no Godot — não declare
isso concluído até o usuário confirmar com vídeo/teste local (ver
checklist em CURRENT_STATUS.md "Known Issues"). NÃO inicie a Etapa 6
(Item System) nem nenhuma etapa futura sem autorização explícita e
literal do usuário para aquela etapa específica — não presuma
continuidade automática. Mantenha HANDOFF.md atualizado (nova entrada no
topo) a cada parada seguindo o mesmo formato desta entrada. Há também uma
branch `docs/handoff` no repositório remoto com conteúdo escrito por
outra IA (Grok) a partir de uma visão desatualizada do projeto (pré-Etapa
0) — não mesclar nem apagar nada dela sem o usuário decidir explicitamente
o que fazer.
```

---

## Entrada 4 — 2026-09-15 — Claude (Sonnet 5, sessão claude.ai/code)

### O que foi feito

1. **Verificação real da Etapa 0+1**: o usuário gravou um vídeo (15s,
   OBS) mostrando o jogo rodando no Godot local. Extraí frames do vídeo
   (`ffmpeg`) e conferi visualmente: player (quadrado azul) se move,
   colide com as 2 paredes/obstáculos marrons, câmera acompanha — bate
   exatamente com `TestWorld.tscn`. **Isso conta como verificação real**,
   diferente de "o usuário disse que funcionou": eu mesmo inspecionei os
   frames.
2. **Etapa 2 (Combat Foundation)** e **Etapa 3 (Dodge/Block)**
   implementadas, autorizadas explicitamente pelo usuário (confirmadas
   por pergunta direta depois de uma confusão inicial sobre os números
   das etapas).

### Como funciona

Detalhado em ARCHITECTURE.md (seções "Sistemas de combate" e "Player —
ataque, dodge, block"). Resumo das decisões não-óbvias (todas também
comentadas no próprio código):

- `PlayerController` continua a única autoridade sobre
  `Velocity`/`MoveAndSlide()` mesmo durante o dodge — `PlayerDodge` só
  seta `IsDodging`/`DodgeVelocity` (dados), nunca chama `MoveAndSlide()`.
  Evita dois scripts brigando pelo movimento no mesmo frame físico.
- `Hitbox`/`Hurtbox` usam uma collision layer própria (layer 2) separada
  da física normal (layer 1, já usada por player/paredes/obstáculos) —
  evita qualquer interferência com a colisão já verificada da Etapa 1.
- `Hitbox` rastreia `_alreadyHit` por ativação, pra um swing não bater
  várias vezes no mesmo alvo em frames físicos sucessivos de overlap.
- Sinais do Godot (`[Signal] delegate void XEventHandler(...)`) só usam
  tipos primitivos/Godot Object nos parâmetros (`float`, `Node2D`) — um
  struct customizado como `DamageInfo` NÃO é seguro como parâmetro de
  sinal (o marshalling de sinais do Godot exige tipos Variant-compatíveis),
  por isso `Hurtbox.Parried` manda `(float amount, Node2D source)` em vez
  de `DamageInfo` diretamente.
- Teclas novas (`attack`=Space, `dodge`=C, `block`=X) foram escolhidas
  deliberadamente como letras/Space simples, não teclas modificadoras
  (Shift/Ctrl) — havia incerteza real sobre o nome exato do enum
  `Godot.Key` para "Shift genérico", e diferente do risco de digitar
  inteiros errados num `project.godot` à mão (que falha silenciosamente),
  aqui um nome de enum errado geraria erro de compilação visível. Ainda
  assim, optei pelas teclas de que eu tinha certeza absoluta.
- "Base de parry" (pedido explícito da Etapa 3) foi implementada como uma
  janela curta (0.15s) após apertar block em que um hit é totalmente
  anulado e emite `Hurtbox.Parried`, sem nenhum sistema de stagger/
  contra-ataque no atacante — isso foi deixado de propósito para uma
  etapa futura, que pode reagir ao sinal sem tocar nesses arquivos.

### Arquivos alterados/criados

Novos: `Game/Combat/{DamageInfo,Health,Combatant,Hurtbox,Hitbox,Cooldown,Dummy}.cs`,
`Game/Combat/Dummy.tscn`, `Game/Actors/Player/{PlayerBasicAttack,PlayerDodge,PlayerBlock}.cs`.
Modificados: `Game/Actors/Player/PlayerController.cs` (FacingDirection,
IsDodging/DodgeVelocity), `Game/Actors/Player/Player.tscn` (nós de
combate), `Game/World/TestWorld.tscn` (instância de Dummy),
`Game/Core/InputBootstrap.cs` (ações attack/dodge/block), além de
`ARCHITECTURE.md`, `ROADMAP.md`, `CURRENT_STATUS.md`.

### Testes executados e resultados

- ✅ Verificado nesta sessão: balanceamento de parênteses/colchetes/chaves
  em todos os arquivos novos/alterados (`.cs` e `.tscn`).
- ✅ Verificado nesta sessão: `load_steps` de cada `.tscn` recontado
  programaticamente contra o número real de `ext_resource`+`sub_resource`
  (achei e corrigi um erro de contagem no `Player.tscn` antes de seguir).
- ✅ Verificado nesta sessão (por mim, não só relatado pelo usuário):
  Etapa 0+1 funcionando de verdade, via frames extraídos do vídeo.
- ❌ **Não verificado**: build da Etapa 2+3 (ainda sem Godot/dotnet nesta
  sessão). Nenhuma das mecânicas de combate (ataque, dano no Dummy, dodge,
  block, parry) foi executada de fato.

### Pendências

1. Abrir a Etapa 2+3 no Godot local, build, 0 erros — checklist detalhado
   em CURRENT_STATUS.md.
2. Testar ataque contra o Dummy, dodge, block (ver ressalva em
   CURRENT_STATUS.md: dodge/block só são totalmente testáveis contra dano
   real quando existir algo que ataque o player — isso é Etapa 4).
3. Commit + push desta etapa — **mesmo bloqueio de sempre nesta sessão**
   (ver Entrada 2/3). Branch/commit exatos abaixo.
4. Etapa 4 (Primeiro inimigo) só começa depois de tudo isso, com
   autorização explícita — regra de parada.

### Erros conhecidos

Nenhum erro de build conhecido, porque build não foi tentado (ver acima).
Bloqueio de push continua o mesmo de sempre (política da sessão, não do
repositório — ver Entrada 2).

### Branch / commit

- Trabalho local desta sessão em cima de `ebc6c8b` (main do PR #2). Ainda
  não commitado nesta entrada — ver o commit real logo após esta escrita
  no histórico do git (`git log`), pois o commit é feito depois deste
  arquivo ser salvo.

### Prompt pronto para a próxima IA continuar

```
Você está continuando o desenvolvimento do jogo Mystery Absolut
(Godot 4.7.2 Mono + C#). Leia nesta ordem: MASTER_CONTEXT.md,
ARCHITECTURE.md, ROADMAP.md, CURRENT_STATUS.md, HANDOFF.md (entrada mais
recente no topo).

Estado atual: Etapa 0+1 verificadas de verdade (vídeo conferido). Etapa 2
(Combat Foundation) e Etapa 3 (Dodge/Block) foram implementadas mas AINDA
NÃO foram abertas no Godot nem testadas — ver "Testes executados" e
"Pendências" desta entrada.

Sua tarefa, nesta ordem:
1. Se tiver Godot 4.7.2 Mono + dotnet: abra o projeto, build (0 erros),
   rode o jogo. Ande até o Dummy (200, -150 em TestWorld) e teste attack
   (Space), dodge (C), block (X). Corrija o que estiver quebrado sem
   adicionar funcionalidade nova.
2. Atualize CURRENT_STATUS.md e adicione nova entrada no topo deste
   HANDOFF.md com o resultado REAL de cada teste.
3. Commit e, se tiver permissão, push + PR. Se não tiver, documente aqui
   como as entradas anteriores fizeram.
4. PARE. Não inicie a Etapa 4 sem autorização explícita do usuário
   especificamente para essa etapa.

Regra permanente (ver MASTER_CONTEXT.md): ao terminar, encerrar a sessão,
ou perceber limite de uso próximo, pare em ponto seguro, salve as
alterações, atualize este HANDOFF.md antes de parar.
```

---

## Entrada 3 — 2026-09-15 — Claude (Sonnet 5, sessão claude.ai/code)

### O que mudou

O usuário fez o push local (branch `feature/etapa-0-1-fundacao-e-movimento`,
commit único `ebc6c8b` — o git local dele já tinha as credenciais, então
funcionou de primeira) e abriu o **Pull Request #2**
(`https://github.com/azhumberty/mystery-absolut/pull/2`,
`feature/etapa-0-1-fundacao-e-movimento` → `main`, 14 arquivos, +774,
ainda sem descrição, sem reviewers, sem checks configurados).

### Nota sobre uma inconsistência encontrada

O commit `ebc6c8b` que o usuário pushou contém o conteúdo da **Entrada 1**
deste arquivo, mas não contém a Entrada 2 (que existia no clone desta
sessão em nuvem, commit `b49bbc4`, e tinha sido copiada para
`C:\Dev\mystery-absolut\HANDOFF.md` via transferência de arquivo antes do
push). Causa exata não confirmada — possivelmente o `git add` local foi
executado a partir de um estado da pasta anterior à cópia mais recente, ou
um editor local re-salvou uma versão antiga por cima. Não tem impacto
prático (a Entrada 2 só registrava uma decisão em aberto que já foi
resolvida), mas fica registrado aqui como lição: **depois de eu escrever
arquivos direto na pasta do usuário via transferência (sem git), vale
confirmar com `git status`/`git diff` local antes de assumir que o próximo
commit vai pegar exatamente o que foi escrito** — principalmente se a
pasta pode estar aberta em outro editor.

### Pendências (sem mudança em relação à Entrada 1/2)

Build/execução no Godot ainda não foram verificados de fato (ver Entrada
1). Isso deveria acontecer **antes do merge do PR #2**, não depois — é a
única validação real de que "Projeto compila / 0 erros / player se move"
(checklist da seção 72 do MASTER_HANDOFF) é verdade. Etapa 2 continua sem
autorização para começar.

### Branch / commit / PR

- Branch: `feature/etapa-0-1-fundacao-e-movimento`
- Commit: `ebc6c8b` (versão que está no GitHub agora — squash de tudo em
  1 commit, feito pelo usuário localmente)
- PR: #2, aberto, aguardando verificação local + review antes do merge.

---

## Entrada 1 — 2026-09-15 — Claude (Sonnet 5, sessão claude.ai/code)

### O que foi feito

Implementadas **Etapa 0 (Fundação)** e **Etapa 1 (Player Movement)** do
ROADMAP.md, a pedido explícito do usuário (única autorização dada até
aqui: "pode iniciar etapas 0 e 1" — nada além disso).

### Como funciona

Descrição completa em `ARCHITECTURE.md` (seção "Fluxo de cena", "InputMap",
"Player", "TestWorld"). Resumo:

- `Scenes/Main.tscn` é o `run/main_scene`; instancia `Game/World/TestWorld.tscn`.
- `TestWorld.tscn` é uma sala fechada (chão + 4 paredes + 2 obstáculos,
  todos com `CollisionShape2D`) contendo uma instância de
  `Game/Actors/Player/Player.tscn`.
- `Player.tscn` é um `CharacterBody2D` (motion_mode Floating, sem
  gravidade) com `PlayerController.cs` controlando movimento em 8 direções
  via `Input.GetVector("move_left","move_right","move_up","move_down")`
  (diagonal já normalizada pela própria API do Godot — decisão: não
  normalizar manualmente, ver comentário no arquivo). Um `Camera2D` filho
  do player segue automaticamente por herança de transform, sem código.
- `Game/Core/InputBootstrap.cs` é um autoload que registra as 4 ações de
  movimento via `InputMap` em código no `_Ready()`, em vez de escrever à
  mão o bloco `[input]` do `project.godot`. **Decisão não óbvia e o motivo
  está documentado em comentário XML no próprio arquivo**: editar
  `[input]` manualmente exige códigos de tecla numéricos que são fáceis de
  errar sem o editor Godot aberto para validar, e o erro seria silencioso
  (ação vazia ou tecla errada não quebra o build, só o gameplay). Usar o
  enum `Godot.Key` em código elimina essa classe de erro.

### Arquivos alterados/criados

Commit `e4ceb91` na branch `feature/etapa-0-1-fundacao-e-movimento`
(branch a partir de `main` @ `6076568`):

- Novos: `MASTER_CONTEXT.md`, `ARCHITECTURE.md`, `ROADMAP.md`,
  `CURRENT_STATUS.md`, `Mystery Absolut.csproj`, `Scenes/Main.tscn`,
  `Game/World/TestWorld.tscn`, `Game/Actors/Player/Player.tscn`,
  `Game/Actors/Player/PlayerController.cs`, `Game/Core/InputBootstrap.cs`,
  `UI/README.md`, `Data/README.md`.
- Modificado: `project.godot` (só `run/main_scene` e `[autoload]`
  adicionados; nada pré-existente foi removido ou alterado).

### Testes executados e resultados

**Nenhum teste real foi executado.** Sendo preciso sobre o que foi e não
foi verificado:

- ✅ Verificado nesta sessão: balanceamento de parênteses/colchetes/chaves
  em todos os `.tscn` e `.cs` novos (checagem sintática superficial via
  script, não é validação do parser do Godot).
- ✅ Verificado nesta sessão: clone do repositório, estrutura de arquivos,
  `git log`/`git status` conferidos.
- ❌ **Não verificado**: build do projeto (`dotnet build` — o ambiente
  desta sessão não tem o SDK .NET nem o Godot instalados; tentativa de
  instalar `dotnet-sdk-8.0` via apt falhou com 404 nos pacotes; tentativa
  de baixar o instalador oficial de dot.net foi bloqueada pela política de
  rede do ambiente).
- ❌ **Não verificado**: abrir o projeto no Godot, geração do `.sln`,
  execução, aparição do player, movimento nas 8 direções, colisão com
  paredes/obstáculos, câmera seguindo o player.
- ❌ **Não verificado**: se os bindings de teclado (`InputBootstrap.cs`)
  realmente funcionam em tempo de execução.

Ou seja: o checklist da seção 72 do MASTER_HANDOFF original (projeto
abre / compila / 0 erros / player aparece / move em 8 direções / diagonal
normalizada / colisão / câmera segue / etc.) **não pode ser marcado como
cumprido ainda** — está redigido como pendência explícita em
`CURRENT_STATUS.md`.

### Pendências

1. Abrir o projeto no Godot 4.7.2 Mono local e confirmar o checklist da
   seção 72 do MASTER_HANDOFF (passos detalhados em `CURRENT_STATUS.md`).
2. Corrigir qualquer erro de build/runtime encontrado (se houver) antes de
   avançar para a Etapa 2.
3. Push da branch `feature/etapa-0-1-fundacao-e-movimento` para o GitHub —
   **bloqueado nesta sessão** (ver "Erros conhecidos" abaixo).
4. Abrir Pull Request para `main` depois do push, para revisão antes de
   integrar (pedido explícito do usuário na primeira mensagem desta
   conversa).
5. Só depois de tudo acima: iniciar Etapa 2 (Combat Foundation) — em uma
   tarefa separada, com autorização explícita do usuário (regra de
   parada).

### Erros conhecidos

- `git push` para `https://github.com/azhumberty/mystery-absolut.git`
  falha com HTTP 403: `access denied by the git proxy: azhumberty/mystery-absolut
  is not in this session's authorized repository set`. Isto é uma política
  de rede da sessão desta IA (Claude, ambiente claude.ai/code), não um
  problema do repositório ou das credenciais do usuário. Correção: o
  usuário precisa adicionar o repositório às fontes autorizadas desta
  sessão, ou o push/PR precisa ser feito por outro caminho (terminal local
  do usuário, ou outra ferramenta/IA com acesso). Confirmado duas vezes
  nesta sessão (mesma mensagem de erro nas duas tentativas).
- Nenhum erro de compilação conhecido — porque a compilação não foi
  tentada (ver "Testes executados" acima). Isso NÃO é o mesmo que "compila
  sem erros".

### Branch / commit

- Branch: `feature/etapa-0-1-fundacao-e-movimento`
- Commit: `e4ceb91` ("Etapa 0 + Etapa 1: fundação do projeto e movimento
  do player")
- Base: `main` @ `6076568` ("Initial Godot project")
- Estado do push: commitado localmente (no clone desta sessão em nuvem, e
  também copiado como arquivos — ainda sem commit próprio — para
  `C:\Dev\mystery-absolut` do usuário). **Não pushado ao GitHub.**

### Prompt pronto para a próxima IA continuar

```
Você está continuando o desenvolvimento do jogo Mystery Absolut
(Godot 4.7.2 Mono + C#). Antes de qualquer coisa, leia nesta ordem:
MASTER_CONTEXT.md, ARCHITECTURE.md, ROADMAP.md, CURRENT_STATUS.md e
HANDOFF.md (entrada mais recente no topo) na raiz do repositório
https://github.com/azhumberty/mystery-absolut.

Estado atual: Etapa 0 (Fundação) e Etapa 1 (Player Movement) foram
implementadas na branch `feature/etapa-0-1-fundacao-e-movimento`
(commit e4ceb91), mas AINDA NÃO foram verificadas localmente (build,
execução, teste de movimento) nem pushadas ao GitHub — ver seção
"Testes executados e resultados" e "Erros conhecidos" da entrada mais
recente do HANDOFF.md para o motivo exato.

Sua tarefa, nesta ordem, sem pular etapas:
1. Se você tiver acesso a um ambiente com Godot 4.7.2 Mono e dotnet
   instalados: abra o projeto, gere/sincronize a solução C# se
   necessário (Project → Tools → C# → Create/Sync C# Solution), rode
   o build, confirme 0 erros, rode o jogo e confirme o checklist da
   seção 72 do MASTER_HANDOFF (player aparece, move em 8 direções,
   diagonal normalizada, colide com paredes/obstáculos, câmera segue).
   Corrija o que estiver quebrado, sem adicionar funcionalidade nova.
2. Atualize CURRENT_STATUS.md e adicione uma nova entrada no topo
   deste HANDOFF.md registrando exatamente o que foi testado e o
   resultado real (nunca declare testado o que não rodou).
3. Faça commit e, se tiver permissão de push, push da branch e abra o
   PR para main. Se não tiver permissão, documente isso aqui como fez
   a entrada anterior.
4. PARE. Não inicie a Etapa 2 (Combat Foundation) sem autorização
   explícita do usuário para essa etapa especificamente.

Regra permanente: ao terminar sua etapa, encerrar a sessão, ou perceber
que seu limite de uso está próximo, pare em um ponto seguro, salve as
alterações, e atualize este HANDOFF.md (nova entrada no topo, mesmo
formato desta) antes de parar — mesmo que a etapa não tenha sido
concluída.
```

---

# CURRENT_STATUS.md — Mystery Absolut

*(mantido curto, de propósito — detalhes completos em ARCHITECTURE.md;
histórico completo de handoff, decisões e testes executados/pendentes em
HANDOFF.md)*

## Completed

- **Etapa 0+1 (Fundação/Movimento), Etapa 2+3 (Combate/Dodge/Block),
  Etapa 4+5 (Inimigo/Loot), Etapa 6+7 (Itens/Inventário) — todas
  verificadas de verdade pelo usuário**, cada uma com vídeo conferido
  quadro a quadro. Detalhes em HANDOFF.md (Entradas 1-7).
- **Etapa 8 (Equipment) + Etapa 9 (Affixes) — verificadas de verdade pelo
  usuário** (quinto vídeo, conferido nesta sessão: projeto compilou com 0
  erros, arma equipada aparece no slot Weapon com raridade `[Magic]` e o
  afixo rolado `+1.1 Dano Físico` exibidos corretamente).
- **Etapa 10 (Crafting)**: as 4 currencies aparecem corretamente no
  inventário (sem raridade — bug da Entrada 7 confirmado corrigido), mas
  nenhum botão "[usar]" foi clicado no vídeo — aplicar um efeito de
  crafting de verdade **ainda não tem confirmação visual**.
- **Etapa 11 (Loot Presentation)**: `LootPresentation` (cor/contorno/
  glow/beam/partículas por raridade) + `GroundItem.ApplyPresentation()`
  construindo tudo em código (Label, Outline, Glow com Tween pulsante,
  Beam, `CpuParticles2D`). Som deliberadamente fora (sem assets de áudio).
- **Etapa 12 (Loot Filter)**: `LootFilterRule`/`LootFilterDatabase`
  avaliando `Data/Loot/loot_filter.json` (4 regras: mostra currency/
  consumível com cor própria, reforça Rare/Unique, esconde Normal
  equipável) — primeira regra que bate vence, sem regra = default da
  Etapa 11. "Hide" = invisível mas continua coletável (física independe
  de `Visible`).
- **Etapa 13 (Stash)**: `Stash`/`StashCategory` (General/Equipment/
  Currency/Unique, componente-filho do Player como Inventory/Equipment —
  localização provisória documentada), `StashUI` (painel novo, tecla T,
  independente do Inventário) com guardar/retirar.

## Working

Etapa 0-1, 2-3, 4-5, 6-7 e 8-9 (equipamento+afixos): **confirmadas
funcionando de verdade** (vídeos do usuário, conferidos). Etapa 10
(crafting): compila e os itens aparecem certos, mas o "usar" em si segue
não-testado. Etapa 11+12+13: escritas nesta sessão, seguindo os mesmos
padrões já comprovados (JSON/FileAccess, singleton lazy, UI 100% em
código) mais APIs novas verificadas via busca (`Tween.BindNode`,
`CpuParticles2D`, `Label` como filho de `Area2D`) — mas **ainda não
abertas no Godot nesta sessão**. Ver "Known Issues" abaixo para o
checklist exato do que falta testar.

## Important Files

- `Game/Loot/LootPresentation.cs` (novo, Etapa 11)
- `Game/Loot/GroundItem.cs` (reescrito: `ApplyPresentation()` +
  Build{Label,Outline,Glow,Beam,Particles}, chamado em `_Ready()`)
- `Game/Loot/LootFilterRule.cs`, `LootFilterColor.cs`, `LootFilterResult.cs`,
  `LootFilterDatabase.cs` (novos, Etapa 12)
- `Data/Loot/loot_filter.json` (novo, Etapa 12: 4 regras)
- `Game/Inventory/Stash.cs`, `StashCategory.cs` (novos, Etapa 13)
- `UI/StashUI.cs`, `UI/StashUI.tscn` (novos, Etapa 13)
- `Game/Actors/Player/Player.tscn` (atualizado: nó `Stash`)
- `Scenes/Main.tscn` (atualizado: instância de `StashUI`)
- `Game/Core/InputBootstrap.cs` (atualizado: ação `stash` = tecla T)

## Architecture

Ver ARCHITECTURE.md — seções "Apresentação de loot — Etapa 11", "Loot
Filter — Etapa 12", "Stash — Etapa 13". Resumo: `LootPresentation` dá o
visual-padrão por raridade (código, não JSON — são só 4 valores fixos de
direção de arte); `LootFilterDatabase` é a camada data-driven de verdade
por cima disso (regras JSON, primeira que bate vence, fallback = Etapa
11); `GroundItem` monta seus nós de apresentação inteiramente em código
(mesmo motivo de sempre: nada de `.tscn` não-verificável). `Stash` mora
temporariamente como componente-filho do Player (mesmo padrão de
Inventory/Equipment) até existir um lugar mais correto (save/conta) pra
ele morar — decisão documentada, não definitiva.

## Known Issues / Verificação pendente

Etapa 11+12+13 (apresentação de loot/loot filter/stash) **não foi aberta
no Godot ainda**. Antes de considerar isso realmente concluído, é preciso,
localmente:

1. Abrir o projeto (branch nova, ver "Branch/PR" abaixo), build, 0 erros
   de compilação. **Atenção especial**: esta etapa usa `Tween`/
   `CreateTween()`/`BindNode` e `CpuParticles2D` pela primeira vez no
   projeto, além de um `Label` sendo criado em código como filho de um
   `Area2D` — tudo verificado via busca na documentação oficial do Godot
   4.x antes de escrever (não por experiência prévia real neste código),
   então é o ponto de maior risco desta leva de etapas.
2. Farmar alguns drops e observar: item Normal aparece com label branco
   (SEM contorno/glow/beam — e, com o filtro padrão, **nem deveria
   aparecer visível no chão**, já que a Etapa 12 esconde Normal por
   padrão — mas continua coletável andando por cima). Currency mostra
   label dourado. Consumível mostra label verde. Item Magic mostra label
   azul com contorno fino. Se algo dropar Rare (5% de chance): contorno +
   glow pulsando + beam curto pra cima.
3. Confirmar que itens escondidos pelo filtro (Normal) ainda são
   coletáveis andando por cima, mesmo invisíveis — esse é o comportamento
   pretendido de "Hide" (ver ARCHITECTURE.md), não um bug.
4. Apertar T: confirmar que o painel do Baú abre/fecha, independente do
   painel de Inventário (I) — dá pra abrir os dois ao mesmo tempo.
5. Clicar num item no inventário dentro do painel do Baú: confirmar que
   ele some do inventário e aparece na aba certa (equipável → Equipment,
   currency → Currency, resto → Geral — nada deveria cair em Unique
   ainda, já que Unique não é sorteável). Clicar num item guardado:
   confirmar que ele volta pro inventário.
6. Se algo quebrar: corrigir antes de avançar (regra: nunca deixar o
   projeto quebrado).

**Nota:** dano ao player pelo Enemy (Etapa 3, dodge/block reduzindo dano)
segue sem confirmação visual. Aplicar um crafting effect de verdade
(botão "[usar]", Etapa 10) também segue sem confirmação visual. Nenhum
dos dois bloqueia Etapa 11+12+13, mas ambos continuam como pendências em
aberto — ver HANDOFF.md.

## Branch / PR

- Etapa 0+1: branch `feature/etapa-0-1-fundacao-e-movimento`, PR aberto
  pelo usuário (commit `ebc6c8b`).
- Etapa 2+3: branch `feature/etapa-2-3-combat-dodge-block`, commit
  `8ba1764` — verificada por vídeo.
- Etapa 4+5: branch `feature/etapa-4-5-inimigo-e-loot`, commit `2294d49`
  — verificada por vídeo.
- Etapa 6+7: branch `feature/etapa-6-7-itens-inventario`, commit
  `18fcb0e` — verificada por vídeo.
- Etapa 8+9+10: branch `feature/etapa-8-9-10-equipamento-afixos-crafting`,
  commit `34adb06` — **Equipment/Affixes verificados por vídeo nesta
  sessão; Crafting ainda sem confirmação de uso.**
- Etapa 11+12+13: nova branch dedicada
  `feature/etapa-11-12-13-loot-presentation-filter-stash`, criada a
  partir da ponta de `feature/etapa-8-9-10-equipamento-afixos-crafting`
  — ver HANDOFF.md para o commit exato e comandos de push.

## Next Step

Verificar Etapa 11+12+13 localmente no Godot (passos acima). Depois
disso, commit/push local (mesma limitação de sempre: esta sessão não
consegue dar `git push` — ver HANDOFF.md) e atualizar este arquivo com o
resultado real. Etapa 14 (Primeiro Companion) só começa depois disso, com
autorização explícita do usuário — **não antecipar**.

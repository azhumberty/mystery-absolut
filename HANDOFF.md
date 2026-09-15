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

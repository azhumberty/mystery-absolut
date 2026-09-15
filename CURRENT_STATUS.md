# CURRENT_STATUS.md — Mystery Absolut

*(mantido curto, de propósito — detalhes completos em ARCHITECTURE.md;
histórico completo de handoff, decisões e testes executados/pendentes em
HANDOFF.md)*

## Completed

- Etapa 0 (Fundação): estrutura de pastas mínima, `Mystery Absolut.csproj`,
  `MASTER_CONTEXT.md`, `ARCHITECTURE.md`, `ROADMAP.md`,
  `CURRENT_STATUS.md`, cena inicial (`Scenes/Main.tscn`) configurada como
  `run/main_scene`.
- Etapa 1 (Player Movement): `Player.tscn` + `PlayerController.cs`
  (`CharacterBody2D`, 8 direções via `Input.GetVector`, diagonal já
  normalizada, câmera acoplada ao player), `TestWorld.tscn` (chão, 4
  paredes, 2 obstáculos, todos com colisão), `InputBootstrap.cs` (autoload
  que registra move_up/down/left/right em código).

## Working

Não verificado localmente ainda (ver "Known Issues" abaixo) — o código foi
escrito seguindo a API padrão do Godot 4.x C#, mas **nenhum build ou
execução real foi feito** nesta sessão.

## Important Files

- `Mystery Absolut.csproj`
- `project.godot` (`run/main_scene`, `[autoload] InputBootstrap`
  adicionados; resto preservado)
- `Scenes/Main.tscn`
- `Game/World/TestWorld.tscn`
- `Game/Actors/Player/Player.tscn`, `Game/Actors/Player/PlayerController.cs`
- `Game/Core/InputBootstrap.cs`

## Architecture

Ver ARCHITECTURE.md — resumo: `Main → TestWorld → Player`, namespaces
`Game.Core` / `Game.Actors.Player`, InputMap registrado em código (não em
`project.godot`), PlayerController contém só input/movimento.

## Known Issues / Verificação pendente

Esta etapa foi implementada em um ambiente sem o editor Godot e sem o SDK
.NET instalados — **não foi possível compilar nem rodar o projeto aqui**.
Antes de considerar a Etapa 0+1 realmente concluída (checklist do
MASTER_HANDOFF seção 72), é preciso, localmente em `C:\Dev\mystery-absolut`:

1. Abrir o projeto no Godot 4.7.2 Mono.
2. Se o Godot não gerar sozinho o `.sln`: **Project → Tools → C# →
   Create/Sync C# Solution**.
3. Build (deve dar 0 erros).
4. Rodar o projeto — deve abrir direto em `Scenes/Main.tscn`, mostrar o
   player (quadrado azul) dentro da sala de teste.
5. Testar movimento nas 8 direções (WASD e/ou setas), confirmar que
   diagonal não é mais rápida, confirmar colisão com paredes/obstáculos e
   que a câmera segue o player.
6. Se tudo passar: marcar este item como resolvido aqui e seguir para a
   Etapa 2. Se algo falhar, corrigir antes de avançar (regra: nunca deixar
   o projeto quebrado).

Push para o GitHub também está pendente: esta sessão não tem permissão de
push configurada para este repositório (bloqueado pela política de rede do
ambiente) — as mudanças estão commitadas localmente na branch
`feature/etapa-0-1-fundacao-e-movimento`, aguardando push manual ou
autorização do repositório para push remoto.

## Next Step

Verificar/compilar localmente (passos acima). Depois disso, push da branch
`feature/etapa-0-1-fundacao-e-movimento` e abertura de PR para `main`. Só
então iniciar a Etapa 2 (Combat Foundation) — em uma tarefa separada.

# HANDOFF — Mystery Absolut

Documento de continuidade entre IAs (Grok, Gemini Pro, Codex, Claude).

**Obrigatório:** este arquivo vive na **raiz do repositório**. Toda etapa, sessão longa, encerramento ou risco de limite de uso deve atualizá-lo **antes de parar**. Sem `HANDOFF.md` no GitHub, a próxima IA não tem contexto.

Não declare concluído ou testado o que não foi.

---

## 1. O que foi feito

### Publicado em `main`

- Repositório GitHub criado: https://github.com/azhumberty/mystery-absolut
- Projeto Godot inicial commitado
- Renderer **Forward Plus** (Avançado+)
- `.gitignore` Godot 4 + C#/.NET
- `.editorconfig` / `.gitattributes`
- Ícone padrão `icon.svg`

### Nesta sessão (Grok) — ainda **não** em `main` até o PR ser aceito

- Conectado ao GitHub como `azhumberty`
- Clone local atualizado a partir de `origin/main`
- Criado este `HANDOFF.md` para continuidade entre IAs
- **Nenhuma** cena, script C#, pasta `Game/` ou documentação de etapa 0/1 foi criada
- `project.godot` **não** foi alterado

### Fonte de verdade de design (chat, ainda não versionada)

O MASTER HANDOFF completo (80 seções) foi entregue no chat e **ainda não está** como `MASTER_CONTEXT.md` no repo. A Etapa 0 deve gravá-lo. Resumo operacional abaixo.

---

## 2. Como funciona o estado atual

Projeto Godot 4.7 recém-criado, sem cenas nem código de gameplay.

| Item | Valor |
|---|---|
| Nome | Mystery Absolut |
| Pasta / repo | `mystery-absolut` |
| Engine | Godot **4.7.2.stable.mono** (features no `project.godot`: `4.7`) |
| Linguagem | C# / .NET (intenção). Há `[dotnet] project/assembly_name="Mystery Absolut"` |
| Renderer | Forward Plus (Avançado+) |
| Stretch | `canvas_items` / `expand` |
| Physics | Jolt (3D default do editor; o jogo é 2D) |
| Windows rendering device | `d3d12` |
| Cena principal | **não definida** |
| `.csproj` | **ausente** no GitHub |
| Scripts C# | nenhum |
| Cenas `.tscn` | nenhuma |

Identidade do jogo (não implementar agora):

> Action RPG 2D top-down com combate rápido e responsivo, personagens/companheiros com relações e escolhas reais, e itemização profunda (bases → instâncias → raridade → affixes → crafting, loot no chão, stash, loot filter).

Pilares de design: Secrets of Grindea (combate) + Fire Emblem (relações) + Path of Exile (itemização). Sem copiar IP.

---

## 3. Arquivos no repositório (`main`)

```text
.editorconfig
.gitattributes
.gitignore
icon.svg
icon.svg.import
project.godot
```

Esta branch adiciona:

```text
HANDOFF.md
```

Arquivos **não** existentes (esperados na Etapa 0 / 1):

```text
MASTER_CONTEXT.md
ARCHITECTURE.md
ROADMAP.md
CURRENT_STATUS.md
*.csproj
Game/
Actors/Player/
World/
Scenes/
Data/
```

---

## 4. Testes executados e resultados

| Teste | Resultado |
|---|---|
| `git fetch` + `git pull origin main` | OK — `main` em `6076568` |
| Leitura de `project.godot`, `.gitignore`, árvore do repo | OK |
| Abrir o projeto no Godot 4.7.2 Mono | **não executado** (este ambiente não tem Godot) |
| Compilação C# | **não executada** (sem `.csproj` / sem scripts) |
| Rodar o jogo / movimento / colisão | **não executado** |

Nada de gameplay foi testado. Não marcar Etapa 0 ou 1 como concluídas.

---

## 5. Pendências

1. Merge deste PR para `HANDOFF.md` ficar na `main`
2. **Etapa 0 — Fundação** (quando o owner pedir): `MASTER_CONTEXT.md`, `ARCHITECTURE.md`, `ROADMAP.md`, `CURRENT_STATUS.md`, estrutura mínima `Game/ Actors/ Player/ World/ UI/ Scenes/ Data/`, bootstrap Main → TestWorld → Player, garantir projeto C# (`.csproj` se o Godot ainda não gerou)
3. **Etapa 1 — Player Movement:** `CharacterBody2D`, 8 direções com diagonal normalizada, colisão, câmera, mapa placeholder, inputs `move_*` (pode *nomear* `attack` / `skill_1` / `dodge` / `block` / `interact` / `inventory` sem implementar)
4. Só depois: combate, loot, inventory, crafting, companions, diálogo

---

## 6. Erros / riscos conhecidos

- `project.godot` features = `"4.7", "Forward Plus"` — **não lista `"C#"`**. Há bloco `[dotnet]`, mas não há `.csproj` no Git. Confirmar no editor local se o projeto C# está realmente habilitado antes de escrever scripts.
- Este ambiente (Grok) **não consegue** abrir/compilar Godot. Quem implementar Etapa 0+1 precisa compilar no Godot 4.7.2 Mono local (`C:\Dev\mystery-absolut`) e reportar o resultado real no HANDOFF.
- Não implementar 10 sistemas futuros. Sem God Classes. PlayerController = input + movimento apenas.
- Nomes de orbs estilo PoE são **temporários** e só em dados, nunca `if (itemName == "Chaos Orb")`.

---

## 7. Branch / commit

| | |
|---|---|
| Fonte de verdade | GitHub `azhumberty/mystery-absolut` |
| `main` no início desta tarefa | `60765685040f77ff1729a8a4252efce90de45f03` — *Initial Godot project* |
| Branch desta mudança | `docs/handoff` |
| Remote local do owner | `C:\Dev\mystery-absolut` — mudanças não enviadas **não** aparecem no GitHub |

Fluxo obrigatório: branch → commit → push → **PR para revisão** → só então `main`. Nunca commit direto na `main` por IA.

Antes de cada tarefa: `git fetch` + ler a `main` mais recente.

---

## 8. Regras permanentes (todas as IAs)

1. GitHub é a fonte de verdade. Pull / ler `main` antes de mexer. Nunca sobrescrever sem entender.
2. Uma etapa por vez. Compilar. Testar. 0 erros. Commit. **STOP.**
3. Código cria sistemas. Dados criam conteúdo.
4. Preservar `project.godot` e configs existentes, salvo mudança explícita da etapa.
5. Placeholders visuais. Sem arte final agora.
6. Sem mundo gigante, multiplayer, skill tree, romance completo, IA generativa in-game.
7. Atualizar **este** `HANDOFF.md` durante trabalho longo e **sempre** ao parar. Manter na raiz.
8. Documentar no código decisões não óbvias.
9. Nunca declarar concluído/testado o que não foi.

Papéis sugeridos: GPT = arquitetura/design; Codex = C#/Godot/implementação; Antigravity/Claude/Gemini = sistemas maiores/UI; Grok = narrativa/quests/revisão criativa — sem várias IAs implementando a mesma coisa.

---

## 9. Prompt pronto para a próxima IA

```text
Você está no repositório https://github.com/azhumberty/mystery-absolut
Fonte de verdade = GitHub. Antes de qualquer edição: git fetch e leia a main mais recente.
Leia HANDOFF.md na raiz (obrigatório, continuidade entre IAs). Se existirem, leia também MASTER_CONTEXT.md, ARCHITECTURE.md, ROADMAP.md, CURRENT_STATUS.md.

Projeto: Mystery Absolut — Godot 4.7.2.stable.mono, C#, renderer Forward Plus.
Identidade: Action RPG 2D top-down (combate estilo Grindea + relações estilo Fire Emblem + itemização estilo PoE). Não copiar IP.

Estado no último HANDOFF:
- main estava em 6076568 (Initial Godot project) + PR docs/handoff com este arquivo
- Sem cenas, sem C# ainda, sem MASTER_CONTEXT.md
- Etapa 0 e 1 AINDA NÃO feitas
- project.godot não deve ser reescrito; features atuais 4.7 + Forward Plus; [dotnet] assembly_name já existe
- Confirmar se o .csproj C# existe no editor local (não está no Git)

Regras:
- Trabalhe SOMENTE na etapa que o owner pediu nesta mensagem
- Branch separada. PR para revisão. Nunca push direto na main
- Ao final (ou se o limite de uso estiver perto): ponto seguro, 0 erros de build, atualize HANDOFF.md na raiz com: o que foi feito, como funciona, arquivos, testes reais e resultados, pendências, erros conhecidos, branch/commit, prompt para a próxima IA. STOP. Não comece a etapa seguinte.

Se o owner ainda não pediu implementação: não implemente. Confirme o HANDOFF e aguarde.
Se o owner pediu a primeira execução: SOMENTE Etapa 0 + Etapa 1 (fundação + movimento 8 direções, CharacterBody2D, colisão, câmera, TestWorld placeholder, docs). Critérios no MASTER HANDOFF seções 65–72. Nada de combate, loot, inventory, companions.
```

---

## 10. Próximo passo (owner)

Aguardando o pedido explícito de **Etapa 0 + Etapa 1**. Sem esse pedido, nenhuma IA deve começar o gameplay.

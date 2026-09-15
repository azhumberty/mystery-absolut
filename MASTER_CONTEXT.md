# MASTER_CONTEXT.md — Mystery Absolut

Visão permanente do projeto. Qualquer IA que for trabalhar aqui deve ler
este arquivo (e ARCHITECTURE.md, ROADMAP.md, CURRENT_STATUS.md) antes de
tocar em qualquer código. O contexto completo original (decisões, exemplos
detalhados de cada sistema) está preservado como "MASTER_HANDOFF" no
projeto Claude vinculado a este repositório; este arquivo é o resumo que
vive junto do código.

## Identidade

Mystery Absolut é um Action RPG 2D top-down com combate rápido e
responsivo, personagens e companheiros com relações e escolhas realmente
importantes, e itemização profunda baseada em bases de itens, affixes,
crafting, loot no chão, stash e um Loot Filter altamente configurável.

Fusão conceitual de **Secrets of Grindea** (combate/exploração) +
**Fire Emblem** (personagens/relacionamentos/consequências) +
**Path of Exile** (itemização/crafting/loot). Usamos apenas princípios de
game design — nunca copiar personagens, sprites, mapas, código, diálogos
ou assets proprietários dessas obras.

## Engine e stack

- Godot 4.7.2 (stable, Mono)
- C# / .NET (net8.0), via `Godot.NET.Sdk`
- Renderer: Forward Plus ("Avançado+"), alvo PC/Desktop
- Física: Jolt Physics
- GitHub é a fonte oficial de verdade do projeto: https://github.com/azhumberty/mystery-absolut

## Três pilares

1. **Action RPG** — top-down 2D, 8 direções, combate em tempo real
   (ataques, skills, mana/recurso, dodge, block, parry, stagger,
   knockback), inimigos com padrões, bosses, exploração, dungeons, loot,
   equipamentos, progressão. Combate deve ser responsivo, rápido, preciso,
   fluido.
2. **Personagens** — aliados, rivais, companheiros e romances opcionais
   com personalidade, história, afinidade e memórias. Escolhas devem ter
   consequências reais (afinidade, quests, recrutamento, mortes, finais) —
   nunca falsa escolha. Companheiros participam do combate (HP, stats,
   nível, skills, equipamento, AI própria, proficiência por arma) e alguns
   concedem benefícios de sistema (ferreiro melhora crafting, alquimista
   consumíveis, caçador drops, mercador preços).
3. **Itemização** — ItemBase → ItemInstance → Rarity → Affixes → Crafting.
   Raridades iniciais Normal/Magic/Rare/Unique, arquitetura extensível para
   Relic/Ancient/Mythic/Corrupted sem reescrever. ItemLevel limita affixes
   disponíveis. Crafting nunca depende de nome do item
   (`if (itemName == "Chaos Orb")` é proibido) — sempre de
   `CraftingCurrencyDefinition` → `CraftingEffect`, para poder trocar nomes
   temporários (Orb of Transmutation, Chaos Orb, etc. — placeholders, a
   substituir antes de versão comercial) sem tocar em lógica.

## Regra central: Data-Driven

> Código cria sistemas. Dados criam conteúdo.

Item Bases, Affixes, Currencies, Enemies, Skills, Companions, Dialogue,
Quests, Drop Tables, Loot Filters e Crafting devem viver em
Resources/JSON/dados externos sempre que possível. Evitar hardcode.

## Princípios de código (em ordem de prioridade)

1. Simplicidade
2. Legibilidade
3. Modularidade
4. Baixo acoplamento
5. Data-driven
6. Expansão fácil
7. Manutenção fácil por diferentes IAs
8. Compilação funcionando

Evitar: God Classes, dependências circulares, abstração excessiva,
otimização prematura, duplicação, singletons sem necessidade.

## Fluxo de trabalho obrigatório

```
pequena tarefa → implementar → compilar → testar → 0 erros → commit → push → PARAR
```

- Nunca deixar o projeto com o build quebrado. Se uma feature não puder ser
  terminada, manter só a parte segura e documentar a pendência.
- Não antecipar sistemas futuros (multiplayer, mundo gigante, economia
  online, skill tree gigante, IA generativa, romance completo, centenas de
  itens — tudo isso é fora de escopo por enquanto).
- Ao final de cada etapa: atualizar CURRENT_STATUS.md e PARAR. Não emendar
  a próxima etapa automaticamente.
- GitHub é a fonte de verdade: verificar a versão mais recente antes de
  começar, nunca sobrescrever mudanças sem entender sua finalidade.

## Regra obrigatória de continuidade entre IAs

Este projeto é trabalhado por múltiplas IAs diferentes (Grok, Gemini,
Codex, Claude, e outras que vierem depois), cada uma sem memória das
sessões anteriores e sem visibilidade de quanto do seu próprio limite de
uso ainda resta. Por isso, a regra abaixo é **obrigatória e não
opcional**, e vale para qualquer IA, independentemente de qual etapa está
fazendo:

- **Nunca presuma que consegue monitorar seus próprios créditos/limite de
  uso.** Ao concluir uma etapa, ao encerrar uma sessão, ou ao perceber
  qualquer indicação de que o limite está próximo, **pare em um ponto
  seguro** (projeto compilando, ou pelo menos sem estado quebrado
  parcialmente commitado) em vez de tentar terminar tudo de uma vez.
- Antes de parar: **salve as alterações** (commit local no mínimo; push se
  tiver permissão) e **atualize `HANDOFF.md`** com uma nova entrada no
  topo contendo, no mínimo: o que foi feito, como funciona, quais arquivos
  foram alterados, quais testes foram executados e o resultado real de
  cada um, pendências, erros conhecidos, a branch/commit atual, e um
  prompt pronto (self-contained) para a próxima IA continuar de onde
  parou.
- Durante trabalhos longos (uma etapa que leva muitas interações), manter
  esse registro **atualizado ao longo do caminho**, não só no final — para
  que uma interrupção inesperada (queda de sessão, limite atingido no meio
  do trabalho) não perca contexto.
- **Documentar no código** (comentários, docstrings/XML doc) as decisões
  não óbvias — o "porquê", não o "o quê". Se uma escolha de implementação
  evita um problema específico ou existe por uma razão que não é óbvia
  olhando só o código, isso precisa estar comentado ali, além de no
  ARCHITECTURE.md.
- **Nunca declarar algo como concluído, testado ou compilando sem erros se
  isso não foi de fato executado e observado nesta sessão.** Se não foi
  possível testar (ex: ambiente sem o Godot/dotnet instalado), isso deve
  ser dito explicitamente como pendência — nunca omitido nem assumido como
  "provavelmente funciona".
- `CURRENT_STATUS.md` continua sendo o resumo curto e sempre atualizado do
  estado atual do projeto. `HANDOFF.md` é o log completo, append-only, com
  o histórico de cada ponto de parada e o porquê de cada decisão — os dois
  se complementam, nenhum substitui o outro.

## Instrução para qualquer nova IA entrando no projeto

1. Leia este documento, ARCHITECTURE.md, ROADMAP.md, CURRENT_STATUS.md e
   HANDOFF.md (entrada mais recente primeiro).
2. Não reinvente a arquitetura.
3. Verifique o estado atual do Git/repositório antes de mexer.
4. Trabalhe somente na etapa explicitamente solicitada.
5. Não implemente sistemas futuros antecipadamente.
6. Preserve código funcional.
7. Compile e teste antes de declarar concluído.
8. Deixe zero erros.
9. Documente o handoff (CURRENT_STATUS.md e nova entrada em HANDOFF.md —
   ver "Regra obrigatória de continuidade entre IAs" acima).
10. Pare.

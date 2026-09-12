---
name: publicar-treino-icu
description: Publica um planejamento de treino (arquivo *-planning.md gerado pelo skill treino-ciclismo) como eventos/treinos planejados no calendário do Intervals.ICU via MCP. Use quando o atleta pedir para "criar os treinos no Intervals.ICU", "publicar o planejamento", "subir o plano pro calendário" ou equivalente, depois que já existe um planning.md pronto em ./treinos.
---

# Publicar planejamento no Intervals.ICU

Este skill roda na conversa principal porque envolve confirmar com o atleta antes de escrever no calendário dele (ação em sistema externo/visível) — a publicação em si é delegada ao agente `intervals-icu-publisher`.

## Passo 1 — Escolher o arquivo

- Se o atleta já indicou qual `planning.md`, use-o. Caso contrário, rode `ls ./treinos/*-planning.md` e pegue o mais recente (maior data final no nome do arquivo).
- Confirme com o atleta qual arquivo e qual período (datas no nome do arquivo) serão publicados, se houver ambiguidade (mais de um planning.md recente).

## Passo 2 — Perguntar o que falta para publicar

O `planning.md` não tem horário do dia, só data. Pergunte ao atleta (ou proponha um padrão e peça confirmação):
- Horário padrão para as sessões de dia útil (padrão sugerido: 18:00, já que o histórico do atleta mostra pedais depois do trabalho).
- Horário padrão para as sessões de fim de semana (padrão sugerido: 07:00).
- Se já existirem eventos no Intervals.ICU nesse período (você pode checar com `intervals_list_events` antes de perguntar, para já informar se há conflito): perguntar se deve sobrepor (apagar e recriar) ou pular os dias em conflito.

## Passo 3 — Confirmar antes de escrever

Mostre um resumo rápido do que será criado (quantas sessões, período, horários) e peça confirmação explícita do atleta antes de publicar — escrever no calendário do Intervals.ICU é uma ação em sistema externo, não deve ser feita sem essa confirmação.

## Passo 4 — Delegar a publicação

Invoque o agente `intervals-icu-publisher` (Agent tool, subagent_type "intervals-icu-publisher") passando: caminho do `planning.md`, datas de início/fim do período, horário padrão de dia útil, horário padrão de fim de semana, e a decisão sobre conflitos (sobrepor ou pular). Ele lê o arquivo, publica os eventos via MCP e devolve um resumo do que foi criado/pulado/apagado.

## Passo 5 — Reportar ao atleta

Repasse ao atleta o resumo devolvido pelo agente: quantos treinos foram criados, quais dias ficaram de fora e por quê, e o link/nome dos eventos se relevante. Não é necessário criar nenhum arquivo novo neste skill — a fonte de verdade continua sendo o `planning.md` em `./treinos`; o Intervals.ICU é só o espelho operacional dele.

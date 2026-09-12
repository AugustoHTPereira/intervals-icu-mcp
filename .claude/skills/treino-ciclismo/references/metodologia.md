# Metodologia de referência

## Métricas do Intervals.ICU usadas

- **icu_training_load / TRIMP**: carga da sessão. Some no período para ter a carga total do bloco.
- **CTL (Chronic Training Load)**: "fitness" — média móvel de longo prazo (~42 dias) da carga. Deve crescer devagar e de forma sustentada.
- **ATL (Acute Training Load)**: "fadiga" — média móvel curta (~7 dias) da carga. Sobe rápido com blocos puxados.
- **TSB/form (CTL - ATL)**: equilíbrio forma/fadiga. Muito negativo (< -15/-20) por muitos dias seguidos = risco de overreaching. Perto de zero ou levemente positivo = bom momento para provas ou testes.
- Como o atleta **não tem potência confiável** (sem medidor de watts), a referência de intensidade é sempre por **frequência cardíaca** (zonas de FC, `icu_hr_zone_times`) e percepção subjetiva (RPE/feel), nunca watts.

## Como ler o conjunto (dados objetivos + percepção)

| Dados objetivos | Percepção do atleta | Decisão do treinador |
|---|---|---|
| TSB estável/OK, sem alertas, cumprimento normal | "foi puxado" | Manter ou progredir levemente — a percepção isolada não basta para recuar |
| TSB muito negativo, FC repouso subindo, alertas de fadiga | "foi tranquilo" | Recuar carga — sinais fisiológicos pesam mais que a sensação pontual |
| TSB muito negativo + atleta relata fadiga/dor | qualquer | Recuar carga, priorizar recuperação |
| TSB neutro/positivo, cumprimento bom, atleta motivado | "tranquilo/ok" | Pode progredir carga (volume ou intensidade, não os dois ao mesmo tempo) |

Regra geral do treinador (fixada pelo atleta): quando a percepção subjetiva e os dados objetivos divergem, **prevalece a leitura do treinador sobre os dados**, e isso deve ficar documentado no `-analisys.md`, nunca escondido.

## Estrutura de quinzena dado o perfil do atleta

- Região montanhosa + só MTB em terra: todo treino tem elevação; não existe "rolado plano". Controle a intensidade pela FC-alvo e pelo tempo, não pela topografia.
- Semana útil (seg-sex, ~1.5h/dia): boa para trabalho de base (Z1-Z2), técnica de subida, e no máximo 1-2 sessões curtas de intensidade (intervalos em subida) por semana — nunca duas sessões duras seguidas.
- Fim de semana (sáb/dom, 4-5h): pedal em grupo, geralmente a sessão-âncora de volume/resistência da semana e naturalmente mais intenso por ser social/competitivo — considere isso como já sendo o estímulo forte da semana; não empilhe outro treino duro na sexta-feira antes.
- Progressão: aumente carga (CTL) em degraus de ~1-2 semanas de progressão seguidas por 1 semana mais leve (recuperação), especialmente a cada 3-4 semanas — o skill trabalha em blocos de 2 semanas, então alterne "quinzena de progressão" com "quinzena de consolidação/recuperação" conforme o histórico dos ciclos anteriores.
- Eventos/provas informados pelo atleta: se há uma prova na quinzena ou logo depois, priorize TSB neutro/positivo perto da data (reduza carga nos 3-5 dias anteriores).

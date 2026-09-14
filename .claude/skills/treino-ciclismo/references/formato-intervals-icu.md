# Formato de treino estruturado do Intervals.ICU

Referência: https://zonepace.cc/intervals-workout-format

Use este formato para escrever o bloco de conferência de cada sessão no `planning.md` (seção "Sessões no formato Intervals.ICU"). É o mesmo texto que o agente `intervals-icu-publisher` vai colar no campo `description` do evento — escrevendo certo aqui, a publicação fica 1:1, sem reinterpretação.

## Regras

- Linhas que começam com `-` são **steps**: `- <duração> <zona de FC>`.
- Duração: `1h`, `10m`, `30s`, ou combinações (`1m30`). (`m` = minutos, não metros.)
- **Zona de FC exige o sufixo `HR`**, sempre. Este atleta não tem medidor de potência — todo alvo é por FC.
  - Zona única: `Z2 HR`.
  - Faixa: `Z1-Z2 HR`, `Z2-Z3 HR`.
  - Nunca escreva `Z1-Z2` sem `HR` — sem o sufixo, o Intervals.ICU interpreta como zona de **potência** (watts), o que é errado para este atleta.
- Repetições: cabeçalho de bloco com `NxM`, ex. `Subidas 3x`, seguido das linhas `-` que repetem (todas as linhas daquele bloco até o próximo cabeçalho).
- Linhas sem `-` (cabeçalhos como `Aquecimento`, `Subidas 3x`, `Volta`, ou texto solto) viram nota/contexto, não step.
- A soma das durações dos steps deve bater exatamente com a duração total da sessão.

## Quando usar repetição (obrigatório para picos/esforço forte)

Qualquer sessão com picos de intensidade mais alta (Z4+, "dia forte", esforço em subida) **nunca** deve ter o pico como bloco contínuo longo — sempre picos curtos (~2-3min) intercalados com recuperação em zona mais baixa, com aquecimento antes e volta depois. Nunca gere um step único de Z4+ com mais de ~3min.

## Exemplo — sessão de zona única (fácil/endurance sem picos)

```
Pedal fácil, manter ritmo controlado mesmo nas subidas.

Sessão
- 1h Z1-Z2 HR
```

## Exemplo — sessão opcional/recuperação muito leve

```
Opcional, só se o corpo pedir.

Sessão
- 45m Z1 HR
```

## Exemplo — "dia forte" com picos em subida (1h no total)

```
Dia forte da semana — só este, não empilhar com o fim de semana. Picos de Z4 curtos e intercalados nas subidas, não contínuos.

Aquecimento
- 15m Z1-Z2 HR

Subidas 3x
- 3m Z4 HR
- 7m Z2-Z3 HR

Volta
- 15m Z2-Z3 HR
```

(15 + 3×10 + 15 = 60min — a soma bate com a duração total da sessão.)

Progressão entre semanas de um bloco de "dia forte" deve vir de **mais repetições** (ou levemente mais tempo de esforço por repetição), não de um pico contínuo mais longo.

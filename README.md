# Intervals.ICU MCP Server

Servidor [MCP](https://modelcontextprotocol.io) para a plataforma [Intervals.ICU](https://intervals.icu),
construído em **.NET 10** e distribuído via **Docker**. Expõe dados de um atleta (perfil, wellness,
atividades, curvas de potência/pace/FC, zonas de treino) e permite escrever no calendário (eventos e
treinos planejados) para que um agente de IA possa atuar como treinador, sugerindo e agendando treinos.

- Documentação da API: https://intervals.icu/api-docs.html
- Spec completa: [`openapi-spec.json`](./openapi-spec.json) (118 endpoints)

## Arquitetura

```
IntervalsMcp.sln
├── src/IntervalsIcu.Client   Cliente HTTP tipado para a API do Intervals.ICU (Basic Auth,
│                             passthrough de JSON, sem DTOs por endpoint dado o tamanho do schema)
└── src/IntervalsIcu.Mcp      Host ASP.NET Core que expõe as tools via MCP (Streamable HTTP)
```

### Cobertura da API

A API do Intervals.ICU tem ~118 endpoints. Este servidor prioriza tools dedicadas e bem
documentadas para as áreas centrais de um agente treinador:

| Área | Tools |
|---|---|
| Atleta | `intervals_get_athlete`, `intervals_update_athlete`, `intervals_get_athlete_profile`, `intervals_get_athlete_summary`, `intervals_list_athletes` |
| Saúde/Wellness | `intervals_get_wellness`, `intervals_list_wellness`, `intervals_upsert_wellness`, `intervals_bulk_upsert_wellness` |
| Atividades | `intervals_list_activities`, `intervals_search_activities`, `intervals_get_activity`, `intervals_get_activity_intervals`, `intervals_get_activity_best_efforts`, `intervals_get_activity_streams`, `intervals_get_power_curve`, `intervals_get_pace_curve`, `intervals_get_hr_curve` |
| Calendário/Eventos | `intervals_list_events`, `intervals_get_event`, `intervals_create_event`, `intervals_update_event`, `intervals_delete_event`, `intervals_bulk_create_events` |
| Biblioteca de treinos | `intervals_list_workouts`, `intervals_get_workout`, `intervals_create_workout`, `intervals_update_workout`, `intervals_delete_workout`, `intervals_list_workout_folders` |
| Zonas/FTP | `intervals_list_sport_settings`, `intervals_get_sport_settings`, `intervals_update_sport_settings` |

Para os demais endpoints (chat, equipamento, clima, rotas, custom items, export GPX/FIT, etc.) há
uma tool genérica de fallback:

- **`intervals_raw_request(method, path, queryJson, bodyJson)`** — chama qualquer endpoint listado
  em `openapi-spec.json` diretamente. O agente deve consultar o spec para montar path/params/body.

Isso cobre 100% da superfície da API sem precisar de ~118 tools individuais.

## Autenticação com o Intervals.ICU

O servidor usa **API Key** (não OAuth): Basic Auth com usuário `API_KEY` e senha = chave da API do
atleta, encontrada em **Intervals.ICU → Settings → Developer Settings**.

O id de atleta pode ser omitido nas tools (default `"0"`, que o Intervals.ICU interpreta como "o
atleta dono da API key").

## Configuração (variáveis de ambiente)

| Variável | Obrigatória | Default | Descrição |
|---|---|---|---|
| `INTERVALS_API_KEY` | Sim | — | API key do atleta (Settings → Developer Settings) |
| `INTERVALS_DEFAULT_ATHLETE_ID` | Não | `0` | Id de atleta padrão quando a tool não especifica um |
| `INTERVALS_BASE_URL` | Não | `https://intervals.icu` | Base URL da API (não deveria precisar mudar) |
| `PORT` | Não | `8080` | Porta HTTP em que o servidor escuta |

## Rodando localmente (sem Docker)

Requer .NET 10 SDK.

```bash
export INTERVALS_API_KEY="sua-api-key"
dotnet run --project src/IntervalsIcu.Mcp
```

O servidor sobe em `http://localhost:8080`:
- `GET /` — health check
- `POST /mcp` — endpoint MCP (Streamable HTTP)

## Rodando com Docker

```bash
docker build -t intervals-icu-mcp:latest .
docker run -d --name intervals-icu-mcp \
  -p 8080:8080 \
  -e INTERVALS_API_KEY="sua-api-key" \
  --restart unless-stopped \
  intervals-icu-mcp:latest
```

Ou com Docker Compose (cria um `.env` na raiz do projeto com `INTERVALS_API_KEY=...`):

```bash
echo "INTERVALS_API_KEY=sua-api-key" > .env
docker compose up -d --build
```

## Publicando no home-lab (192.168.3.10)

1. Copie o repositório (ou faça `git clone`) para o host `192.168.3.10`.
2. Crie o arquivo `.env` com `INTERVALS_API_KEY` (e opcionalmente `INTERVALS_DEFAULT_ATHLETE_ID`).
3. Suba o serviço:
   ```bash
   docker compose up -d --build
   ```
4. O servidor MCP ficará acessível em `http://192.168.3.10:8080/mcp` para qualquer máquina da
   rede local.

> **Segurança:** por decisão deliberada, este servidor **não** adiciona uma camada extra de
> autenticação (bearer token) na frente do endpoint MCP — ele confia na rede local do home-lab.
> Não exponha a porta 8080 diretamente à internet (sem VPN/reverse proxy com autenticação) sem
> antes adicionar essa camada.

## Conectando o Claude Code ao servidor

Do lado do cliente (sua máquina, não o home-lab), registre o servidor remoto:

```bash
claude mcp add --transport http intervals-icu http://192.168.3.10:8080/mcp
```

Depois disso, as tools `intervals_*` ficam disponíveis nas sessões do Claude Code. Peça, por
exemplo: *"veja meu wellness dos últimos 14 dias e sugira um treino de bike para amanhã"* — o
Claude vai combinar `intervals_list_wellness`, `intervals_list_sport_settings` e
`intervals_create_event` para propor e agendar o treino.

## Desenvolvimento

```bash
dotnet build                 # build da solução inteira
dotnet run --project src/IntervalsIcu.Mcp   # rodar localmente
```

Para adicionar uma nova tool dedicada (em vez de usar `intervals_raw_request`), crie um método
estático com `[McpServerTool]` em uma classe `[McpServerToolType]` dentro de
`src/IntervalsIcu.Mcp/Tools/`, injetando `IntervalsClient` como parâmetro — ele é resolvido
automaticamente via DI pelo SDK do MCP.

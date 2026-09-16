#!/bin/bash
# Instala o agendamento (cron) que dispara run.sh todo dia às 08:00.
# Só mexe no crontab do usuário atual — não toca em Docker/containers MCP.
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
RUN_SH="$SCRIPT_DIR/run.sh"
MARKER="# intervals-daily-notes (managed by install.sh)"
CRON_LINE="0 8 * * * $RUN_SH $MARKER"

chmod +x "$RUN_SH"

# Remove qualquer entrada anterior com o mesmo marker e adiciona a atual (idempotente).
( crontab -l 2>/dev/null | grep -vF "$MARKER" || true; echo "$CRON_LINE" ) | crontab -

echo "Cron instalado: todo dia às 08:00 executando $RUN_SH"
echo "Verifique com: crontab -l"
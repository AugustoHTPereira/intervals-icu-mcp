#!/bin/bash
# Remove a entrada de cron instalada por install.sh.
# Só mexe no crontab do usuário atual — não toca em Docker/containers MCP.
set -euo pipefail

MARKER="# intervals-daily-notes (managed by install.sh)"

if crontab -l 2>/dev/null | grep -qF "$MARKER"; then
  crontab -l 2>/dev/null | grep -vF "$MARKER" | crontab -
  echo "Cron removido."
else
  echo "Nenhuma entrada de cron encontrada (nada a fazer)."
fi

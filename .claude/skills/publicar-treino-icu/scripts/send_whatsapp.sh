#!/usr/bin/env bash
# Envia uma mensagem de texto via WhatsApp usando a API do CallMeBot.
# Lê CALLMEBOT_PHONE e CALLMEBOT_API_KEY do .env na raiz do projeto.
#
# Uso: send_whatsapp.sh "<texto da mensagem>"

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../../../.." && pwd)"
ENV_FILE="$PROJECT_ROOT/.env"

if [ -f "$ENV_FILE" ]; then
  set -a
  # shellcheck disable=SC1090
  source "$ENV_FILE"
  set +a
fi

if [ -z "${CALLMEBOT_PHONE:-}" ] || [ -z "${CALLMEBOT_API_KEY:-}" ]; then
  echo "Erro: defina CALLMEBOT_PHONE e CALLMEBOT_API_KEY em $ENV_FILE" >&2
  exit 1
fi

if [ $# -lt 1 ]; then
  echo "Uso: $0 \"<texto da mensagem>\"" >&2
  exit 1
fi

TEXT="$1"

RESPONSE=$(curl -s -G "https://api.callmebot.com/whatsapp.php" \
  --data-urlencode "phone=${CALLMEBOT_PHONE}" \
  --data-urlencode "apikey=${CALLMEBOT_API_KEY}" \
  --data-urlencode "text=${TEXT}")

echo "$RESPONSE"

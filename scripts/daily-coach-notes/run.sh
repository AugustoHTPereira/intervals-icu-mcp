#!/bin/bash
# Roda o Claude Code em modo headless para comentar atividades recentes no Intervals.ICU.
# Disparado por launchd (ver scripts/daily-coach-notes/com.augustohtp.intervals-daily-notes.plist).
set -euo pipefail

REPO_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
PROMPT_FILE="$REPO_DIR/scripts/daily-coach-notes/prompt.md"
LOG_DIR="$REPO_DIR/scripts/daily-coach-notes/logs"
mkdir -p "$LOG_DIR"
LOG_FILE="$LOG_DIR/$(date +%Y-%m-%d).log"

cd "$REPO_DIR"

ALLOWED_TOOLS="mcp__intervals-icu__intervals_list_activities,mcp__intervals-icu__intervals_get_activity,mcp__intervals-icu__intervals_get_activity_intervals,mcp__intervals-icu__intervals_list_activity_comments,mcp__intervals-icu__intervals_add_activity_comment,mcp__intervals-icu__intervals_get_athlete_summary,mcp__intervals-icu__intervals_list_events,Bash(.claude/skills/publicar-treino-icu/scripts/send_whatsapp.sh:*)"

{
  echo "=== $(date '+%Y-%m-%d %H:%M:%S') ==="
  claude -p "$(cat "$PROMPT_FILE")" \
    --allowedTools "$ALLOWED_TOOLS" \
    --output-format text
  echo
} >> "$LOG_FILE" 2>&1

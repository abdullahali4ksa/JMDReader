#!/bin/bash
# Launch the app (optionally with a file) and monitor for 180s, reporting exactly when/if it dies.
set -u
cd /d/AI_PROJECTS/MDReader || exit 2
FILE="${1:-}"
MODE="${2:-detached}"   # detached | dotnetrun

taskkill /IM MDReader.App.exe /F >/dev/null 2>&1
sleep 2

if [ "$MODE" = "detached" ]; then
  # Launch the registered packaged app directly (detached from this shell) via its AUMID.
  powershell.exe -NoProfile -Command "Start-Process 'shell:appsFolder\\MDReader.MDReader_1z32rh13vfry6!App'" >/dev/null 2>&1
else
  if [ -n "$FILE" ]; then
    (dotnet run --project src/MDReader.App -- "$FILE" >/tmp/mdr_mon.log 2>&1 &)
  else
    (dotnet run --project src/MDReader.App >/tmp/mdr_mon.log 2>&1 &)
  fi
fi

# Wait for the process to appear
pid=""
for i in $(seq 1 20); do
  sleep 2
  line=$(tasklist /FI "IMAGENAME eq MDReader.App.exe" /NH 2>/dev/null | grep -i "MDReader.App.exe" | head -1)
  if [ -n "$line" ]; then
    pid=$(echo "$line" | awk '{print $2}')
    break
  fi
done

if [ -z "$pid" ]; then echo "NEVER_STARTED"; exit 1; fi
echo "started pid=$pid; monitoring 180s..."

start=$(date +%s)
while true; do
  now=$(date +%s); elapsed=$((now-start))
  if [ $elapsed -ge 180 ]; then echo "ALIVE_180s pid=$pid"; break; fi
  if ! tasklist /FI "IMAGENAME eq MDReader.App.exe" /NH 2>/dev/null | grep -qi "MDReader.App.exe"; then
    echo "DIED_AFTER_${elapsed}s pid=$pid"; break
  fi
  sleep 5
done

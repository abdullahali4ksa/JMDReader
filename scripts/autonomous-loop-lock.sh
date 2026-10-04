#!/bin/bash
# Overlap-prevention script for MDReader autonomous loop (P02-T002 and beyond)
LOCKFILE="/tmp/mdreader_autonomous.lock"
PID=""
if [ -f "$LOCKFILE" ]; then
    PID=$(cat "$LOCKFILE" 2>/dev/null)
    if [ -n "$PID" ] && kill -0 "$PID" 2>/dev/null; then
        echo "SKIP: previous MDReader autonomous loop running (PID $PID)"
        exit 0
    else
        rm -f "$LOCKFILE"
    fi
fi
echo $$ > "$LOCKFILE"
echo "LOCK: MDReader autonomous loop (PID $$)"
# The agent prompt handles the rest of the loop.
# Clean up when finished.
trap 'rm -f "$LOCKFILE"; echo "LOCK_RELEASE: loop finished"' EXIT

#!/usr/bin/env bash

set -euo pipefail

if [[ "$#" -ne 2 ]]; then
  echo "Usage: $0 START_DATE END_DATE"
  echo "Example: $0 2025-12-01 2025-12-10"
  exit 1
fi

START_DATE="$1"
END_DATE="$2"

# Validate input format (YYYY-MM-DD)
if ! [[ "$START_DATE" =~ ^[0-9]{4}-[0-9]{2}-[0-9]{2}$ ]]; then
  echo "Invalid START_DATE format. Use YYYY-MM-DD"
  exit 1
fi

if ! [[ "$END_DATE" =~ ^[0-9]{4}-[0-9]{2}-[0-9]{2}$ ]]; then
  echo "Invalid END_DATE format. Use YYYY-MM-DD"
  exit 1
fi

# Convert to epoch seconds (macOS + Linux)
start_epoch=$(date -d "$START_DATE" +%s 2>/dev/null || date -j -f "%Y-%m-%d" "$START_DATE" +%s)
end_epoch=$(date -d "$END_DATE"   +%s 2>/dev/null || date -j -f "%Y-%m-%d" "$END_DATE"   +%s)

if (( start_epoch > end_epoch )); then
  echo "Start date must be before or equal to end date"
  exit 1
fi

# Activate the virtual environment
export REPORTS_ROOT=$( cd "$( dirname "$0" )" && pwd )
. $REPORTS_ROOT/venv/bin/activate

export PYTHONWARNINGS="ignore"

# Store the current working directory so we can restore it and change to the folder
# containing the notebook
CURDIR=`pwd`
cd glucose

# Main loop (inclusive)
current_epoch="$start_epoch"
while (( current_epoch <= end_epoch )); do
    # Get the current date in YYYY-MM-DD format
    current_date=$(date -d "@$current_epoch" +%Y-%m-%d 2>/dev/null || date -j -f "%s" "$current_epoch" +%Y-%m-%d)

    # Run the glucose baseline analysis notebook, passing in the date
    papermill glucose_baseline.ipynb /dev/null -p date_param "$current_date"

    # Add one day to the current date
    current_epoch=$(( current_epoch + 86400 ))
done

# Restore the current working directory
cd "$CURDIR"

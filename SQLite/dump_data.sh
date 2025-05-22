#!/bin/sh -f

if [[ $# -ne 2 ]]; then
    echo "Usage: $0 DBPATH DUMPFILE"
    exit 1
fi

if [ ! -f "$1" ]; then
    echo "Database file not found"
fi

# Create an array containing the arguments - this is to ensure spaces in arguments don't cause
# issues with the Python script when the argements are passed to it
i=0
options=()
for arg in "$@"; do
    options[$i]="$arg"
    i=$((i + 1))
done

# Dump the data
python dump_table_data.py "${options[@]}"

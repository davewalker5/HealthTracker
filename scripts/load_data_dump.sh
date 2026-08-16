#!/bin/sh -f

if [[ $# -ne 2 ]]; then
    echo "Usage: $0 DBPATH DUMPFILE"
    exit 1
fi

if [ ! -f "$1" ]; then
    echo "Database file not found"
fi

if [ ! -f "$2" ]; then
    echo "Data dump file not found"
fi

echo
echo "DATABASE FILE : $1"
echo "DUMP FILE     : $2"
echo

sqlite3 "$1" < $2

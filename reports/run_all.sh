#!/bin/bash -f

# Get the root of the reporting folder
export REPORTS_ROOT=$( cd "$( dirname "$0" )" && pwd )

# If an (optional) "category" (i.e. report folder) has been specified, make sure
# it exists
REPORTS_FOLDER=""
if [[ $# -eq 1 ]]; then
    REPORTS_FOLDER="$REPORTS_ROOT/$1"
    if [ ! -d "$REPORTS_FOLDER" ]; then
        echo "Reports folder '$1' not found"
        exit 1
    fi
fi

# Activate the virtual environment
. $REPORTS_ROOT/venv/bin/activate

# Suppress warnings about the output file extension
export PYTHONWARNINGS="ignore"

# Define a list of notebooks to skip
declare -a exclusions=(
    "api.ipynb"
    "config.ipynb"
    "export.ipynb"
    "glucose_segment_plot.ipynb"
)

# Store the current working directory so we can restore it
CURDIR=`pwd`

# If specified, change to the folder containing the reports to run
if [ -n "$REPORTS_FOLDER" ]; then
    cd "$REPORTS_FOLDER"
fi

# Get a list of Jupyter Notebooks and iterate over them
files=$(find `pwd` -name '*.ipynb')
while IFS= read -r file; do
    # Get the notebook file name and extension without the path
    folder=$(dirname "$file")
    filename=$(basename -- "$file")

    # See if the notebook is in the exclusions list
    found=0
    if [[ " ${exclusions[@]} " =~ " $filename " ]]; then
        echo "Notebook $filename is in the exclusions list and will not be run"
        found=1
    fi

    # If this notebook isn't in the exclusions list, run it
    if [[ found -eq 0 ]]; then
        cd "$folder"
        papermill "$filename" /dev/null
    fi
done <<< "$files"

# Restore the current working directory
cd "$CURDIR"

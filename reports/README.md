# Health Tracker Reporting

This folder contains Jupyter notebooks and supporting files for reporting on health data stored in the Health Tracker SQLite database. The data is retrieved using the Health Tracker API.

The following reports are currently available:

| Notebook | Report Type |
| --- | --- |
| api.ipynb | Define methods for accessing the API |
| blood_pressure_assessment_over_time.ipynb | Stacked histogram of changing blood pressure assessment over time |
| blood_pressure_kde_plot.ipynb | KDE plots of systolic and diastolic blood pressure readings |
| blood_pressure_over_time.ipynb | Chart systolic and diastolic blood pressure readings over time |
| blood_pressure_trends.ipynb | Chart changes in minimum, maximum and mean blood pressure over time |
| glucose_agp.ipynb | Ambulatory Glucose Profile charts |
| systolic_diastolic_correlation.ipynb | Systolic/diastolic correlation chart |
| weight_calendar_heatmap.ipynb | Heatmap of weight and weight trends over time |
| weight_over_time.ipynb | Charts weight and BMI measurements over time |

## Setting Up the Reporting Environment

The reports have been written and tested using [Visual Studio Code](https://code.visualstudio.com/download) and the Jupyter extension from Microsoft using a Python virtual environment with the requirements listed in requirements.txt installed as the kernel for running the notebooks.

### Build the Virtual Environment

To build the virtual environment, run the following command:

```bash
./make_venv.sh
```

Or, in PowerShell:

```powershell
.\make_venv.bat
```

## Running a Report in Visual Studio Code

- Make sure the Health Tracker API is running
- Open the Jupyter notebook for the report of interest
- If using Visual Studio Code, select the Python virtual environment as the kernel for running the notebook
- Review the instructions at the top of the report and make any required changes to e.g. reporting parameters
- Click on "Run All" to run the report and export the results
- Exported results are written to a folder named "exported" within the reports folder

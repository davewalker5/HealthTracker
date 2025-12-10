# Health Tracker Reporting

This folder contains Jupyter notebooks and supporting files for reporting on health data stored in the Health Tracker SQLite database. The data is retrieved using the Health Tracker API.

The following reports are currently available:

| Folder         | Notebook                                  | Report Type                                                             |
| -------------- | ----------------------------------------- | ----------------------------------------------------------------------- |
| -              | api.ipynb                                 | Define methods for accessing the API                                    |
| -              | config.ipynb                              | Reporting session parameters, used by all the reports                   |
| -              | export.ipynb                              | Define methods for exporting the data                                   |
| blood_pressure | blood_pressure_assessment_over_time.ipynb | Stacked histogram of changing blood pressure assessment over time       |
| blood_pressure | blood_pressure_kde_plot.ipynb             | KDE plots of systolic and diastolic blood pressure readings             |
| blood_pressure | blood_pressure_over_time.ipynb            | Chart systolic and diastolic blood pressure readings over time          |
| blood_pressure | blood_pressure_trends.ipynb               | Chart changes in minimum, maximum and mean blood pressure over time     |
| blood_pressure | systolic_diastolic_correlation.ipynb      | Systolic/diastolic correlation chart                                    |
| glucose        | glucose_agp.ipynb                         | Ambulatory Glucose Profile charts                                       |
| glucose        | glucose_baseline.ipynb                    | Baseline analysis                                                       |
| glucose        | glucose_daily_mean.ipynb                  | Daily mean glucose levels with variability and in-range indicators      |
| glucose        | glucose_daily_mean_comparison.ipynb       | As "glucose_daily_mean.ipynb" but for two date ranges on a single chart |
| glucose        | glucose_heatmap.ipynb                     | Glucose level heatmap by time of day                                    |
| glucose        | glucose_histogram.ipynb                   | Glucose level distribution histograms and KDE charts                    |
| glucose        | glucose_metrics.ipynb                     | All time and daily glucose metrics                                      |
| glucose        | glucose_overlay.ipynb                     | Daily glucose overlay or "spaghetti" charts                             |
| glucose        | glucose_overlay_comparison.ipynb          | As "glucose_overlay.ipynb" but for two date ranges on a single chart    |
| glucose        | glucose_segment_plot.ipynb                | Chart a segment of data, delimited by two timestamps, as a line chart   |
| weight         | weight_calendar_heatmap.ipynb             | Heatmap of weight and weight trends over time                           |
| weight         | weight_over_time.ipynb                    | Charts weight and BMI measurements over time                            |

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

## Methods & Credits

This project uses standard scientific Python libraries for data processing and visualisation:

- pandas – data handling and time series processing  
- NumPy – numerical computation  
- statsmodels – LOWESS smoothing (locally weighted regression)  
- Matplotlib – visualisation  

Baseline detection is performed using a custom multi-stage signal-processing pipeline:

1. LOWESS smoothing of CGM glucose data  
2. First-derivative (slope) and curvature estimation  
3. Flat-segment detection  
4. Rolling lower-envelope filtering using quantiles  
5. Excursion-zone suppression based on steep slope regions  

This approach is designed to isolate true physiological baseline glucose while suppressing:
- Meal-related excursions  
- Exercise effects  
- Stress-related plateaus  

All implementation logic is original to this project.

## Related Concepts

The baseline detector is inspired by general principles from:
- Time-series denoising
- Envelope detection
- Change-point / excursion detection

However, no external algorithms are directly implemented beyond LOWESS smoothing.
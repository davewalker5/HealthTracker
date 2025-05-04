# healthtrackermvc

The [Health Tracker](https://github.com/davewalker5/HealthTracker) GitHub project implements an application for recording health metrics:

- Measurements
  - Blood glucose measurements
  - Blood oxygen saturation measurements
  - Blood pressure measurements
  - Cholesterol measurements
  - Exercise records
  - Weight measurements
- Medications
  - Medication definitions
  - Person/medication associations and medication tracking
- Reports
  - Flexible reporting using Jupyter Notebooks with data retrieval via the API
- Data Exchange via CSV import and export

The healthtrackermvc image contains a build of the UI and is intended to be run as a pair with the associated web service.

## Getting Started

### Prerequisities

In order to run this image you'll need docker installed.

- [Windows](https://docs.docker.com/windows/started)
- [OS X](https://docs.docker.com/mac/started/)
- [Linux](https://docs.docker.com/linux/started/)

### Usage

#### Service Container Parameters

An instance of the healthtrackerapisqlite image must be started first in order for the UI to work. The recommended parameters are:

| Parameter | Value                              | Purpose                                              |
| --------- | ---------------------------------- | ---------------------------------------------------- |
| -d        | -                                  | Run as a background process                          |
| -v        | /local:/var/opt/healthtracker.api | Mount the host folder containing the SQLite database |
| --name    | healthtrackerservice              | Name the service so the UI can find it               |
| --rm      | -                                  | Remove the container automatically when it stops     |

The "/local" path given to the -v argument is described, below, and should be replaced with a value appropriate for the host running the container.

The "--name" parameter is mandatory as the service URL is held in the application settings for the UI image and is expected to be:

http://healthtrackerservice:80

#### UI Container Parameters

The following "docker run" parameters are recommended when running the healthtrackermvc image:

| Parameter | Value                 | Purpose                                                 |
| --------- | --------------------- | ------------------------------------------------------- |
| -d        | -                     | Run as a background process                             |
| -p        | 5001:80               | Expose the container's port 80 as port 5001 on the host |
| --link    | healthtrackerservice  | Link to the health tracker service container            |
| --rm      | -                     | Remove the container automatically when it stops        |

For example:

```shell
docker run -d -p 5001:80 --rm --link healthtrackerservice davewalker5/healthtrackermvc:latest
```

The port number "5001" can be replaced with any available port on the host.

#### Volumes

The description of the container parameters, above, specifies that a folder containing the SQLite database file for the Health Tracker is mounted in the running container, using the "-v" parameter.

That folder should contain a SQLite database that has been created using the instructions in the [Health Tracker wiki](https://github.com/davewalker5/HealthTrackerDb/wiki).

Specifically, the following should be done:

- [Create the SQLite database](https://github.com/davewalker5/HealthTracker/wiki/Database)

The folder containing the "healthtracker.db" file can then be passed to the "docker run" command using the "-v" parameter.

#### Running the Image

To run the images for the service and UI, enter the following commands, substituting "/local" for the host folder containing the SQLite database, as described:

```shell
docker run -d -v  /local:/var/opt/healthtracker.api/ --name healthtrackerservice --rm  davewalker5/healthtrackerapisqlite:latest
docker run -d -p 5001:80 --rm --link healthtrackerservice davewalker5/healthtrackermvc:latest
```

Once the container is running, browse to the following URL on the host:

http://localhost:5001

You should see the login page for the UI.

## Find Us

- [HealthTracker on GitHub](https://github.com/davewalker5/HealthTracker)

## Versioning

For the versions available, see the [tags on this repository](https://github.com/davewalker5/HealthTracker/tags).

## Authors

- **Dave Walker** - _Initial work_

See also the list of [contributors](https://github.com/davewalker5/HealthTracker/contributors) who
participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/davewalker5/HealthTracker/blob/master/LICENSE) file for details.

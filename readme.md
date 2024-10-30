
# DCC Connect Backend
_NOTE:_ Currently this readme does not support development on linux, gonna have to figure out that yourself.
## Description
This is the backend for the DCC Connect project.
It is a RESTful API that is built using C# and .NET 8.
This API allows for the maintenance of state in our project while avoiding direct access to the database on the frontend.

## Getting Started
### Database Setup
For development environments it is recommended to use Docker to run a MongoDB instance. 
The configuration file would support hosting on some remote platform however.
This was developed working with the following image ID: _sha256:4d441da0b85563bec4ac79fdedfbf140c133986e1e8a9d4d073a0b4767514d2e_

### Development Setup Instructions
1. Clone the repository
1. Clone the latest mongodb image from docker hub. This is the id 4d441da0b855
1. On Windows you might need to install the WSL2 backend for Docker Desktop, and MongoDB Compass to view the database.

## Running the application
### App Settings
if you are running both of the containers on the same machine use the following URL's in appsettings.
- http/https mode, MongoDB.URL should be localhost.
When running in 
When running in Docker, it should be host.docker.internal.

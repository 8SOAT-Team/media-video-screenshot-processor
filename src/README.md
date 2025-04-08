# Running the Project with Docker

This project is containerized using Docker, with services defined in the provided Docker Compose file. Follow the steps below to build and run the project:

## Prerequisites

- Docker version 20.10 or higher
- Docker Compose version 1.29 or higher

## Services and Ports

- **video-screenshot-message-consumer**: Exposes port `5148`

## Build and Run Instructions

1. Clone the repository and navigate to the project root directory.
2. Build and start the services using Docker Compose:
   ```bash
   docker-compose up --build
   ```
3. Access the services via their respective exposed ports.

## Environment Variables

- Define any required environment variables in a `.env` file in the project root. Refer to the Docker Compose file for variable names.

## Notes

- Ensure the required ports are available and not in use by other applications.
- For additional configuration, modify the Docker Compose file as needed.

This setup ensures a seamless development and deployment experience using Docker.
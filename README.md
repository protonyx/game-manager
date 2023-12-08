# Game Manager

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

An app for tracking player turns and scores. Players join a game using a unique entry code and receive notifications when it is their turn.

## Tech Stack

**Client:** Angular, Ngrx

**Server:** ASP.NET Core, Entity Framework Core


## Dependencies

* .NET 8.0 SDK
* NodeJS 16

## Run Locally

### Using Docker Compose

```bash
docker compose up --build
```

### Manually

Run Web Application

```bash
cd web
npm install
npm run start
```

Run API

```bash
cd src
dotnet restore
dotnet run
```

## Running Tests

```bash
cd src/
dotnet test
```

## Deploying

1. Generate a signing key for JWT tokens:

```bash
openssl rand -base64 32
```

## License

[MIT](https://choosealicense.com/licenses/mit/)
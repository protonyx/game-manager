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

### Push notifications (VAPID)

Generate a VAPID key pair for each environment:

```bash
npx web-push generate-vapid-keys
```

Set the public key in both `web/src/environments/environment.ts` (and
`environment.production.ts` for production) as `vapidPublicKey`, and in the
server's `PushNotifications:VapidPublicKey` configuration. These public keys
must match. Keep the private key out of source control; configure it locally
with `dotnet user-secrets set "PushNotifications:VapidPrivateKey" "<private-key>"`
from `src/GameManager.Server`, and configure production with the
`PushNotifications__VapidPrivateKey` environment variable. Set
`PushNotifications:VapidSubject` to a contact URI for the deployment.

Rotating the VAPID key pair invalidates existing browser push subscriptions;
players need to re-enable turn notifications after rotation.

## License

[MIT](https://choosealicense.com/licenses/mit/)
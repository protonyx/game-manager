# Game Manager Development Guidelines

This document provides essential information for developers working on the Game Manager project.

## Build/Configuration Instructions

### Backend (.NET)

The backend is built using .NET 10.0 with the Aspire framework for distributed applications.

#### Building and Running

Solution file: `src/GameManager.sln`

Build the solution:

```
dotnet build src/GameManager.sln
```

Run the application:

```
dotnet run --project src/AppHost/AppHost.csproj
```

The Aspire AppHost will start the following components:
- Redis cache (with RedisInsight for monitoring)
- GameManager.Server API
- Angular frontend application

### Frontend (Angular)

The frontend is an Angular 20 application with NgRx for state management.

#### Building and Running
1. Navigate to the web directory:
   ```
   cd web
   ```
2. Install dependencies:
   ```
   npm install
   ```
3. Run the development server:
   ```
   npm start
   ```
   
The Angular application will be available at http://localhost:4200.

## Running Tests

### Running Backend Tests

```
dotnet test src/GameManager.Tests/GameManager.Tests.csproj
```

### Running Frontend Tests

```
cd web
npm test
```

## Additional Development Information

### Architecture

The application follows a clean architecture pattern:
- **GameManager.Domain**: Contains domain entities and business logic
- **GameManager.Application**: Contains application services and use cases
- **GameManager.Persistence.Sqlite**: Data access using SQLite
- **GameManager.Server**: API endpoints and SignalR hubs
- **AppHost**: Aspire host application that orchestrates all components
- **web**: Angular frontend application

### Code Style and Conventions

#### Backend (.NET)
- Use C# 12 features where appropriate
- Follow Microsoft's C# coding conventions
- Use nullable reference types
- Use async/await for asynchronous operations
- Use dependency injection for services

#### Frontend (Angular)
- Follow Angular style guide
- Use TypeScript features and strong typing
- Component selectors should be elements with 'app' prefix in kebab-case
- Directive selectors should be attributes with 'app' prefix in camelCase
- Use NgRx for state management
- Use Angular Material for UI components

### Linting and Formatting

#### Backend
No specific linting tools are configured, but follow standard C# conventions.

#### Frontend
- ESLint is configured for linting TypeScript files
- Prettier is used for code formatting
- Run linting: `npm run lint`
- Run formatting: `npm run format`

### SignalR for Real-time Communication

The application uses SignalR for real-time communication between the server and clients. The SignalR hub is defined in the GameManager.Server project and connected to in the Angular application using @microsoft/signalr.

### Docker Support

The application can be containerized using Docker:
- A Dockerfile is provided in the root directory
- A docker-compose.yaml file is available for running the application with its dependencies
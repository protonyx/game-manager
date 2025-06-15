# Game Manager Development Guidelines

This document provides essential information for developers working on the Game Manager project.

## Build/Configuration Instructions

### Backend (.NET)

The backend is built using .NET 9.0 with the Aspire framework for distributed applications.

#### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or later (recommended) or VS Code with C# extensions

#### Building and Running
1. Clone the repository
2. Open the solution file `src/GameManager.sln` in Visual Studio or use the .NET CLI
3. Build the solution:
   ```
   dotnet build src/GameManager.sln
   ```
4. Run the application:
   ```
   dotnet run --project src/AppHost/AppHost.csproj
   ```

The Aspire AppHost will start the following components:
- Redis cache (with RedisInsight for monitoring)
- GameManager.Server API
- Angular frontend application

### Frontend (Angular)

The frontend is an Angular 18 application with NgRx for state management.

#### Prerequisites
- Node.js (latest LTS version)
- npm (comes with Node.js)

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

## Testing Information

### Backend Testing

The backend uses xUnit v3 as the testing framework with additional libraries:
- AutoFixture and AutoMoq for test data generation and mocking
- FluentAssertions for more readable assertions
- FastEndpoints.Testing for API endpoint testing
- Microsoft.AspNetCore.Mvc.Testing for integration testing

Use these guidelines when writing tests:
- All test dependencies must be registered with the test fixture by calling `fixture.Freeze` with the `Mock` type as the type argument
- Never mock `IUserContext`, instead use the `SetUser` extension for AutoFixture to configure the User Principal:
  ```
  fixture.SetUser(user =>
  {
    user.AddGameId(game.Id)
        .AddPlayerId(player.Id);
  })
  ```

#### Running Backend Tests
```
dotnet test src/GameManager.Tests/GameManager.Tests.csproj
```

#### Adding New Tests

1. Create a new test class in the appropriate directory under `src/GameManager.Tests/`
2. Use the `[Fact]` attribute for test methods
3. Follow the Arrange-Act-Assert pattern
4. Use FluentAssertions for assertions

Example:
```csharp
using FluentAssertions;

namespace GameManager.Tests;

public class SimpleTest
{
    [Fact]
    public void SimpleTestExample_ShouldPass()
    {
        // Arrange
        var testString = "Hello, World!";
        
        // Act
        var result = testString.ToUpper();
        
        // Assert
        result.Should().Be("HELLO, WORLD!");
    }
}
```

### Frontend Testing

The frontend uses Jasmine and Karma for testing.

#### Running Frontend Tests
```
cd web
npm test
```

#### Adding New Tests

1. Create a `.spec.ts` file alongside the component or service you want to test
2. Use Jasmine's `describe` and `it` functions to structure your tests
3. Use Angular's TestBed for component testing

Example:
```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyComponent } from './my-component.component';

describe('MyComponent', () => {
  let component: MyComponent;
  let fixture: ComponentFixture<MyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyComponent ]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
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
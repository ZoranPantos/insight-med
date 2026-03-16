# Technical

<br>

## High-level look

_InsightMed_ is a microservice-based system whose infrastructure consists of the
following dockerized components:
- _InsightMed.API_: [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0) (.NET 10)
- _InsightMed.Web_: [Angular](https://angular.dev/) Single Page Application (v21, Node.js v24)
- _InsightMed.LabRpcServer_: [ASP.NET Core Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0) + Background Worker (.NET 10) acting as a simulated external laboratory system
- API Gateway: [nginx](https://nginx.org/) reverse proxy serving as the single entry point for all client traffic
- Databases: two distinct [SQL Server](https://www.microsoft.com/en-us/sql-server) instances
    - **InsightMedDb**: Main application database managed via [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
    - **LabDb**: Laboratory simulation database managed via [ADO.NET](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ado-net-overview)
- Message Broker: [RabbitMQ](https://www.rabbitmq.com/) for asynchronous service communication
- Observability: [Elasticsearch](https://www.elastic.co/elasticsearch) and [Kibana](https://www.elastic.co/kibana) for log aggregation and visualization

<br>

<p align="center">
    <img src="img/high-level-look.png" alt="High level look">
</p>

<br>

_NOTE: Diagram to be updated; missing gateway_

## API Gateway

All client traffic flows through a single entry point — an nginx reverse proxy acting as the API Gateway.
The frontend does not communicate with backend services directly; instead, it sends all requests to the gateway,
which routes them to the appropriate service based on the URL path:

- `/api/*` → _InsightMed.API_
- `/notifications` → _InsightMed.API_ (WebSocket upgrade for SignalR)
- `/*` → _InsightMed.Web_ (Angular SPA)

The gateway also exposes the _InsightMed.LabRpcServer_ REST endpoints under the `/lab/*` prefix
(e.g. `/lab/health`, `/lab/lab-parameters`). These are not used by the frontend application but are
available for direct access.

When running locally from the IDE (without Docker), the Angular CLI dev server provides an equivalent
proxy configuration (`proxy.conf.json`) that routes `/api` and `/notifications` to the locally running API.

<br>

## Communication between components

The system uses different communication patterns to handle different requirements:
- RESTful HTTP: Standard request/response model used between the Web API and Angular frontend
- Real-Time with [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr): A persistent WebSocket connection between the Web API and Angular frontend (proxied through the gateway) to push notifications to the user without the need for polling
- [RPC Messaging with RabbitMQ](https://www.rabbitmq.com/tutorials/tutorial-six-dotnet): Used for asynchronous bidirectional communication between the Web API and the LabRpcServer (Minimal API)

<br>

## Backend architecture

The backend solution, i.e. _InsightMed.API_ and associated class libraries follows Clean Architecture principles
combined with the CQRS (Command Query Responsibility Segregation) pattern using the [MediatR](https://mediatr.io/) library.  

The solution is partitioned into four primary layers:
- Domain
- Application
- Infrastructure
- API

<br>

<p align="center">
    <img src="img/clean-architecture.png" alt="Clean Architecture">
</p>

<br>

**Domain** is the core of the application, containing entities and enums. Entities are just POCOs (Plain Old CLR Objects) and do not inherit from
base classes or depend on external frameworks.  

**Application** defines the busines logic and use cases. By the CQRS, logic is split into commands (writes) and queries (reads).
All requests pass through a MediatR pipeline that handles cross-cutting concerns before reaching the handler.
Currently in our case, we have:
- Validation behavior which uses [FluentValidation](https://docs.fluentvalidation.net/en/latest/) to validate DTOs. Throws exception and logs errors if validation fails
- Logging behavior which uses [Serilog](https://serilog.net/) and logs request/response performance and details  

[AutoMapper](https://automapper.io/) is utilized to map between domain entities and DTOs.  
This layer defines contracts for Infrastructure to implement.

**Infrastructure** implements interfaces defined in the Application layer.
Here we have:
- Data access: Implementation of the `AppDbContext`
- Messaging: Implementation of the `RabbitMqRpcClient`
- Services: Module-specific services that inject the `DbContext` to manipulate state. These act as a higher-level repository layer although
they are not limited only to state manipulation
- PDF Generation: Uses [QuestPDF](https://www.questpdf.com/) to generate lab reports pdf
- Authentication: Implementation of `AuthService` and `CurrentUserService`

**API** is the entry point of the application. It consists of:
- Controllers: Minimal logic, responsible for dispatching MediatR requests and returning standard HTTP responses
- Global exception handling: A dictionary-based `GlobalExceptionHandler` converts exceptions into standard `ProblemDetails` responses with appropriate HTTP status codes
- Hubs: Contains the `NotificationHub` for SignalR communication

<br>

## Data access strategy

The solution uses two distinct approaches for database interaction.  

- **InsightMedDb** (EF Core) is used by the main API with Code-First approach. Relationships are defined via Fluent API in `OnModelCreating` method.  
Database is created automatically on startup by execution of migration scripts and seeded via a specific API endpoint _[GET] api/AppManagement/SeedData_.  
- **LabDb** (ADO.NET) is used by the _LabRpcServer_. The approach consists of raw SQL via ADO.NET.  
The worker service ensures the database and tables exist and are seeded upon startup.

Use case of in-memory caching is also present, where `IMemoryCache` is utilized for lab parameters as this reference data changes rarely.

<br>

## Lab RPC Server

The _InsightMed.LabRpcServer_ is a standalone ASP.NET Core service designed to simulate an external laboratory clinic.
It combines a background worker (for RabbitMQ message consumption) with a minimal REST API surface, running both
in the same process.

### RPC message processing
1. Connection: Maintains a persistent connection to RabbitMQ using `IConnection` and `IChannel`
2. Consumption: Listens on a specific queue for `LabRequest` messages
3. Processing:
    1. Deserializes the request
    2. Fetches parameter constraints from **LabDb**
    3. Randomizer Service: Uses `ParameterValueRandomizerService` to generate results. It produces either a boolean (for qualitative tests)
       or a numeric value (for quantitative tests) based on defined thresholds (`MinThreshold`, `MaxThreshold`)
    4. Simulation: Introduces a random delay (configurable via `LabResultsDelaySimulationParameters` in `appsettings.json`) to simulate
       physical processing time
4. Reply: Publishes the result back to the `ReplyTo` queue specified in the message properties, creating an RPC experience for the API

### REST API
The service also exposes two HTTP endpoints:
- `GET /health` — Standard health check returning service status
- `GET /lab-parameters` — Returns the list of available laboratory parameters from **LabDb** with their reference ranges

These endpoints are routable through the API Gateway under the `/lab/` prefix (e.g. `/lab/health`, `/lab/lab-parameters`)
or accessible directly on port `5100`.

<br>

## Authentication & Identity

The solution uses hybrid approach for identity management, combining [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-10.0&tabs=visual-studio) for user persistence and
JWT (JSON Web Tokens) for stateless authentication.  

`AppDbContext` extends `IdentityDbContext` to persist users, roles and claims in the **InsightMedDb**. The system uses the standard `IdentityUser` to store credentials.  
Since the API is stateless, it does not use cookies for sessions but instead it issues JWTs.
The JWT settings are loaded from `appsettings.json` via `JwtOptions`.  
`JwtBearer` middleware is configured in the API pipeline
to validate the token signature and expiration on every request before it reaches the controllers.  

The `[Authorize]` attribute is used on controllers/endpoints to enforce that a valid token is present.  

Main services used for the authentication on the backend:
- `AuthService`: A scoped service that acts as the facade for identity operations, responsible for managing user
  accounts (registration, password changes) via ASP.NET Core Identity and generating signed JWT tokens upon successful credential validation
- `CurrentUserService`: A scoped service that extracts the `UserId` and `Claims` from the current `HttpContext`, making the current user's identity available to the Application layer

<br>

## Logging

Serilog is the logging backbone for both the API and the Worker Service.  
Logs are shipped directly to **Elasticsearch**, where a **Kibana Data View** (`logs-insightmed-development*`) is used to inspect logs.  
Additionally, logs are enriched with context such as `Application` name and `CorrelationId` to trace requests across the distributed system.

<br>

## Testing

Here we have two projects in question:
- _InsightMed.UnitTests_: [xUnit](https://xunit.net/?tabs=cs) unit test project in a combination with [Moq](https://github.com/devlooped/moq) for basic mocking scenarios
- _InsightMed.IntegrationTests_: xUnit integration test project that leverages [Testcontainers](https://testcontainers.com/) for throwaway Docker dependencies and `WebApplicationFactory` to host the API in-memory

<br>

## Frontend architecture

The frontend is an Angular 21 application structured by feature modules.  

Project structure consists of:
- App Modules: Split into logical folders (patients, reports, requests, etc.) containing components and specific logic
- Core/Shared: Reusable components, pipes and directives  

Authentication on the frontend is leveraged by an interceptor - `authInterceptor` which is registered globally.
It injects the `AuthService`, retrieves the JWT token and appends the `Authorization: Bearer {token}` header to
every outgoing HTTP request.  
Additionally, route guards protect access to authorized pages.  

As for SignalR integration, a dedicated `SignalRService` manages the WebSocket connection lifecycle:
- Connection: Establishes a connection to the `/notifications` hub, passing the JWT via an access token factory
- State Management: Uses Angular Signals (`hasUnseenNotifications`) to reactively update the UI when the server invokes `ReceiveUnseenStatus`
- Resilience: Configured with `.withAutomaticReconnect()` to handle network instability
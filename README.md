# PicGramWebApp — AADBDT + APP Project

PicGramWebApp is an ASP.NET Core MVC photo-sharing web application created for two connected university projects:

- **Project 1 — Advanced Application Development Based on Development Templates (AADBDT)**
- **Project 2 — Advanced Programming Paradigms (APP)**

The first project focuses on building the core photo-sharing application and demonstrating design patterns.  
The second project improves the same application by adding automated testing, functional programming examples, aspect-oriented programming, metrics, monitoring, Git workflow, refactoring/SOLID examples, and Docker containerization.

---

## Project Overview

PicGramWebApp allows users to upload, browse, search, edit, and download photos. The application supports anonymous users, registered users, and administrators.

Main features include:

- Local registration and login using ASP.NET Core Identity
- GitHub external login
- User roles: registered user and administrator
- Package system with FREE, PRO, and GOLD plans
- Upload limits, download limits, and storage limits
- Photo upload with description and hashtags
- Photo browsing and photo details
- Search by hashtag, author, date range, and file size
- Original photo download
- Processed photo download with image filters
- Admin panel for users, photos, logs, packages, and statistics
- Action logging
- Design patterns across the application
- Unit, integration, and UI tests
- Functional helper methods used in production code
- AOP-style action filters for metrics
- Health and metrics endpoints
- Docker and Docker Compose support

---

## Technology Stack

### Backend

- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server LocalDB for local development
- SQL Server Docker container for containerized execution
- Razor Views and Identity Pages
- ImageSharp for image processing

### Testing

- xUnit
- Entity Framework Core InMemory provider
- Microsoft.AspNetCore.Mvc.Testing
- Playwright for UI/browser tests

### DevOps / Tooling

- Git and GitHub
- Docker
- Docker Compose
- Visual Studio 2022

---

## Repository Structure

```text
PicGram/
??? PicGram.sln
??? README.md
??? docker-compose.yml
??? PicGramWebApp/
?   ??? Controllers/
?   ??? Data/
?   ??? Filters/
?   ??? Models/
?   ??? Services/
?   ??? Views/
?   ??? Areas/Identity/
?   ??? wwwroot/
?   ??? Dockerfile
?   ??? Program.cs
??? PicGramWebApp.Tests/
??? PicGramWebApp.IntegrationTests/
??? PicGramWebApp.UiTests/
```

---

# Project 1 — AADBDT

## User Types

The application supports three user types:

### Anonymous User

Anonymous users can:

- Browse public photos
- View photo details
- Search photos by filters
- Open download options

### Registered User

Registered users can:

- Upload photos
- Edit their own photo descriptions and hashtags
- Download original photos
- Download processed photos
- View their own uploaded photos
- View their package usage
- Request package changes

### Administrator

Administrators can:

- View all users
- Change user packages
- View all uploaded photos
- Edit any photo
- Delete any photo
- View action logs
- View user statistics

---

## Authentication

Authentication is implemented using ASP.NET Core Identity.

Supported authentication methods:

- Local email/password registration
- Local login/logout
- GitHub external login

GitHub authentication data is adapted into the internal application model using an Adapter pattern.

Sensitive GitHub OAuth values are not stored in the tracked `appsettings.json`. Local development secrets should be stored in `appsettings.Development.json` or user secrets.

---

## Package System

The application includes three package plans:

- FREE
- PRO
- GOLD

Each package controls:

- Maximum uploads per month
- Maximum downloads per month
- Maximum storage capacity

Users can view their current package usage and request a package change. Package change requests become active the following day.

The package validation logic checks:

- Monthly upload count
- Monthly download count
- Total storage usage

---

## Photo Upload

Registered users can upload photos with:

- Image file
- Description
- Hashtags

Uploaded photos are saved through the storage provider abstraction. The current implementation uses local storage.

Hashtag normalization is handled through functional helper methods, so hashtags are consistently cleaned and lowercased before being saved.

---

## Photo Browsing

Users can browse uploaded photos. The default photo index shows the latest uploaded photos with:

- Thumbnail
- Description
- Author
- Upload date
- Hashtags

Clicking a photo opens a details page with the full photo and metadata.

---

## Photo Search

Photo search supports:

- Hashtag
- Author
- Upload date range
- Minimum file size
- Maximum file size

The search flow uses:

- `PhotoSearchCriteriaBuilder`
- `PhotoSearchCriteria`
- `IPhotoSearchService`
- `PhotoSearchService`

This separates search criteria construction from query execution.

---

## Photo Download and Image Processing

Users can download:

- The original uploaded image
- A processed version of the image

Processed downloads support:

- Resize
- Sepia
- Blur
- Grayscale
- Sharpen
- Brightness
- Contrast
- JPG, PNG, or BMP output format

Image processing is implemented with the Strategy pattern. Each processing operation is represented by a separate strategy class.

---

## Logging

Application actions are logged with:

- User
- Timestamp
- Action type
- Details

Examples of logged actions:

- Upload photo
- Edit photo
- Download original
- Download processed
- Package change request
- Admin package change
- Admin photo edit
- Admin photo delete

Logging is implemented through an observer-based action notification flow.

---

# Design Patterns

The AADBDT project demonstrates multiple design patterns.

## 1. Facade Pattern

Location:

```text
PicGramWebApp/Services/Facade/PhotoFacade.cs
```

`PhotoFacade` provides a simplified entry point for complex photo workflows such as upload and edit. It coordinates user lookup, package validation, storage, database updates, hashtag handling, and logging.

## 2. Builder Pattern

Location:

```text
PicGramWebApp/Services/Search/PhotoSearchCriteriaBuilder.cs
```

The Builder pattern is used to construct photo search criteria step by step. This is useful because search filters are optional.

## 3. Strategy Pattern

Location:

```text
PicGramWebApp/Services/ImageProcessing/
```

Image filters are implemented as strategies. Examples:

- `ResizeStrategy`
- `SepiaStrategy`
- `BlurStrategy`
- `GrayscaleStrategy`
- `SharpenStrategy`
- `BrightnessStrategy`
- `ContrastStrategy`

This allows filters to be selected and combined dynamically.

## 4. Factory Method Pattern

Location:

```text
PicGramWebApp/Services/Storage/StorageProviderFactory.cs
```

The storage provider factory creates the correct storage provider based on configuration. The current implementation supports local storage.

## 5. Observer Pattern

Location:

```text
PicGramWebApp/Services/Observers/
```

The observer pattern is used for action logging. Business operations notify a subject, and observers react by writing action logs.

## 6. Command Pattern

Location:

```text
PicGramWebApp/Services/Commands/
```

Commands encapsulate business actions such as:

- Requesting package changes
- Admin changing a user package
- Admin editing a photo
- Admin deleting a photo

## 7. Chain of Responsibility Pattern

Location:

```text
PicGramWebApp/Services/Packages/Validation/
```

Package validation rules are implemented as a chain of handlers. Each handler checks one rule and either passes the request forward or denies it.

## 8. Adapter Pattern

Location:

```text
PicGramWebApp/Services/Adapters/
```

The GitHub external login adapter converts provider-specific login data into a unified internal representation.

---

# Project 2 — Advanced Programming Paradigms Improvements

Project 2 improves the original AADBDT project by adding testing, AOP, functional programming, metrics, Docker, Git workflow, and refactoring/SOLID examples.

All Project 2 work was developed on the branch:

```text
project2-app-improvements
```

The original Project 1 baseline is preserved on:

```text
main
```

A tag was also created:

```text
project-1-final
```

---

## Testing

The solution contains three test projects:

```text
PicGramWebApp.Tests
PicGramWebApp.IntegrationTests
PicGramWebApp.UiTests
```

### Unit Tests

Unit tests cover isolated logic such as:

- Search criteria builder
- Package validation handlers
- Functional helper methods

Examples:

```text
PhotoSearchCriteriaBuilderTests
PackageValidationHandlerTests
PhotoFunctionalHelpersTests
```

### Integration Tests

Integration tests verify application-level behavior such as:

- Home page loading
- Photo index loading
- Anonymous upload access redirecting to login

These tests use `WebApplicationFactory<Program>`.

### UI Tests

UI tests are implemented with Playwright.

They verify browser-level behavior such as:

- Home page opens
- Search page opens
- Anonymous upload redirects to login

UI tests require the application to be running locally.

---

## Running Tests

Run unit tests:

```bash
dotnet test PicGramWebApp.Tests
```

Run integration tests:

```bash
dotnet test PicGramWebApp.IntegrationTests
```

Run UI tests after starting the application on `https://localhost:7089`:

```bash
dotnet test PicGramWebApp.UiTests
```

Run the whole solution:

```bash
dotnet test
```

Note: UI tests are browser-based and expect the app to already be running on the configured local URL.

---

## Functional Programming

Functional programming examples are implemented in:

```text
PicGramWebApp/Services/Functional/PhotoFunctionalHelpers.cs
```

The helper methods are pure functions:

- Same input produces same output
- No database access
- No file access
- No hidden state
- No side effects

Implemented helper methods:

- `NormalizeHashtag`
- `NormalizeAuthor`
- `NormalizeOutputFormat`
- `CalculateStorageUsagePercentage`
- `CalculateRemainingStorageBytes`

These helpers are used in production code for:

- Hashtag normalization during upload/edit
- Search normalization
- Processed download format normalization
- Package storage usage calculations

This reduces duplication and separates deterministic transformations from controllers and service workflow code.

---

## Aspect-Oriented Programming

AOP-style behavior is implemented using ASP.NET Core action filters.

Locations:

```text
PicGramWebApp/Filters/ExecutionTimeAspectAttribute.cs
PicGramWebApp/Filters/ActionCounterAspectAttribute.cs
PicGramWebApp/Services/Metrics/AppMetrics.cs
```

### ExecutionTimeAspectAttribute

Measures how long selected controller actions take to execute.

### ActionCounterAspectAttribute

Counts how many times selected controller actions are executed.

These aspects are applied declaratively to selected controller actions, keeping monitoring logic separate from business logic.

Examples of monitored actions:

- `PhotoController.Search`
- `PhotoController.Upload`
- `PhotoController.Download`
- `PhotoController.DownloadProcessed`
- `AdminController.Statistics`

---

## Metrics and Monitoring

Monitoring endpoints are implemented in:

```text
PicGramWebApp/Controllers/MonitoringController.cs
```

Available endpoints:

```text
/Monitoring/Health
/Monitoring/Metrics
```

### Health Endpoint

Example:

```text
http://localhost:8080/Monitoring/Health
```

Returns application health information such as:

- Status
- Timestamp
- Database configuration flag

### Metrics Endpoint

Example:

```text
http://localhost:8080/Monitoring/Metrics
```

Returns metrics such as:

- Total users
- Total photos
- Total action logs
- Total package change requests
- Per-action execution counts
- Average execution time per monitored action

Custom metrics are collected through the AOP-style action filters.

---

## SOLID Principles

The project demonstrates several SOLID principles.

### Single Responsibility Principle

Examples:

- `PhotoSearchService` handles search query execution
- `PhotoFacade` coordinates photo workflows
- Image strategy classes each handle one image operation
- AOP filter classes each handle one monitoring concern

### Open/Closed Principle

Examples:

- New image filters can be added by implementing `IImageProcessingStrategy`
- New package validation rules can be added as validation handlers
- New storage providers can be added behind `IStorageProvider`

### Liskov Substitution Principle

Examples:

- Concrete image strategies are interchangeable through `IImageProcessingStrategy`
- Observer implementations are interchangeable through `IPhotoActionObserver`
- Storage providers are interchangeable through `IStorageProvider`

### Interface Segregation Principle

Examples:

- `ICommand<T>`
- `IPhotoSearchService`
- `IExternalUserAdapter`
- `IPhotoActionObserver`
- `IStorageProvider`

These interfaces are small and focused.

### Dependency Inversion Principle

Examples:

- Controllers depend on services and abstractions
- Search is accessed through `IPhotoSearchService`
- External login mapping is accessed through `IExternalUserAdapter`
- Storage is accessed through `IStorageProvider`

---

## Refactoring

Project 2 was developed as an incremental improvement of Project 1.

Main refactoring/improvement examples:

- Added missing authentication middleware
- Extracted pure functional helper methods
- Wired functional helpers into production code
- Added test projects instead of mixing tests with production code
- Added AOP filters for monitoring instead of scattering timing/counting code through controllers
- Added Docker support without rewriting the core application
- Removed GitHub OAuth secrets from tracked `appsettings.json`

The goal was to improve maintainability, testability, observability, and portability without redesigning the whole application.

---

# Docker

The application can be run with Docker Compose.

Docker-related files:

```text
PicGramWebApp/Dockerfile
docker-compose.yml
```

Docker Compose starts:

- ASP.NET Core web application container
- SQL Server container

The original local development setup used SQL Server LocalDB, which is Windows-specific. For Docker, the app uses a SQL Server container and a container-friendly connection string.

Database migrations are applied during startup using:

```csharp
db.Database.Migrate();
```

This allows the Docker SQL Server database to initialize automatically.

---

## Docker Volumes

The Docker setup uses two named volumes:

```text
sqlserver_data
uploads_data
```

### `sqlserver_data`

Persists SQL Server database data.

This keeps:

- users
- photo metadata
- action logs
- package data
- package change requests

### `uploads_data`

Persists uploaded physical image files.

This prevents a situation where the database photo record remains, but the physical uploaded image disappears after the web container is recreated.

---

## Running with Docker

Start Docker Desktop first.

From the repository root, run:

```bash
docker compose up --build
```

Open the app:

```text
http://localhost:8080
```

Stop containers while keeping data:

```bash
docker compose stop
```

Start stopped containers again:

```bash
docker compose start
```

Cleanly remove containers and network while keeping volumes:

```bash
docker compose down
```

Do not run this unless you intentionally want to delete the database and uploaded files:

```bash
docker compose down -v
```

The `-v` flag removes Docker volumes.

---

# Running Locally Without Docker

Open the solution in Visual Studio:

```text
PicGram.sln
```

Make sure SQL Server LocalDB is available.

Apply migrations if needed:

```bash
dotnet ef database update --project PicGramWebApp
```

Run the application from Visual Studio or with:

```bash
dotnet run --project PicGramWebApp
```

The local development app usually runs on:

```text
https://localhost:7089
```

---

# Configuration

Tracked `appsettings.json` should not contain real secrets.

Local-only secrets can be stored in:

```text
PicGramWebApp/appsettings.Development.json
```

Example local-only GitHub OAuth configuration:

```json
{
  "Authentication": {
    "GitHub": {
      "ClientId": "YOUR_CLIENT_ID",
      "ClientSecret": "YOUR_CLIENT_SECRET"
    }
  }
}
```

This file should not be committed.

---

# Git Workflow

The repository uses a simple branch workflow:

```text
main
project2-app-improvements
```

### `main`

Contains the Project 1 AADBDT baseline.

### `project2-app-improvements`

Contains Project 2 APP improvements:

- tests
- AOP
- metrics
- functional programming
- Docker
- refactoring/SOLID examples

### Tag

```text
project-1-final
```

Marks the final Project 1 version before Project 2 improvements.

This workflow keeps the original Project 1 solution stable while Project 2 improvements are developed separately.

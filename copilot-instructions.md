# EkosPro Development Standards

## Project Overview

This solution is a small to medium-sized .NET 9 REST API.

The main objective is to keep the codebase simple, maintainable, readable, and easy to evolve.

Prioritize simplicity over enterprise-level complexity.

## Solution Structure

```text
backend-ekos-pro
Domain.Service
Infrastructure.Entity
Infrastructure.Core
Utils
```

### backend-ekos-pro

Responsibilities:

* Controllers
* API configuration
* Dependency Injection registration
* Swagger configuration
* Middleware configuration
* Authentication and Authorization configuration

Restrictions:

* Do not implement business logic in Controllers.
* Controllers should only invoke MediatR requests.
* Controllers should return standardized API responses.

### Domain.Service

Responsibilities:

* Domain entities
* DTOs
* CQRS features
* Commands
* Queries
* Handlers
* Validators
* Interfaces
* Domain exceptions

Suggested structure:

```text
Domain.Service
│
├── Entities
├── DTOs
├── Interfaces
├── Exceptions
│
└── Features
    └── FeatureName
        ├── Commands
        ├── Queries
        ├── Handlers
        └── Validators
```

### Infrastructure.Entity

Responsibilities:

* Entity Framework Core
* DbContext
* Entity configurations
* Migrations
* Data access implementation

### Infrastructure.Core

Responsibilities:

* External integrations
* Azure services
* Email services
* Storage services
* Third-party services

### Utils

Responsibilities:

* Constants
* Extensions
* Helper methods
* Shared utility classes

---

# General Architecture Rules

* Follow SOLID principles.
* Follow Clean Code principles.
* Prefer composition over inheritance.
* Keep classes focused on a single responsibility.
* Avoid unnecessary abstractions.
* Avoid overengineering.
* Keep the solution easy to understand.

Do not create unnecessary layers.

---

# Forbidden Patterns

Do not create:

* Generic Repository
* Unit Of Work
* Service Locator
* Abstract Factory
* Generic Service Base classes
* Deep inheritance hierarchies
* Unnecessary design patterns

Use only patterns that provide real value.

---

# CQRS

Use MediatR.

Each business feature should contain:

* Commands
* Queries
* Handlers
* Validators

Example:

```text
Features
└── CatalogBooks
    ├── Commands
    ├── Queries
    ├── Handlers
    └── Validators
```

Rules:

* Commands modify data.
* Queries retrieve data.
* Handlers contain application logic.
* Validation must not be implemented inside handlers.

---

# Entity Framework Core

Use:

* Entity Framework Core
* Code First
* SQL Server

Requirements:

* Use Fluent API configurations.
* Use IEntityTypeConfiguration.
* Keep DbContext inside Infrastructure.Entity.
* Use async operations for database access.

Avoid DataAnnotations whenever Fluent API is sufficient.

---

# Base Entity

All entities should inherit from BaseEntity.

BaseEntity should contain:

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
```

---

# DTOs

DTOs must be separated from entities.

Rules:

* Never expose entities directly through APIs.
* Use DTOs for requests and responses.
* Keep DTOs simple.
* Avoid business logic in DTOs.

---

# API Response Standard

All endpoints should return:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public IEnumerable<string>? Errors { get; set; }
}
```

Example:

```json
{
  "success": true,
  "message": "Books retrieved successfully",
  "data": []
}
```

---

# Validation

Use FluentValidation.

Requirements:

* One validator per command.
* Keep validation rules separate from handlers.
* Return meaningful validation messages.

---

# Exception Handling

Implement global exception handling.

Create custom exceptions when necessary:

* NotFoundException
* BusinessException
* ValidationException

Use middleware to handle exceptions consistently.

---

# Logging

Use ILogger.

Log:

* Request start
* Request completion
* Validation failures
* Unexpected exceptions

All log messages must be written in English.

Example:

```csharp
logger.LogInformation("Book created successfully");
```

---

# Language Standard

All source code must be written in English.

The following must always be written in English:

* Classes
* Methods
* Properties
* Variables
* Interfaces
* DTOs
* Commands
* Queries
* Handlers
* Validators
* Exception messages
* Log messages
* XML comments
* Inline comments
* Swagger descriptions

Never generate source code in Spanish.

---

# Naming Conventions

Follow Microsoft .NET naming conventions.

## PascalCase

Use for:

* Classes
* Records
* Properties
* Enums

Examples:

```csharp
CatalogBook
CatalogBookDto
CreateCatalogBookCommand
GetCatalogBooksQuery
```

## camelCase

Use for:

* Variables
* Parameters

Example:

```csharp
catalogBook
bookId
```

## Interfaces

Prefix with I.

Examples:

```csharp
ICatalogBookRepository
IBookService
```

## Async Methods

All asynchronous methods must end with Async.

Examples:

```csharp
GetBookByIdAsync
CreateBookAsync
UpdateBookAsync
DeleteBookAsync
```

---

# Controllers

Controllers should:

* Be thin.
* Use MediatR.
* Return ApiResponse<T>.
* Not contain business rules.
* Not access DbContext directly.

---

# Code Style

Prefer:

* Readability
* Maintainability
* Explicit code

Avoid:

* Large classes
* Large methods
* Duplicate code
* Premature optimization

Keep implementations straightforward and easy to maintain.

When multiple solutions are possible, always choose the simplest solution that satisfies the requirement.

# Architecture

## Overview

TaskManager follows a layered architecture separating responsibilities between:

- API
- Application
- Domain
- Infrastructure

This structure improves maintainability, testability, and separation of concerns.

---

## Layers

### API

Responsibilities:

- HTTP endpoints
- Authentication & authorization
- Rate limiting
- Request/response mapping
- Cookie handling

The API layer does not contain business logic.

---

### Application

Responsibilities:

- Business logic
- Use case orchestration
- Validation rules
- Coordination between repositories and services

Services include:

- Authentication service
- Task service

This layer defines interfaces for repositories and external services.

---

### Domain

Responsibilities:

- Core entities
- Business rules at entity level
- No external dependencies

Entities include:

- User
- TaskItem
- RefreshToken

The domain layer does not depend on infrastructure or frameworks.

---

### Infrastructure

Responsibilities:

- Database access (EF Core)
- Repository implementations
- JWT token generation
- Hashing utilities

Infrastructure depends on external frameworks and implements interfaces defined in the Application layer.

---

## Dependency Direction

Dependencies follow this rule:

API → Application → Domain  
Infrastructure → Domain  
Infrastructure → Application (implements interfaces)

The Domain layer has no outward dependencies.

---

## Design Principles

- Separation of concerns
- Explicit dependency boundaries
- Business logic isolated from HTTP layer
- Security-sensitive operations centralized in services
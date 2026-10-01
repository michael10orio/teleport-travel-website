# Teleport - Travel Discovery & Trip Planning PoC

## Project Description

Teleport is a Proof of Concept travel website for students, young adults, and personal travelers. It is intended to support travel discovery and basic trip planning through a clear, responsive user experience.

## Project Goal

The PoC will demonstrate destination discovery, trip planning, itinerary management, and basic travel expense tracking. The project will be developed incrementally according to the planned sprints and documented requirements.

## Scope

Planned functionality includes:

- Browse, search, filter, and view destination details.
- View activities associated with destinations.
- Create, view, update, and delete trips.
- Organize destinations and activities into a basic itinerary.
- Add, view, update, and delete expenses associated with trips.
- View relevant trip information in a simple dashboard.

This PoC does not include flight booking, hotel booking, payment processing, real-time services, real-time flight or hotel pricing, complex AI features, or a complex recommendation engine.

## Technology Stack

- Angular with Nx
- Bootstrap
- .NET Minimal API
- PostgreSQL
- Podman
- AWS-hosted PostgreSQL (planned for a later stage)

These technologies are required by the project brief. They will be introduced during the appropriate implementation stages.

## Architecture

The initial planned architecture is:

```text
Angular Nx + Bootstrap
        |
     HTTP / JSON
        v
.NET Minimal API
        |
       SQL
        v
   PostgreSQL
```

Podman containerization and AWS-hosted PostgreSQL are planned for later project stages. Architecture documentation will be expanded as the system is implemented.

## Project Status

**Current stage: Repository setup only.**

Completed in this stage:

- Repository setup files created.
- Initial `.gitignore` added for the planned technology stack and local development files.
- This README added with the project scope, planned stack, and initial architecture.
- Git repository initialized with the initial setup commit.

Not yet implemented:

- Angular/Nx frontend.
- .NET Minimal API.
- PostgreSQL database or schema.
- Podman configuration.
- AWS resources or credentials.
- User stories, requirements, architecture diagrams, ERD, API, and frontend features beyond this initial repository documentation.

The next project step is Sprint 1 planning: requirements gathering, user stories, hypotheses, use cases, architecture v0, ERD/database planning, API planning, frontend planning, Trello backlog organization, and sprint organization.

## Development Principles

The project will be developed honestly and incrementally. Documentation, evidence, test results, and progress claims will be added only when the corresponding work has actually been completed.

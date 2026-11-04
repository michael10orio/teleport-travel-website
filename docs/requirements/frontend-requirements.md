# Teleport Frontend Requirements

## Purpose

Define the planned frontend requirements for the Teleport Travel Discovery & Trip Planning PoC.

## Functional Requirements

- The user can browse destinations.
- The user can search destinations.
- The user can filter destinations.
- The user can view destination details.
- The user can view activities associated with a destination.
- The user can create and review trips.
- The user can organize planned destinations and activities into a trip.
- The user can record and review basic travel expenses.
- The user can view relevant trip information in a dashboard.

## Non-Functional Requirements

- The interface should be responsive on desktop and mobile screens.
- The interface should use Angular with Nx and Bootstrap.
- The interface should provide clear navigation between planned features.
- Forms should provide understandable validation messages.
- The frontend should display useful loading and error states when connected to the API.
- The frontend should not connect directly to PostgreSQL.

## Planned Frontend Areas

- Home
- Destinations
- Destination Details
- Activities
- Trips
- Trip Details
- Itinerary
- Expenses
- Dashboard

## User Story Traceability

These links connect the planned requirements to the project user stories. They do not indicate implementation status or Trello card status.

| Requirement | Related user story |
| --- | --- |
| Browse destinations | View Destinations |
| Search and filter destinations | Search Destinations; Filter Destinations |
| View destination details | View Destination Details |
| View activities for a destination | View Activities |
| Create and review trips | Create a Trip |
| Organize destinations and activities into a trip | Manage Itinerary |
| Record and review travel expenses | Manage Expenses |
| Review trip information in one place | View Dashboard |

## Current Status

The responsive Angular application is implemented in `apps/web`. Its API-backed homepage supports destination search and category filters; destination pages show details and associated activities. Trip pages create, update, and delete trips, organize destinations and activities in itineraries, and support expense creation, editing, and deletion. The dashboard summarizes the user's trips.

Loading, empty, and error states are present on data-driven screens. The production frontend build and browser checks for home, destination details, trips, trip details, and dashboard passed. Browser checks also exercised trip creation, destination/activity itinerary entries, expense creation, dashboard visibility, and deletion; API CRUD/validation coverage is in `scripts/smoke-test-api.sh`.

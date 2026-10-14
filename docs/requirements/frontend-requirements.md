# Teleport Frontend Requirements

## Purpose

Define the planned frontend requirements for the Teleport Travel Discovery & Trip Planning PoC.

## Project Foundation

- **Business problem:** Travelers need a simple way to discover places and keep trip plans and basic spending notes together. This proof of concept deliberately does not handle bookings or payments.
- **Target users:** Students, young adults, and personal travelers planning their own trips.
- **Project type:** Proof of Concept (POC).
- **Primary stakeholder:** The traveler using the planner. The project team builds the prototype; the course instructor reviews milestone evidence.
- **Primary workflow:** Browse a destination, create a trip, add destinations or activities to its itinerary, and record expenses.

## Functional Requirements

- **FR-01:** The user can browse destinations and see a useful loading, empty, or error state.
- **FR-02:** The user can search destinations and filter by category.
- **FR-03:** The user can open destination details and view associated activities.
- **FR-04:** The user can create, list, update, and delete trips.
- **FR-05:** The user can add a destination or one of its activities to a trip itinerary, change item order, and remove an item.
- **FR-06:** The user can create, list, update, and delete trip expenses.
- **FR-07:** The user can review the trip's itinerary and expense total in the trip detail view.
- **FR-08:** The user can view relevant trip information in a dashboard (planned; not yet implemented).

## Data Collection and Validation

| Workflow | User-entered data | Required / optional | Validation | Result shown |
| --- | --- | --- | --- | --- |
| Search destinations | Search text and category filter | Both optional | Search is trimmed by the API; category must match available destination categories to return results. | Matching destination cards, empty state, or API error state. |
| Create or edit a trip | Trip name, start date, end date | Name required; dates optional | Name must not be blank; name is limited to 120 characters in the frontend; when both dates are set, end date cannot precede start date. | Saved trip in the list or a validation/API error. |
| Add an itinerary item | Destination and optional activity | Destination required; activity optional | Exactly one destination or activity is sent; API requires a non-negative order. The UI chooses the next order. | Item appears in the ordered trip itinerary. |
| Add or edit an expense | Description, amount, expense date | Description and positive amount required; date optional | Description must not be blank and is limited to 200 characters in the frontend; amount must be greater than zero; omitted API date defaults to today. | Expense appears in the list and the total is recalculated. |

Destination and activity records are curated project data, not fields entered by end users. Currency is currently USD in the frontend and should be treated as a prototype assumption, not currency conversion support.

## Non-Functional Requirements

- The interface should be responsive on desktop and mobile screens.
- The interface should use Angular with Nx and Bootstrap.
- The interface should provide clear navigation between planned features.
- Forms should provide understandable validation messages.
- The frontend should display useful loading and error states when connected to the API.
- The frontend should not connect directly to PostgreSQL.
- API/database credentials must be supplied through local environment settings or a secrets store; do not commit credentials.
- API operations should return explicit success, validation, and not-found responses.
- Interactive controls should have programmatic labels and keyboard-accessible actions.

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

The Angular/Nx frontend is implemented under `apps/web`. It includes destination discovery backed by the API, search and category filters, trip create/edit/delete, and a trip detail view for itinerary and expense workflows. Destination data is retrieved through the API; the API accesses PostgreSQL.

Trip, itinerary, expense, destination-detail, and activity database-backed workflows require further end-to-end verification. The dashboard and authentication remain outside the current POC implementation.

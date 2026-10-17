# RaceDay API Endpoint Plan

## 1. Purpose

The RaceDay API will provide the operations required to manage users, events, event categories, participant enrolments and race results.

The API is designed to work with the Part 1 database and ERD. Endpoint names and responsibilities are organised around the database entities.

---

## 2. User Roles

### Organiser

Organisers can:
- Create and manage events
- Create/manage event categories
- View participant enrolments
- Record race results

### Participant

Participants can:
- Register/login
- View available events
- View event categories
- Enrol in an event category
- View their own enrolments
- View their own results

---

## 3. Authentication Endpoints

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | `/api/auth/register` | Register a new user | Public |
| POST | `/api/auth/login` | Authenticate a user and return an access token | Public |
| GET | `/api/auth/me` | Return the currently authenticated user's details | Authenticated |

### POST `/api/auth/register`

Creates a new user.

Example request:

```json
{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com",
  "password": "Password123"
}
```

The password must be securely hashed before it is stored in `Users.PasswordHash`.

### POST `/api/auth/login`

Authenticates the user using their email and password.

Example request:

```json
{
  "email": "john.smith@example.com",
  "password": "Password123"
}
```

A successful response should provide an authentication token and the user's role.

---

## 4. Event Endpoints

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | `/api/events` | List events | Public |
| GET | `/api/events/{eventId}` | View one event | Public |
| POST | `/api/events` | Create an event | Organiser |
| PUT | `/api/events/{eventId}` | Update an event | Organiser |
| DELETE | `/api/events/{eventId}` | Delete/cancel an event | Organiser |

### GET `/api/events`

Returns available events.

Example response:

```json
[
  {
    "eventId": 1,
    "eventName": "Johannesburg Spring Run",
    "eventDate": "2026-10-10",
    "location": "Johannesburg",
    "status": "Open"
  }
]
```

### POST `/api/events`

Creates an event.

Example request:

```json
{
  "eventName": "Johannesburg Spring Run",
  "description": "Road running event",
  "eventDate": "2026-10-10",
  "location": "Johannesburg"
}
```

The authenticated organiser becomes the `OrganiserID`.

---

## 5. Category Endpoints

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | `/api/categories` | List categories | Public |
| GET | `/api/categories/{categoryId}` | View one category | Public |
| POST | `/api/categories` | Create a category | Organiser |
| PUT | `/api/categories/{categoryId}` | Update a category | Organiser |
| DELETE | `/api/categories/{categoryId}` | Delete a category | Organiser |

Example category:

```json
{
  "categoryName": "10 km Road Race",
  "distanceKm": 10.00
}
```

---

## 6. Event Category Endpoints

`EventCategories` resolves the many-to-many relationship between `Events` and `Categories`.

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | `/api/events/{eventId}/categories` | List categories offered by an event | Public |
| POST | `/api/events/{eventId}/categories` | Add a category to an event | Organiser |
| PUT | `/api/events/{eventId}/categories/{categoryId}` | Update fee/capacity | Organiser |
| DELETE | `/api/events/{eventId}/categories/{categoryId}` | Remove a category from an event | Organiser |

Example request:

```json
{
  "categoryId": 2,
  "entryFee": 150.00,
  "capacity": 500
}
```

---

## 7. Enrolment Endpoints

| Method | Endpoint | Description | Access |
|---|---|---|---|
| POST | `/api/events/{eventId}/categories/{categoryId}/enrolments` | Enrol the current participant | Participant |
| GET | `/api/enrolments/{enrolmentId}` | View an enrolment | Participant/Organiser |
| GET | `/api/me/enrolments` | View current participant's enrolments | Participant |
| GET | `/api/events/{eventId}/enrolments` | View enrolments for an event | Organiser |
| PUT | `/api/enrolments/{enrolmentId}/cancel` | Cancel an enrolment | Participant |

Example enrolment request:

```json
{
  "categoryId": 2
}
```

The API obtains `ParticipantID` from the authenticated user rather than allowing a participant to enrol someone else by supplying another user's ID.

---

## 8. Results Endpoints

| Method | Endpoint | Description | Access |
|---|---|---|---|
| GET | `/api/enrolments/{enrolmentId}/result` | View a result | Participant/Organiser |
| GET | `/api/me/results` | View current participant's results | Participant |
| POST | `/api/enrolments/{enrolmentId}/result` | Record a result | Organiser |
| PUT | `/api/enrolments/{enrolmentId}/result` | Update a result | Organiser |

Example result:

```json
{
  "finishTime": "00:52:18",
  "position": 21,
  "resultStatus": "Recorded"
}
```

---

## 9. Database-to-API Mapping

| Database Table | Main API Resource |
|---|---|
| `Roles` | User role/authentication information |
| `Users` | `/api/auth`, `/api/me` |
| `Events` | `/api/events` |
| `Categories` | `/api/categories` |
| `EventCategories` | `/api/events/{eventId}/categories` |
| `Enrolments` | `/api/enrolments` and event enrolment routes |
| `Results` | `/api/enrolments/{enrolmentId}/result` and `/api/me/results` |

This mapping keeps the API aligned with the ERD and SQL database.

---

## 10. Validation Rules

The API should validate:

1. Required fields are not empty.
2. Email addresses are valid and unique.
3. Passwords are never stored as plain text.
4. `RoleID` must reference an existing role.
5. `OrganiserID` must reference an existing user.
6. `CategoryID` and `EventID` must reference existing records.
7. An enrolment can only use an event/category combination that exists in `EventCategories`.
8. A participant cannot create an enrolment for another participant.
9. An event category cannot exceed its capacity.
10. A participant cannot enrol in the same event/category more than once.
11. `EntryFee` cannot be negative.
12. `Capacity` must be greater than zero.
13. Result positions must be positive when supplied.
14. An enrolment can have at most one result.

---

## 11. Recommended HTTP Status Codes

| Status | Meaning |
|---|---|
| 200 | Request successful |
| 201 | Resource created |
| 204 | Successful request with no response body |
| 400 | Invalid request |
| 401 | Authentication required/failed |
| 403 | User does not have permission |
| 404 | Resource not found |
| 409 | Conflict, such as duplicate enrolment |
| 500 | Unexpected server error |

---

## 12. Example API Flow

A typical participant workflow is:

```text
Register
   ↓
Login
   ↓
Receive authentication token
   ↓
GET /api/events
   ↓
GET /api/events/{eventId}/categories
   ↓
POST /api/events/{eventId}/categories/{categoryId}/enrolments
   ↓
GET /api/me/enrolments
   ↓
GET /api/me/results
```

A typical organiser workflow is:

```text
Login
   ↓
POST /api/events
   ↓
POST /api/events/{eventId}/categories
   ↓
GET /api/events/{eventId}/enrolments
   ↓
POST /api/enrolments/{enrolmentId}/result
   ↓
PUT /api/enrolments/{enrolmentId}/result
```

---

## 13. Part 1 Traceability

The design links the three Part 1 artefacts as follows:

```text
ERD
 │
 ├── defines entities and relationships
 │
 ▼
SQL Database
 │
 ├── implements tables, PKs and FKs
 │
 ▼
API Endpoint Plan
 │
 └── exposes operations for the database resources
```

The ERD, SQL script and API endpoint plan should remain consistent. If an entity, field or relationship changes in the ERD, the SQL and API documentation should be reviewed and updated accordingly.

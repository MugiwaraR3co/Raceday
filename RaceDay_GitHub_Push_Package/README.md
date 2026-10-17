# RaceDay Part 2 — RESTful API

This repository preserves the original RaceDay Part 1 planning/database documents and adds an ASP.NET Core 10 Web API implementation aligned to the Part 1 tables: Roles, Users, Events, Categories, EventCategories, Enrolments and Results.

## What is included

- JWT authentication and role-based authorisation (`Organiser`, `Participant`).
- Password hashing with ASP.NET Core `PasswordHasher<User>`; plain-text passwords are never stored.
- Profile view/update and password change endpoints.
- CRUD-style event/category management; event delete is a safe cancellation rather than destructive deletion.
- Event category offerings, participant enrolments, capacity/duplicate checks and result recording.
- Swagger/OpenAPI with Bearer token support and XML summaries.
- Integration tests using `WebApplicationFactory` and an in-memory EF Core provider.
- GitHub Actions workflow that restores, builds, runs tests and uploads a test report.

## Important compatibility notes

1. The repository's SQL script starts by dropping and recreating the `RaceDay` database. **Do not run it against a database containing data you need.** Use a development/test database only.
2. This project targets .NET 10. Install the .NET 10 SDK and Visual Studio 2022 with the ASP.NET and web development workload.
3. `appsettings.json` is configured for SQL Server LocalDB. If you use SQL Server Express or a named SQL Server instance, change `ConnectionStrings:RaceDay` accordingly.
4. The JWT key in appsettings is a development placeholder. Replace it for local use and store real secrets in user-secrets/environment variables; never commit production secrets.
5. The API maps to the Part 1 schema and expects the database and `Organiser`/`Participant` rows to exist. The existing Part 1 sample users use placeholder password hashes and cannot log in. Register a participant through the API. Create an organiser account using a trusted database/admin setup, not a public role-selection field.
6. This source bundle was generated in an environment without the .NET SDK, so it could not be compiled here. Run the commands below and address any environment-specific issues before submitting; do not describe CI as green until GitHub Actions actually passes.

## 1. Database setup

Open SQL Server Management Studio, connect to your local development SQL Server, open `docs/RaceDay_Database.sql` from your existing repository and execute it **only if it is safe to drop/recreate the RaceDay database**. Otherwise create a separate development database and apply the schema there. Update the connection string in `RaceDay.Api/appsettings.json`.

## 2. Open/build the solution

Open `RaceDay.sln` in Visual Studio 2022, or from a terminal in this directory run:

```bash
dotnet restore RaceDay.sln
dotnet build RaceDay.sln
dotnet test RaceDay.Tests/RaceDay.Tests.csproj
```

Run the API:

```bash
dotnet run --project RaceDay.Api
```

Open the HTTPS URL shown in the terminal followed by `/swagger`. If Swagger does not load, confirm the Development environment is active.

## 3. Test protected endpoints in Swagger

1. `POST /api/auth/register` to create a Participant.
2. `POST /api/auth/login` with the same email/password and copy `accessToken`.
3. Click **Authorize** in Swagger and enter `Bearer YOUR_TOKEN` (or paste the token if Swagger's Bearer scheme prompts for it).
4. Participant token can browse events, enrol, view own enrolments and own results. It should receive 403 when trying to create events or record results.
5. Organiser role is provisioned by a trusted administrator. Do not add a public `role` field to registration. Log in with an organiser account, Authorize with its token, then create events/categories, attach offerings, view enrolments and record results.

## Endpoints

| Method | Endpoint | Access | Purpose |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Register Participant |
| POST | `/api/auth/login` | Public | Login and return JWT |
| GET/PUT | `/api/auth/me` | Authenticated | View/update profile |
| PUT | `/api/auth/change-password` | Authenticated | Change password |
| GET | `/api/events`, `/api/events/{id}` | Public | Browse events |
| POST/PUT/DELETE | `/api/events[/ {id}]` | Organiser | Create/update/cancel event |
| GET | `/api/categories[/ {id}]` | Public | Browse categories |
| POST/PUT/DELETE | `/api/categories[/ {id}]` | Organiser | Manage categories |
| GET | `/api/events/{eventId}/categories` | Public | View offerings |
| POST | `/api/events/{eventId}/categories?categoryId={id}` | Organiser | Add offering |
| PUT/DELETE | `/api/events/{eventId}/categories/{categoryId}` | Organiser | Update/remove offering |
| POST | `/api/enrolments` | Participant | Enrol in offering |
| GET | `/api/enrolments/mine` | Participant | View own enrolments |
| GET | `/api/enrolments?eventId={id}` | Organiser | View enrolments |
| DELETE | `/api/enrolments/{id}` | Owning Participant | Cancel own enrolment |
| GET | `/api/results`, `/api/results/mine` | Authenticated (filtered by role) | View results; participant sees own only |
| PUT | `/api/results/enrolment/{enrolmentId}` | Organiser | Capture/update result |

## 4. GitHub Actions

The workflow is `.github/workflows/raceday-part2-ci.yml`. Push the API and tests to your Part 2 branch; open the repository's **Actions** tab and wait for the **RaceDay Part 2 CI** workflow. Capture a screenshot of a successful green run and add it to the README/submission evidence only after it passes.

## 5. Suggested meaningful commit sequence

Use a branch named `part2-rest-api`. Make commits as you complete and verify actual units of work (do not create fake commits just to reach a number):

1. Create Part 2 branch and solution skeleton
2. Add API project and dependencies
3. Add entity models
4. Add EF Core database context
5. Configure SQL Server connection
6. Add request/response DTOs
7. Add password hashing service
8. Add JWT token service
9. Configure authentication/authorisation
10. Add registration endpoint
11. Add login endpoint
12. Add profile endpoints
13. Add event endpoints
14. Add category endpoints
15. Add event category endpoints
16. Add enrolment endpoints
17. Add results endpoints
18. Add Swagger documentation
19. Add integration test project
20. Add auth and role tests
21. Add event/enrolment/result tests
22. Add GitHub Actions build/test workflow
23. Document local setup and API demo
24. Add verified screenshots and video link

## 6. Submission evidence still required

- GitHub repository URL and clearly named Part 2 branch.
- Successful green GitHub Actions build screenshot.
- Screenshot of Swagger with endpoints visible and at least one successful request.
- YouTube video URL demonstrating the API, code structure, endpoints and automated tests.
- Run the build and tests locally and in GitHub Actions before submission. Never claim an unrun build/test passed.


## How Part 1 and Part 2 fit together

The original Part 1 deliverables remain under `docs/` and `Evidence/`. Part 2 adds `RaceDay.Api/`, `RaceDay.Tests/`, the solution files and `.github/workflows/raceday-part2-ci.yml`. The API's EF Core mappings correspond to the existing SQL Server tables; run the SQL script only against a disposable development database because it drops/recreates `RaceDay`.

## Evidence still required before submission

- Capture a screenshot of the real successful GitHub Actions run after pushing this code.
- Capture a screenshot of Swagger running locally.
- Record and add the required YouTube demonstration URL.
- Verify that `dotnet build RaceDay.sln --configuration Release` and `dotnet test RaceDay.sln --configuration Release` pass on your machine or in GitHub Actions.
- Add meaningful commits as actual features and tests are implemented; do not create empty commits just to reach the minimum.

# Code guide and testing notes

## Request lifecycle

1. An HTTP request enters an API controller.
2. `[ApiController]` automatically validates DTO annotations and returns 400 for invalid input.
3. EF Core (`RaceDayContext`) queries SQL Server.
4. `PasswordService` hashes/verifies passwords; `TokenService` issues a signed JWT after successful login.
5. JWT bearer middleware validates the signature, issuer, audience and expiration. The role claim is used by `[Authorize(Roles="Organiser")]` and `[Authorize(Roles="Participant")]`.
6. Controllers return standard HTTP statuses: 200 OK, 201 Created, 204 No Content, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found and 409 Conflict.

## Test coverage included

- Registration and subsequent login
- Invalid login
- Participant rejected from organiser-only event creation (403)
- Organiser event creation
- Participant enrolment and persistence
- Organiser result capture and participant viewing their own result
- Anonymous profile request rejected

Tests use `WebApplicationFactory<Program>` so they exercise the HTTP pipeline. The test factory replaces the SQL Server provider with an in-memory EF Core database. This avoids needing SQL Server in GitHub Actions; local API runs still use SQL Server. If tests expose provider-specific differences, fix the implementation/tests before submission.

## Important design/security choices

- Public registration always creates `Participant`; users cannot promote themselves to Organiser.
- Password hashes and database entities are not returned by API responses.
- Participant result queries are filtered to the participant ID from the JWT, not a user-supplied ID.
- Capacity and duplicate enrolment checks are performed before creating an enrolment. For a high-traffic production system, enforce capacity with a transaction/locking strategy as well as application checks.
- Event DELETE changes status to Cancelled so enrolments/results are not destroyed.
- The Part 1 SQL seed users have placeholder password hashes. Use fresh accounts or provision a real organiser securely.

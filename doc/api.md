# API Documentation (General)

## Purpose

TaskManager is a REST API for managing personal tasks (to-do) with authentication and session management.

The API is designed around:
- Short-lived access tokens for API calls
- Long-lived refresh tokens for session continuity
- A consistent response envelope (`ApiResponse<T>`)

---

## Authentication & Sessions

### Token model

- **Access Token (JWT)**: **5 minutes**
  - Sent by the client in the `Authorization` header.
  - Used to access protected endpoints.

- **Refresh Token**: **30 days**
  - Stored **client-side** as an **HttpOnly cookie** named `refresh_token`.
  - Stored **server-side** as a **hashed** value (never stored in plaintext).
  - Used to obtain a new access token when the access token expires.

### Client responsibilities

- Store and attach the access token:
  - `Authorization: Bearer <accessToken>`
- Allow cookies to be sent (important when using Postman or a browser client).

### Refresh behavior (high-level)

- The refresh token is read from the `refresh_token` cookie.
- The server hashes it and looks it up in the database.
- If valid and active:
  - A new access token is issued.
- If invalid / revoked / expired:
  - Refresh is denied and the client must re-authenticate.

### Logout behavior (high-level)

- Logout revokes the user session server-side (refresh token revocation).
- The `refresh_token` cookie is deleted client-side.

---

## Response Envelope

All API responses use a standard envelope:

```json
{
  "success": true,
  "message": "Human readable message",
  "data": {},
  "errors": null
}
```


## Fields

- `success` (boolean): Indicates if the request was successful.
- `message` (string): A human-readable message.
- `data` (object): The payload of the response.
- `errors` (object|null): Any errors that occurred during the request.

---

## Rate Limiting

Authenfication endpoints are rate-limited to reduce brute-force attemps:

If the limit is exceed, the API will return a `429 Too Many Requests` response with a message indicating that the rate limit has been exceeded and when the client can retry.

## Testing with Postman

This repository provides a Postman collection (to import) to test the API.

Recommended Postman setup:

- Use an environment with variables such as:
  - `baseUrl` (e.g. http://localhost:5000)
  - `accessToken` (set automatically by collection scripts if you add them)

- Ensure cookies are enabled in Postman so the refresh_token cookie is stored and sent automatically.

Notes:
- If running in HTTP (not HTTPS), cookies configured with Secure=true will not be stored/sent by clients.
- In local development/testing, cookie Secure should be conditional on HTTPS.
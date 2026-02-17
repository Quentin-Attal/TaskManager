# Security & Threat Model

## Authentication Model

- Access tokens (JWT) are short-lived (5 minutes).
- Refresh tokens are long-lived (30 days).
- Refresh tokens are stored:
  - As HttpOnly cookies client-side
  - Hashed in the database server-side
- Only one active refresh token per user.

---

## Threat Considerations

### Stolen Access Token

Impact:
- Limited to 5 minutes due to short lifetime.

Mitigation:
- Short expiration time.

---

### Stolen Refresh Token

Impact:
- Allows generation of new access tokens until expiration or revocation.

Mitigation:
- Stored in HttpOnly cookie (not accessible via JavaScript).
- Stored hashed in database.
- Revoked on logout.
- Only one active token per user.

---

### Brute Force Attacks

Risk:
- Credential stuffing or repeated login attempts.

Mitigation:
- Rate limiting on authentication endpoints.

---

### Database Compromise

Risk:
- Exposure of stored refresh tokens.

Mitigation:
- Refresh tokens stored hashed with server-side pepper.
- Plain refresh tokens never stored.

---

## Assumptions

- HTTPS is used in production.
- Secure cookie flag is enabled in production.
- The client properly handles access token storage.

---

## Session Model

The API currently allows one active refresh token per user.

This simplifies session management for demonstration purposes.

The model could be extended to support multiple concurrent sessions (multi-device) if required.

---

## Limitations

- No refresh token rotation implemented.
- Single session model (one refresh token per user).
- No device binding or fingerprinting.

These decisions simplify the system while maintaining reasonable security for a personal task management API.

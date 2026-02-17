# TaskManager

API REST de gestion de tâches (*to-do*) avec authentification JWT + refresh tokens, construite en .NET avec une architecture en couches (API / Application / Domain / Infrastructure).

## ✨ Features

- Authentification :
  - Register / Login
  - JWT (access token)
  - Refresh tokens stockés en base **hachés** + rotation + révocation
  - Logout
- Gestion des tâches (auth requise) :
  - Lister ses tâches
  - Créer une tâche
  - Marquer une tâche comme terminée
  - Supprimer une tâche
- Rate limiting sur les routes d’auth
- EF Core + migrations
- CI GitHub Actions :
  - build
  - `dotnet format --verify-no-changes`
  - tests unitaires + intégration

---

## 🧱 Architecture

Le repo est organisé en 4 couches :

- **API** : controllers, middleware, configuration, endpoints HTTP
- **Application** : services métier, interfaces (repositories/services), DTOs, réponses API
- **Domain** : entités
- **Infrastructure** : EF Core, repositories, services techniques

---

## ✅ Prérequis

- .NET SDK (version du projet / CI)
- Une base de données supportée par EF Core (selon la config)

---

## 🔐 Authentication Model

- Access Token (JWT): 5 minutes
- Refresh Token: 30 days
- Refresh token stored in HttpOnly cookie
- Refresh tokens stored hashed in database
- One active refresh token per user

---

### Flow

1. Login:
   - Returns access token
   - Sets refresh token cookie
2. Access token expires (5 min):
   - Client calls /api/auth/refresh
   - Server validates refresh token
   - New access token is issued
3. Logout:
   - Refresh token revoked in database
   - Cookie deleted

---

## ⚙️ Configuration

Crée un fichier `appsettings.json` (ou utilise des variables d’environnement) avec les sections suivantes (exemple) :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },
  "Jwt": {
    "Issuer": "TaskManager",
    "Audience": "TaskManager",
    "Key": "PUT_A_LONG_RANDOM_SECRET_HERE",
    "AccessTokenLifetimeMinutes": 15,
    "RefreshTokenLifetimeDays": 7
  },
  "TokenHash": {
    "Pepper": "PUT_A_SERVER_SIDE_PEPPER_HERE"
  }
}
```

---

## Future Improvements

- Refresh token rotation
- Multi-device session support
- Device binding
- Role-based authorization
- Pagination and filtering on tasks
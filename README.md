# 🎯 TicTacToe REST API

A concise REST API for playing Tic‑Tac‑Toe built with ASP.NET Core (.NET 9) and SQLite.  
Includes user registration/login (JWT), game lifecycle (open, join, play), paginated & filtered game listings, and user profile statistics.

---

## 🚀 Quick start

- Requirements: .NET 9 SDK  
- Build and run: `dotnet build dotnet run`
- Swagger UI: `https://localhost:8081/swagger`  
- Default SQLite DB file: `tictactoe.db`

Configuration (example connection string in `Program.cs`)

---

## ✅ Features

- 🔐 User registration and login using `ASP.NET Core Identity` (username + password)  
- 🔑 JWT authentication for protected endpoints  
- 🎮 Open a game, join an open game, play moves with server-side turn validation  
- 📄 Paginated listing (newest first) and filtered search by user, start time, status  
- 🔎 View game board/status and user profile (games played, wins, win %)  
- 🧭 Swagger/OpenAPI with JWT Bearer support  
- ⚡ Dapper for performant read queries; EF Core Identity for user store

---

## 🔒 Authentication

- Register: `POST /api/auth/register`  
  Body example: `{ "username": "alice", "password": "P@ssw0rd!" }`
- Login: `POST /api/auth/login` — returns: `{ "token": "eyJhbGciOi..." }`
- Use token on protected calls: Authorization: `Bearer {token}`

---

## 🔗 Endpoints (summary)

### Auth
- `POST /api/auth/register` — register
- `POST /api/auth/login` — login (returns JWT)

### Games
- `GET /api/games` — paginated list (newest first)  
  - Query: `page` (default 1), `pageSize` (default 10)
- `GET /api/games/search` — filtered list  
  - Query params: `userId`, `startedFromUtc`, `startedToUtc`, `status`, `page`, `pageSize`  
  - Date-time format: ISO 8601 (e.g. `2025-12-22T16:02:00Z`)
- `POST /api/games` — open a new game (current user becomes `PlayerX`)
- `POST /api/games/{gameId}/join` — join an open game (becomes `PlayerO`)
- `GET /api/games/{gameId}/board` — get current board and next turn
- `POST /api/games/{gameId}/play` — play a move  
  Body example: `{ "row": 0, "col": 1 }`
  
### Users
- `GET /api/users/{userId}` — public profile (username, games played, wins, win %)

---

## 📦 Pagination & filtering

- Pagination: `page`, `pageSize`  
- Filtering supports any combination of:
  - `userId` (matches `PlayerX` or `PlayerO`)
  - `startedFromUtc` / `startedToUtc` (ISO‑8601)
  - `status` (`Open`, `InProgress`, `Finished`)
- Example date-time (no surrounding quotes in query fields):
  - `2025-12-22T16:02:00Z`

---

## 🗃️ Data model (overview)

- `Games` table: `Id`, `PlayerX`, `PlayerO`, `Board` (CSV 9 cells), `NextTurn`, `Winner`, `CreatedAt`, `StartedAt`, `Status`  
- `AppUser` (Identity): `Id`, `UserName`, `GamesPlayed`, `Wins`

---

## 🧰 Packages / libraries used

- `Dapper` — efficient SQL mapping for read queries  
- `Microsoft.EntityFrameworkCore.Sqlite` — EF Core SQLite provider (Identity backing store)  
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` — Identity integration with EF Core  
- `Swashbuckle.AspNetCore` — Swagger / OpenAPI UI  
- `Microsoft.AspNetCore.Authentication.JwtBearer` / `System.IdentityModel.Tokens.Jwt` — JWT support

---

## 🐳 Docker

Multi-stage `Dockerfile` included (targets .NET 9): 

`docker build -t tictactoe-api . docker run -p 8080:80 -e ASPNETCORE_ENVIRONMENT=Production tictactoe-api`

---

## 📌 Notes

- Use Swagger UI to explore endpoints and set the JWT via the `Authorize` button.  
- Date-time query parameters should be provided in ISO‑8601 format (e.g. `2025-12-22T16:02:00Z`).  
- The repository separates domain rules (game logic) from orchestration (services) and uses a small, focused service for player stats to avoid coupling.

---

  

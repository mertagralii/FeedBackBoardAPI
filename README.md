# FeedBackBoardAPI

A feedback board REST API built with **ASP.NET Core Web API** — users post feedback, categorize it, track its status and discuss it in threaded comments, while admins manage users and roles.

![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Identity](https://img.shields.io/badge/ASP.NET_Identity-000000?style=flat-square)
![AutoMapper](https://img.shields.io/badge/AutoMapper-BE0028?style=flat-square)

## Features

- **Feedback** — create, update, delete and list feedback items, filter them by category
- **Threaded comments** — comment on a feedback item, and reply to an existing comment (two levels)
- **Status & category** — every feedback item carries a status and a category, so the board can be filtered like a Kanban view
- **Authentication** — ASP.NET Core Identity with register/login endpoints; write operations require an authenticated user
- **Role-based admin area** — a separate controller restricted to the `Admin` role for user management, role promotion and profile images
- **DTO layer** — requests and responses are mapped with AutoMapper, so entities are never exposed directly

## Tech stack

| Concern | Technology |
|---|---|
| API | ASP.NET Core Web API |
| Persistence | Entity Framework Core, SQL Server, code-first migrations |
| Auth | ASP.NET Core Identity, role-based authorization |
| Mapping | AutoMapper (`MappingProfile`) |

## Endpoints

### Feedback — `/Feedback`

| Method | Endpoint | Auth |
|---|---|---|
| `GET` | `/Feedback/GetAllFeedback` | — |
| `GET` | `/Feedback/GetFeedbackByCategory` | — |
| `GET` | `/Feedback/GetFeedbackDetailsAndComment` | — |
| `POST` | `/Feedback/AddFeedback` | ✅ |
| `POST` | `/Feedback/UpdateFeedback` | ✅ |
| `DELETE` | `/Feedback/DeleteFeedback` | ✅ |

### Comments — `/Feedback`

| Method | Endpoint | Auth |
|---|---|---|
| `POST` | `/Feedback/AddComment` | ✅ |
| `PUT` | `/Feedback/UpdateCommentDto` | ✅ |
| `DELETE` | `/Feedback/DeleteComment` | ✅ |
| `POST` | `/Feedback/CommentOnTheComment` | ✅ |
| `PUT` | `/Feedback/UpdateCommentOnTheComment` | ✅ |
| `DELETE` | `/Feedback/DeleteCommentOnTheComment` | ✅ |

### User management — `/Use` (Admin role only)

| Method | Endpoint |
|---|---|
| `GET` | `/Use/GetUsersList` |
| `GET` | `/Use/GetUser` |
| `PUT` | `/Use/UpdateUser` |
| `DELETE` | `/Use/DeleteUser` |
| `POST` | `/Use/MakeAdmin` |
| `POST` | `/Use/MakeUser` |
| `POST` | `/Use/{userEmail}/UpdateProfileImage` |

Registration and login are exposed through the ASP.NET Core Identity API endpoints registered in `Program.cs`.

## Getting started

**Requirements:** .NET SDK · SQL Server (or LocalDB)

```bash
git clone https://github.com/mertagralii/FeedBackBoardAPI.git
cd FeedBackBoardAPI
```

Set the connection string in `appsettings.json`, then:

```bash
dotnet ef database update
dotnet run
```

Swagger is available at `/swagger` while running in development.

## Author

**Mert Ağralı** — [GitHub](https://github.com/mertagralii) · [LinkedIn](https://www.linkedin.com/in/mertagrali/)

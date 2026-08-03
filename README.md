# Assignment & Submission Management System

A complete, production-quality, role-based **Assignment & Submission Management System** built with **ASP.NET Core (.NET 10)** following **Clean Architecture**, **CQRS with MediatR**, **Database-First EF Core** on **PostgreSQL**, and a modern **Next.js (App Router)** frontend with **TypeScript** & **Tailwind CSS**.

---

## 🌟 Features & Role Capabilities

### 👑 Admin
- **User Management**: Create, view, update, and deactivate Admin, Teacher, and Student accounts.
- **Academic Setup**: Create and manage Classes/Courses and Subjects.
- **Teacher Allocations**: Map teachers to specific subjects and classes/courses.
- **Student Enrollments**: Enroll students into classes/courses.
- **System Visibility**: Full read visibility into all system assignments and student submissions.

### 👩‍🏫 Teacher
- **Assignment Lifecycle**: Create, update, publish, or delete assignments for subjects/classes they are assigned to teach.
- **Draft & Publish Controls**: Keep assignments in `Draft` state (hidden from students) until ready, or publish immediately.
- **Submission Reviews**: View all student submissions for created assignments.
- **Grading & Feedback**: Assign marks obtained (`MarksObtained <= MaxMarks`) and provide constructive feedback to students.
- **Status Management**: Mark submission statuses (`Graded`, `ResubmissionRequired`, `Late`).

### 🎓 Student
- **Assignment Feed**: View published assignments assigned to their enrolled class/course.
- **Submission Portal**: Submit text solutions and/or file attachments for published assignments before deadlines.
- **Resubmission Support**: Update submissions before the deadline or when resubmissions are explicitly allowed.
- **Grades & Feedback**: Real-time view of marks obtained, teacher feedback, and submission status (`Submitted`, `Late`, `Graded`, `ResubmissionRequired`).

---

## 🛠️ Technology Stack

| Layer | Technology |
|---|---|
| **Frontend** | Next.js (App Router), React 19, TypeScript, Tailwind CSS, Lucide Icons, Typed API Client |
| **Backend API** | ASP.NET Core Web API (.NET 10), C#, RESTful endpoints |
| **Architecture** | Clean Architecture (Domain, Application, Infrastructure, Api, Tests) |
| **CQRS Pattern** | MediatR pipeline behaviors for FluentValidation and request logging |
| **ORM & Database** | PostgreSQL 18 + EF Core (Database-First mode via `schema.sql` and `seed.sql`) |
| **Security & Auth** | JWT Bearer Authentication, BCrypt Password Hashing (`BCrypt.Net-Next`) |
| **Testing** | xUnit, Moq, FluentAssertions |
| **API Documentation** | Swagger / OpenAPI UI with JWT Bearer security integration |

---

## 📁 Solution Architecture

The backend strictly follows **Clean Architecture** boundaries with dependencies flowing inward only:

```
AssignmentProject/
├── database/
│   ├── schema.sql              # PostgreSQL DDL script (tables, constraints, indexes)
│   └── seed.sql                # Seed data with demo accounts and assignments
│
├── backend/
│   ├── Domain/                 # Enterprise Domain layer: Entities, Enums, Exceptions
│   │   ├── Entities/           # User, ClassCourse, Subject, Assignment, Submission, etc.
│   │   ├── Enums/              # UserRole, AssignmentStatus, SubmissionStatus
│   │   └── Exceptions/         # DomainException, NotFoundException, ForbiddenException, etc.
│   │
│   ├── Application/            # CQRS commands/queries (MediatR), DTOs, interfaces, validators
│   │   ├── Common/             # Interfaces (IRepository, IUnitOfWork), Behaviors (Validation, Logging)
│   │   ├── DTOs/               # Auth, User, Academic, Assignment, Submission DTOs
│   │   └── Features/           # CQRS feature handlers (Auth, Users, Academic, Assignments, Submissions)
│   │
│   ├── Infrastructure/         # EF Core DbContext, Repositories, JWT generator, PasswordHasher
│   │   ├── Persistence/        # AppDbContext, Repository implementations, UnitOfWork
│   │   ├── Authentication/     # JwtTokenGenerator, PasswordHasher (BCrypt)
│   │   └── Services/           # CurrentUserService (ClaimsPrincipal reader)
│   │
│   ├── Api/                    # Presentation layer: ASP.NET Core controllers, middleware, Swagger
│   │   ├── Controllers/        # Auth, Users, Academic, Assignments, Submissions controllers
│   │   ├── Middleware/         # ExceptionHandlingMiddleware (catches & wraps errors in ApiResponse<T>)
│   │   ├── Models/             # Standard ApiResponse<T> wrapper
│   │   └── Program.cs          # DI registration, CORS, Authentication, Swagger config
│   │
│   └── Tests/                  # xUnit unit test suite for business & authorization rules
│
└── frontend/                   # Next.js App Router, TypeScript & Tailwind CSS
    ├── src/
    │   ├── app/                # Next.js App Router pages (login, dashboard)
    │   ├── components/         # Navigation, Admin, Teacher, Student dashboards
    │   ├── context/            # AuthContext provider
    │   ├── lib/                # Typed API client handling Jwt headers & error parsing
    │   └── types/              # TypeScript interface contracts
```

---

## 🔑 Standard API Response Model (`ApiResponse<T>`)

Per mandatory requirements (Section 0.5), **every single API endpoint** returns JSON wrapped in this exact shape:

```csharp
namespace BdjobsMIS.Aggregator.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(bool success, int statusCode, string message, T? data = default)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
    }
}
```

---

## 🔑 Demo Credentials

Use these seeded accounts to log in and test role-specific functionalities:

| Role | Email | Password | Pre-seeded Features |
|---|---|---|---|
| **Admin** | `admin@school.com` | `Admin@123` | User creation/deactivation, Class & Subject management, Allocations |
| **Teacher** | `teacher@school.com` | `Teacher@123` | Assigned to Grade 10 Advanced Math & CS, 1 Published & 1 Draft assignment |
| **Student** | `student@school.com` | `Student@123` | Enrolled in Grade 10 Science & Tech, 1 Sample submission graded (95/100) |

---

## 🚀 Setup & Execution Guide

### Prerequisites
- **.NET 10 SDK** installed (`dotnet --version`)
- **Node.js 20+** and **npm** installed (`node -v`, `npm -v`)
- **PostgreSQL 15+** server running locally on port `5432`

---

### Step 1: Database Setup (PostgreSQL)

Run `schema.sql` and `seed.sql` against PostgreSQL:

```bash
# Set password for psql execution (default: postgres)
export PGPASSWORD=postgres

# 1. Create database
psql -U postgres -h 127.0.0.1 -c "CREATE DATABASE assignment_db;"

# 2. Run DDL schema script
psql -U postgres -h 127.0.0.1 -d assignment_db -f "database/schema.sql"

# 3. Run Seed script
psql -U postgres -h 127.0.0.1 -d assignment_db -f "database/seed.sql"
```

*(On Windows PowerShell: `$env:PGPASSWORD="postgres"; & "C:\Program Files\PostgreSQL\18\bin\psql.exe" -U postgres -h 127.0.0.1 -d assignment_db -f "database/schema.sql"`)*

---

### Step 2: Run Backend API (.NET 10)

```bash
cd backend/Api
dotnet run
```

- **Scalar API Reference UI**: `http://localhost:5000/scalar/v1`
- **Swagger / OpenAPI Interface**: `http://localhost:5000/swagger`
- **Base API Endpoint**: `http://localhost:5000/api`

---

### Step 3: Run Frontend (Next.js)

```bash
cd frontend
npm install
npm run dev
```

- **Application URL**: `http://localhost:3000`

---

### Step 4: Run Unit Tests

Execute the xUnit test suite covering all core business rules and authorization policies:

```bash
cd backend
dotnet test AssignmentSystem.slnx
```

---

## 📌 Key Business Rules Implemented & Tested

1. **Draft Visibility Constraint**: Students cannot view or submit answers to `Draft` assignments.
2. **Class Enrollment Enforcement**: Students can only view and submit assignments for classes they are enrolled in.
3. **Deadline & Late Submission Policy**: Submissions past the deadline are rejected with `DeadlinePassedException` unless `AllowResubmission` is enabled on the assignment. When allowed past deadline, submission status is automatically marked as `Late`.
4. **Teacher Subject Ownership**: Teachers can only create assignments for subjects and classes they are explicitly assigned to teach.
5. **Max Marks Validation**: `MarksObtained` on a submission cannot be negative or exceed `MaxMarks`.
6. **Grading State Lock**: Students cannot update or modify a submission once it has been graded (`Status == Graded`).
7. **Strict Server-Side Authorization**: Endpoints enforce role authorization (`[Authorize(Roles = "...")]`) server-side, returning standard `401 Unauthorized` or `403 Forbidden` wrapped responses.

---

## 💡 Assumptions & Design Decisions

1. **Database-First Schema Authority**: `database/schema.sql` serves as the authoritative source of truth. Clean domain entities in `Domain` decouple business rules from persistence mechanisms.
2. **Password Security**: Passwords are saved as BCrypt hashes using cost factor 11 (`BCrypt.Net-Next`). Plaintext passwords are never stored or logged.
3. **CQRS with MediatR**: Controllers remain thin and delegates all requests to MediatR handlers. FluentValidation executes at the pipeline level before reaching handlers.
4. **Token Storage**: JWT tokens are issued on login and attached via `Authorization: Bearer <token>` header in client requests.

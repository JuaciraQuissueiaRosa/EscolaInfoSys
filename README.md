EscolaInfoSys – School Management System

A modern and modular school management system developed with ASP.NET Core MVC and Entity Framework Core, featuring full user authentication, role-based access, dashboards, email confirmation, real-time SignalR notifications, and automated student exclusion logic based on attendance.
---

Architecture Overview

The solution follows a clean, layered and service-oriented architecture:

Frontend: ASP.NET Core MVC with custom layout using Tabler and Font Awesome (no Bootstrap themes)

Backend: ASP.NET Core MVC + Identity + RESTful endpoints

Database: SQL Server (hosted on Somee)

Real-Time Communication: SignalR

Image/File Uploads: Handled via IFormFile in Razor views

Mobile Support: Compatible with .NET MAUI (via API) – (optional, external project)
---

Features
✅ Full user authentication (login, password reset, email confirmation)
✅ Role-based access control (Administrator, StaffMember, Student)
✅ Student exclusion logic based on attendance threshold
✅ Dashboards personalised per role
✅ Upload and display of student profile photo and document image
✅ Real-time notifications using SignalR (alerts, new marks, absences)
✅ Syncfusion DataGrid and ProgressBar
✅ Modern UI with Tabler UI and Font Awesome 6.4.0
✅ Fully responsive views (no Blazor used)
✅ Email sending via SMTP
✅ Custom 403, 404, and 500 error pages
✅ API endpoint:
GET /api/students/by-formgroup?id=1

---

Technologies Used

.Backend
ASP.NET Core MVC (.NET 8)

Entity Framework Core

ASP.NET Identity (User/Role management)

SignalR (real-time notifications)

SMTP (email services)

RESTful Web API (basic endpoint)

---
.Database
SQL Server

Migrations with EF Core (Code-First)

Hosted on Somee

----
.Frontend

Razor Views (CSHTML)

Tabler UI Framework (CSS/JS)

Font Awesome (icons)

Bootstrap 5 (basic grid and layout support)

Syncfusion (Community License – DataGrid, ProgressBar)

jQuery and custom JavaScript

.File Uploads

Student profile picture

Document photo (e.g. ID card)

Stored in /wwwroot/uploads

.How to Run

### 1. Clone the repository:

```bash
git clone https://github.com/JuaciraQuissueiaRosa/EscolaInfoSys.git


##  Video Demonstration

Click below to watch the full demo:

[▶️ Watch Demo](https://drive.google.com/file/d/SEU-LINK-DO-VIDEO/view?usp=sharing)

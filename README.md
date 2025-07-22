# EscolaInfoSys – School Management System

A modern and modular school management system developed with **ASP.NET Core MVC** and **Entity Framework Core**, featuring full user authentication, role-based access, dashboards, email confirmation, notifications via SignalR, and student exclusion logic based on attendance.

---

##  Architecture Overview

The solution follows a layered and service-oriented architecture:

- **Frontend:** ASP.NET Core MVC with custom layout (no external CSS frameworks)
- **Backend:** ASP.NET Core Web API (RESTful)
- **Database:** SQL Server (hosted on Azure)
- **Real-Time Communication:** SignalR
- **Mobile Support:** .NET MAUI (optional, separate project)

---

## Features

- ✅ User authentication & registration with email confirmation  
- ✅ Role-based access (Administrator, Staff, Student)  
- ✅ CRUD operations for:
  - Students (with profile & document photo upload)
  - Staff Members
  - Courses & Subjects
  - Form Groups & Marks
  - Alerts & Absences
- ✅ Real-time notifications via SignalR
- ✅ Automatic student exclusion based on system-configured absence threshold
- ✅ Dashboards tailored per role
- ✅ Custom 403, 404, and 500 error pages
- ✅ Modern UI built with Tabler & Font Awesome
- ✅ API endpoint: `/api/students/by-formgroup?id=1`

---

##  Technologies Used

- ASP.NET Core MVC (.NET 8)  
- Entity Framework Core  
- ASP.NET Identity  
- SignalR  
- SQL Server + Somee
- Syncfusion (UI components)  
- Git & GitHub

---

## How to Run

### 1. Clone the repository:

```bash
git clone https://github.com/JuaciraQuissueiaRosa/EscolaInfoSys.git


##  Video Demonstration

Click below to watch the full demo:

[▶️ Watch Demo](https://drive.google.com/file/d/SEU-LINK-DO-VIDEO/view?usp=sharing)

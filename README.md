# ⏱️ Quartz Scheduler Web App

A complete job scheduling system built using **Quartz.NET**, **Blazor WebAssembly**, and **ASP.NET Core Web API**.  
This application allows users to create, manage, and monitor scheduled jobs directly from a modern UI.

---

## 🚀 Features

- ✅ Create, update, and delete scheduled jobs
- ✅ Pause and resume jobs anytime
- ✅ Track job execution history
- ✅ Flexible cron expression scheduling
- ✅ Email job scheduling (automated emails)
- ✅ HTTP API job scheduling (external API calls)
- ✅ Custom middleware for centralized error handling
- ✅ Logging with Serilog (logs stored in database)
- ✅ Clean and modern Blazor WebAssembly UI
- ✅ RESTful API backend

---

## 🛠️ Tech Stack

### Frontend
- Blazor WebAssembly
- Bootstrap (UI Styling)

### Backend
- ASP.NET Core Web API
- Quartz.NET (Job Scheduler)

### Database
- SQL Server

### Logging & Middleware
- Serilog (Database logging)
- Custom Exception Handling Middleware

---

## 📧 Job Types

### 1. Email Jobs
- Schedule automated emails
- Supports cron expressions
- Useful for:
  - Reports
  - Notifications
  - Alerts

### 2. HTTP API Jobs
- Call external APIs on schedule
- Supports:
  - Custom headers
  - Request body
- Ideal for integrations

---

## ⏰ Cron Examples

| Expression        | Description                          |
|------------------|--------------------------------------|
| `0 0 9 * * ?`     | Every day at 9:00 AM                |
| `0 */5 * * * ?`   | Every 5 minutes                     |
| `0 0 0 1 * ?`     | First day of every month            |
| `0 0 9 * * MON-FRI` | Weekdays at 9:00 AM             |
| `0 0 8,12,18 * * ?` | 8 AM, 12 PM, and 6 PM daily     |

---

## 🧠 Architecture

- **Blazor WASM** → UI Layer
- **ASP.NET Core API** → Business Logic
- **Quartz.NET** → Background Job Scheduler
- **Repository Pattern** → Clean data access
- **Middleware** → Centralized error handling
- **Serilog** → Logging to database
- **Entity Framework** → ORM

---

## 📂 Key Modules

- Job Management (CRUD)
- Scheduler Engine (Quartz)
- API Job Executor
- Email Service
- Logging System
- Middleware Pipeline

---

## ⚙️ Setup Instructions

1. Clone the repository:
   ```bash
   git clone <your-repo-url>

2. Update database connection string and email credentials in:
    - appsettings.json

3. Run migrations / create database
    - Update-Database -Project QuartzData -StartupProject QuartzAPI

4. Run Project

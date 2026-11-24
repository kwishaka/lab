###﻿Bicycle Rental Management System 
Understanding
Convert the starter .NET 10 console project into a Windows Forms bicycle rental management system using SQLite and an MVC-style separation. Implement the supplied requirements for authentication, role permissions, customers, bicycles, rentals, configurable hourly prices, payments, receipts, and reports.

Assumptions
SQLite is stored in a local bicycle-rental.db file beside the application.
The application is implemented with programmatic WinForms controls so no designer-generated files are required.
The supplied requirements are complete through FR27; receipt printing is implemented with a print preview/print document.
Initial accounts are admin / admin123 and attendant / attendant123, with administrator-only account management.
Rental times represent integer hour boundaries, and pricing uses the hour interval [start, end).
Approach
Change the project to a Windows-targeted .NET 10 WinForms executable and add the SQLite provider. Create model classes for the domain, a database initializer/repository layer for schema and persistence, services for authentication and pricing, and controllers that mediate form actions. Build login, role-based dashboard, and focused management dialogs/forms programmatically. Keep database access out of forms and expose only the operations needed by the UI.

Key Files
LAB 3 assignment.csproj - enables WinForms and adds SQLite.
Program.cs - application entry point and database initialization.
Models/* - domain entities and enums.
Data/Database.cs - SQLite schema, seed data, and database connection.
Data/Repositories.cs - persistence operations.
Services/* - authentication, pricing, rental, and reporting logic.
Views/* - WinForms UI.
Risks & Open Questions
Printing depends on the local Windows print subsystem; preview remains available when no printer is configured.
Existing Docker configuration is not suitable for WinForms and is left unused; the application runs on Windows.
Steps
Configure the project for .NET 10 Windows Forms and SQLite.
Create domain models and database initialization.
Create repositories and application services.
Create login and dashboard forms with role-based navigation.
Create management forms for users, customers, bicycles, rentals, payments, prices, and reports.
Add receipt printing and complete rental/payment workflows.
Build the solution and fix compilation issues.

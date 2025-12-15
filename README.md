# LoanAPI - RESTful API for loan management system with JWT authentication and role-based authorization (User, Accountant).

## Overview

Loan Management API provides functionality for: 

- User registration and authentication
- Loan creation, updating, and deletion
- Loan status management (Processing, Approved, Rejected)
- Accountant approval/rejection of loans
- User blocking/unblocking system
- Role-based access control


**Users** can create and manage their own loans (only while status is "Processing").  
**Accountants** can view all loans, approve/reject requests, block users, and delete any loan.  
**Blocked users** cannot create new loan requests.

---

## Technologies

- **ASP.NET Core** 
- **Entity Framework Core** 
- **SQL Server**  
- **JWT (JSON Web Token)** 
- **Serilog** 
- **FluentValidation** 
- **Swagger/OpenAPI** 

---


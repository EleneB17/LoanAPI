# LoanAPI 


## Overview
LoanAPI is a RESTful Web API built with ASP.NET Core.  
It implements a comprehensive loan management system with JWT authentication, role-based authorization, input validation, structured logging, and API documentation.



<br>

## Core Functionality:


- User registration and authentication
- Loan creation, updating, and deletion
- Loan status management (Processing, Approved, Rejected)
- Accountant approval/rejection of loans
- User blocking/unblocking system
- Role-based access control

---


<br>

## Technologies

- **ASP.NET Core** 
- **Entity Framework Core** 
- **SQL Server**  
- **JWT (JSON Web Token)** 
- **Serilog** 
- **FluentValidation** 
- **Swagger/OpenAPI** 

---

<br>

## Application Roles

### User

- Register and log in
- Create, update, and delete own loans
- Can only modify loans in **Processing** status
- Cannot access other users' loans

### Accountant

- View all loans in the system
- Approve or reject loan requests
- Block/unblock users to prevent loan creation
- Delete any loan regardless of status
  


---
<br>

## Main Endpoints

### Authentication

**Register User**
```http
POST /api/Auth/register
```
Registers a new user after validation.

**Login**
```http
POST /api/Auth/login
```
Authenticates a user and returns a JWT token.

---

### User Operations

**Get User Profile**
```http
GET /api/User/{id}
```
Returns user information by ID.

---

### Loan (User Role)

**Create Loan**
```http
POST /api/Loan
```
Creates a loan request (status starts as **Processing**).

**Get My Loans**
```http
GET /api/Loan/my-loans
```
Returns all loans belonging to the authenticated user.

**Get Loan by ID**
```http
GET /api/Loan/{id}
```
Returns a specific loan (only owner can access).

**Update Loan**
```http
PUT /api/Loan/{id}
```
Updates a loan (only owner, only if status is **Processing**).

**Delete Loan**
```http
DELETE /api/Loan/{id}
```
Deletes a loan (only owner, only if status is **Processing**).

---

### Accountant Operations

**Get All Loans**
```http
GET /api/Accountant/loans
```
Returns all loans in the system.

**Update Loan Status**
```http
PATCH /api/Accountant/loans/{id}/status
```
Approves or rejects a loan.

**Block/Unblock User**
```http
PATCH /api/Accountant/users/{id}/block
```
Blocks or unblocks a user from creating new loans.

**Delete Any Loan**
```http
DELETE /api/Accountant/loans/{id}
```
Deletes any loan regardless of status.

---





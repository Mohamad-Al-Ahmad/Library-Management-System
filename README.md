# Library Management System - ASP.NET Core API
A comprehensive library management system backend built with ASP.NET Core and Entity Framework, providing robust APIs for managing books, 
authors, members, and borrowing operations. The API utilizes a code-first approach, and follows modern development practices for 
maintainability and scalability.

# Features
1- Complete CRUD Operations: Full Create, Read, Update, Delete functionality.
2- Smart Validation: Duplicate prevention and relationship validation.
3- Advanced Borrowing System: Intelligent book tracking with automatic status updates.
4- Member Management: Unique email validation and membership tracking.
5- Dependency Injection: built-in dependency injection for loosely coupled components and better code maintainability.
6- Reporting Services: Advanced filtering, pagination, and sorting capabilities.
7- Comprehensive Error Handling: Sophisticated logging and exception handling.

# Special Features
- Prevent deletion of authors with existing books.
- Prevent deletion of borrowed books.
- Email uniqueness validation for members.
- Automatic book availability updates.
- Active borrows tracking.
- Get Borrow history by member/book.

# Technologies Used:
1-ASP.NET Core.
2-Entity Framework Core.
3-SQL Server.
4-Async/Await Programming.
5-Dependency Injection.
6-Repository Pattern.
7-LINQ.
8-AutoMapper

# How It Works
"Architecture Overview"

This project follows a layered architecture with separation of concerns:
- API Layer: Controllers handle HTTP requests and responses.
- Business Layer: Repositories contain the core logic and data access.
- Data Layer: Entity Framework Core with SQL Server for data persistence.

"Key Features Implementation"

-Pagination & Sorting: Implemented using Skip, Take, and dynamic LINQ ordering.
-Borrowing Logic: Automatic book availability tracking when borrowing/returning.
-Validation: Custom checks for duplicates (author name + city, member email).

# Contributing
Contributions are welcome! If you find any issues or have suggestions for improvements, feel free to open an issue or submit a pull request.


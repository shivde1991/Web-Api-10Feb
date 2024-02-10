## Test Overview

You are required to implement a dynamic web application that allows public users to smoothly search for books. The user interface should be simple and intuitive, featuring a single input field where users can input relevant book information. As the user types into this field, the application should dynamically display book records that match the entered text based on the title, author, publication date, or description.

## Technical Expectations

Your application should demonstrate:

- **Responsiveness and Efficiency:** The application should be optimized for both bandwidth and responsiveness to ensure a smooth user experience. You may consider implementing indexing or caching techniques to improve performance.
- **Security:** Implement best practices for input validation and protection against malicious attacks and vulnerabilities.
- **Architecture:** The solution should demonstrate best practices for layered structure, reusability, and inversion of control.
- **Frontend:** The frontend should be implemented using Angular 16.
- **Backend:** The backend should be implemented using .Net Core v6. 
- **Database:** We strongly prefer using Dapper for DB access over any other ORM technology.
- **Pagination:** The search results should implement a 'load-more' pagination approach, triggered when the user scrolls down.
- **Comprehensive Results:** Each result record should display all book information, including the book image.

*Note: You should expect that at the time of task assessment, we will be running your solution on our own data seed which might have much larger number of records*

## Environment Setup

Required tools include:

- Visual Studio 2022
- SQL Server 2019 (or later) installed in mixed-mode authentication

Please note that while you may add new tables to the database, modifications to the initial structure or initial data seed are not permitted.

## Getting Started

1. Open the solution in Visual Studio 2022.
2. Modify the `appsettings.json` for the `Migrations` project and run it (it might take a while to finish).
3. The project should create an SQL server database and seed it with random data.

We look forward to seeing your creative approach to this project! Your solution will provide valuable insight into your skills and approach to web development. Good luck!
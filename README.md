This project is a Store Visit Tracking System, developed to manage and track store visits.
The application allows users to create visits, upload related photos, and manage store and product data.
Admin users can manage stores, products, and view all visits, while standard users can view stores and create their own visits.

The system was built on .NET 8 and a MySQL database.
It features role-based authentication for different user types (Admins and Standard Users), allowing secure and structured access to the system's features.

Key features include:
User Registration & Authentication: Users can register and log in using JWT tokens for authentication.
Store Management: Admins can create, update, and delete stores.
Visit Tracking: Users can log visits, upload photos related to their visits, and track visit status.
Role-Based Access: Admins have full control over the system, while standard users can only view stores and create their own visits.
Data Storage: Data is stored securely in a MySQL database, including user, store, and visit information

Provided Swagger UI for documentation purpose - http://localhost:5240/swagger/index.html

Setup Instructions
To get started with the Store Visit Tracking System after downloading or cloning the repository, follow the steps below:

1. Clone the Repository
If you haven't cloned the repository yet, open a terminal or Git Bash and run the following command:
https://github.com/SamirGanbarli/VisitingTrackerWebApiApp.git

2. Run the Application
After setting up the database connection and applying migrations, you can run the application locally by executing the following command:
dotnet run

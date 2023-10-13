# Blazor Market

## Overview

Blazor Market is a web application that serves as a practical exploration of the Blazor framework. This repository focuses on creating a full-stack web application where an ASP.NET Core API provides endpoints for data management, while a Blazor Server App consumes these endpoints on the client side. In the process, several essential features are implemented, including JWT authentication and authorization through ASP.NET Identity.

## Features

- **Authentication and Authorization:** Blazor Market features user authentication and authorization using ASP.NET Identity. Only authorized users can access certain parts of the application, ensuring data security.

- **Product Management:** The application includes a Product model and controller, providing CRUD (Create, Read, Update, Delete) operations for managing products.

- **User-Friendly UI:** The user interface is designed with a user-friendly navbar, intuitive user flows, and a product tab for creating, viewing, and updating products.

## User Permissions

- **Unauthenticated Users:** Unauthenticated users can access the Home page and add new products.

- **Authenticated Users:** Authenticated users can log in, access the Home page, and manage their own products.

## Technologies

- **Blazor Server App:** The client side of the application is built using Blazor Server App, enabling rich, interactive web experiences.

- **ASP.NET Core API:** The back end is powered by an ASP.NET Core API that serves as the data gateway for the client.

- **SQL Server:** The application uses SQL Server as the database to store product information.

## Learning Objectives

Through this project, I have learned how to:

- Use the Blazor framework to create interactive web applications.
- Implement user authentication and authorization, securing data access.
- Build CRUD endpoints in an ASP.NET Core API to manage data.
- Consume API endpoints on the client side and perform tasks like login, creating, viewing, and updating products.

This repository can be a valuable resource for developers seeking hands-on experience in building secure web applications with Blazor. Feel free to explore the code, learn from it, and adapt it for your own projects.

Happy coding!

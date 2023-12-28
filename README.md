# Azure Blob Web App
Full stack web application built with React, C# and .NET, and SQL Server.  This app is a full cloud storage application that allows users to upload, download and delete file on Azure Blob.

## Main Features and design
- Full user authorisation and authentication, with role based access and JWT tokens
- Admins can set file type restrictions and size limits on files that a user can upload
- Soft delete feature to allow users to restore recently deleted files
- Search bar to filter files based on the name
- Layered/Onion architecture used on the backend to separate into layers: Presentation, API, Business, Data layer
- Entity framework used on the backend, and normalized tables on the database
- API layer built on the frontend in React as well to create a clear separation between components and API logic
- Bootstrap and Material UI used for many components, and others, like the file tiles, built from scratch

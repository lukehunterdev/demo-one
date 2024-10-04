# Demo One

An ASP.NET Core MVC demo. There are some instructions on the home page when running the application.

## Features

- Simple CRUD operations on a "customer" object.
- RESTful API interface, with Swagger and OpenAPI support. 
- Fully self-contained. IIS not required.
- OpenAPI definition is generated automatically, with proper documentation.
- User auth. You need to login to view or edit customers.
- API auth. Uses same credentials as User auth.
- JWT tokens used for proving identity. Bearer token on RESTful API, session cookie on web UI.
- For simplicity and portability, uses SQLite, but can be ported to MS SQL Server or others easily.
- Database is initialised automatically.
- Simple health checks when application is containerised. These do not reflect the state of the application as SQLite is used and not
  likely to be down. Just a demo.
- Swagger has a dark mode :)
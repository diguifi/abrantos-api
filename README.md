# Abrantos API
This is the API for Abrantos project.

## Abrantos?
Abrantos is a fun sideproject that aims to be a kind of social network for people with bad luck, 
where you can quantify how unlucky you were in a day and share your status with friends.

## This project contains
- .NET Core 2.2
- EF Core
- JWT implementation
- RESTful design
- Swagger UI
- Identity
- PostreSQL
- Dependency Injection
- Sendgrid support for emailing
- Complete social network environment

## Run the project
- Install [ASP.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2) and [Runtime](https://dotnet.microsoft.com/download/dotnet-core/2.2)
- Install [PostgreSQL](https://www.postgresql.org/download/)
- Create an env variable like so:  
Key: _AbrantosConnectionString_  
Value: _Server=localhost;Database=AbrantosDb;Port=5432;User Id=postgres;Password=YOURPGPASSWORD;_
- Run `dotnet ef database update`
- Finally: `dotnet run`

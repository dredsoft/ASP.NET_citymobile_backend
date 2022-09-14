## Technologies
CityApp is built using the following technologies: 

 - [Visual Studio 2017](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)
 - ASP.NET Core (Targeting .net 4.61)
 - [SQL Server 2014/Azure SQL for data storage](https://www.microsoft.com/en-us/download/details.aspx?id=42299)
 - Entity Framework Core for data access
 - Code First using Migrations
 - Dapper micro-ORM for report data access and for queries that EF can't handle easily
 - redis for caching
 - AWS S3 for file storage
 - Azure WebApps for hosting
 - Azure WebJobs for resource intensive background processing
 - Serilog for application logging
 - [Bootstrap/Material UI Theme](http://demos.creative-tim.com/material-dashboard-pro/examples/dashboard.html)
 - SendGrid for transactional emails

## Deploying
City App is hosted on Azure.

## Data Context
Since this is a multitenant app with a physical database per tenant, we have 2 data contexts.

 - CommonContext is used for all things common to the application like User Authentication, LookUp Values, Account(Tenant) information and Database partitions
 - AccountContext is used for the application business requirement specific data, like Issues, Tickets, Vendors

## Migration
### How to create a migration in the common context
 - Add-Migration -Name [Name_of_your_Migration] -Context CityApp.Data.CommonContext
### How to create a migration in account context
 - Add-Migration -Name [Name_of_your_Migration] -Context CityApp.Data.AccountContext
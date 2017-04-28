# Accelist.IdentityGenerator

> The official .NET development tool of Accelist Technology for cleanly generating Entity Framework Core's Entity Models from an existing SQL Server database. (Entity Framework Core stand-alone Reverse Engineering tool and API) 

![WPF Application Screenshot](https://raw.githubusercontent.com/ryanelian/Accelist.EntityGenerator/master/screenshot.PNG)

## Getting Started

- When running on Windows, have at least .NET Framework 4.5.1.
- [Click here to download latest build.](https://github.com/ryanelian/Accelist.EntityGenerator/raw/master/Dist/Accelist.EntityGenerator.Wpf.exe)

## Features:
- Designed for SQL Server and C#, when developing using database-first approach.
- Supported SQL types: `VARCHAR`, `CHAR`, `NVARCHAR`, `NCHAR`, `BIGINT`, `INT`, `SMALLINT`, `TINYINT`, `NUMERIC`, `DECIMAL`, `SMALLMONEY`, `MONEY`, `REAL`, `FLOAT`, `BIT`, `UNIQUEIDENTIFIER`, `VARBINARY`, `SMALLDATETIME`, `DATETIME`, `DATETIME2`, `TIME`
- Maps the type in accordance to appropriate C# data type. Provides null `?` hint for nullable columns.
- Supports composite primary keys.
- Generates the Entity Framework Core DbContext class, without impacting your project / solution!
- Save your configuration.
- Fast and easy to use!

## Not (Yet) Implemented:
- Navigation properties and inversion properties. *(Hint: improve your multi-join queries using Dapper instead!)*

## Accelist.EntityGenerator

> The official Accelist Technology .NET development tool for cleanly generating [Entity Framework Core](https://docs.microsoft.com/en-us/ef/) C# classes from an existing SQL Server database. (stand-alone Reverse Engineering tool and API)

![WPF Application Screenshot](https://raw.githubusercontent.com/ryanelian/Accelist.EntityGenerator/master/screenshot.PNG)

### Getting Started

- When running on Windows (WPF desktop application), have at least [.NET Framework 4.5.1](https://www.microsoft.com/en-us/download/details.aspx?id=53344).

- [Click here to download latest build.](https://github.com/ryanelian/Accelist.EntityGenerator/raw/master/Dist/Accelist.EntityGenerator.Wpf.exe)

### Features
- Designed with love for SQL Server and C#, when developing using database-first approach.

- Most SQL server basic data types [are supported](https://github.com/ryanelian/Accelist.EntityGenerator/blob/master/Accelist.EntityGenerator/SqlTypeDictionary.cs): 
  
  - String-like types: `VARCHAR`, `CHAR`, `NVARCHAR`, `NCHAR`, `XML`
  - Ordinal types: `BIGINT`, `INT`, `SMALLINT`, `TINYINT`, `BIT`
  - Decimal types: `NUMERIC`, `DECIMAL`, `SMALLMONEY`, `MONEY`
  - Floating-point types: `REAL`, `FLOAT`
  - Blob types: `VARBINARY`, `BINARY`
  - Date and time data structures: `DATETIMEOFFSET`, `SMALLDATETIME`, `DATETIME`, `DATETIME2`, `TIME`
  - Unique types: `UNIQUEIDENTIFIER`, `ROWVERSION`, `TIMESTAMP`

- Maps the type in accordance to appropriate C# data type. Provides null `?` hint for nullable columns.

- Supports composite primary keys.

- Supports non-standard (non-`dbo`) table schema.

- Generates the Entity Framework Core DbContext class, without requiring you to install extra / unwanted packages on your project / solution!

- Save your configurations for later!

- Blazing fast and easy to use!

### Not (Yet) Implemented
- [Navigation and inversion relational properties](https://docs.microsoft.com/en-us/ef/core/modeling/relationships). *(Hint: improve your multi-join queries performance using [Dapper](https://github.com/StackExchange/Dapper) instead!)*

- Spatial data types such as: `GEOMETRY`, `GEOGRAPHY`, and `HIERARCHYID`, because they are [not yet supported by Entity Framework Core](https://github.com/aspnet/EntityFramework/issues/1100)...

- [Retired](https://docs.microsoft.com/en-us/sql/t-sql/data-types/ntext-text-and-image-transact-sql) data types: `TEXT`, `NTEXT`, `IMAGE`.

- [Custom type mapping](https://github.com/aspnet/EntityFramework/issues/242) for `XML` to `XElement`, `CHAR(1)` to C# `char`.

### Credits

- Tie Tieantono for developing the WPF application, and the testing console application.


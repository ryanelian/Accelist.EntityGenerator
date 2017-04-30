using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelist.EntityGenerator.ConsoleEfTest.Entities;
using Accelist.EntityGenerator.ConsoleEfTest.Services;
using Microsoft.EntityFrameworkCore;

namespace Accelist.EntityGenerator.ConsoleEfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            dbContextOptionsBuilder.UseSqlServer(connectionString);
            var testDbContext = new TestDbContext(dbContextOptionsBuilder.Options);
            var dbService = new DbService(testDbContext);

            dbService.InsertTest();
            dbService.SelectTest();

            dbService.InsertTheNullable();
            dbService.SelectTheNullable();
        }
    }
}

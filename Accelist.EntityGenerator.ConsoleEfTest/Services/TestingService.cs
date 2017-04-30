using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelist.EntityGenerator.ConsoleEfTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accelist.EntityGenerator.ConsoleEfTest.Services
{
    public class TestingService
    {
        public TestingService(TestDbContext testDbContext) {
            this.TestDbContext = testDbContext;
        }

        private readonly TestDbContext TestDbContext;

        public void InsertTest() {
            var randomBytes = new Random();
            var bytes = new byte[8];
            randomBytes.NextBytes(bytes);

            var newTest = new Test
            {
                TheBigInt = long.MaxValue,
                TheBinary = bytes,
                TheBit = true,
                TheChar = "TheChar",
                TheDate = DateTime.Now,
                TheDateTime = DateTime.Now,
                TheDateTime2 = DateTime.Now,
                TheDateTimeOffset = DateTimeOffset.Now,
                TheDecimal = 10155.61M,
                TheFloat = 203941.2145,
                TheGuid = Guid.NewGuid(),
                TheMoney = 175000.50M,
                TheNChar = "TheNChar",
                TheNumeric = 12345.6789M,
                TheNVarChar = "TheNVarChar",
                TheReal = 12345.12F,
                TheSmallDateTime = DateTime.Now,
                TheSmallInt = short.MaxValue,
                TheSmallMoney = 21700.15M,
                TheTime = TimeSpan.FromMinutes(1000),
                TheTinyInt = byte.MaxValue,
                TheVarBinary = bytes,
                TheVarChar = "TheVarChar",
                TheXml = "<Test>Hello World!</Test>"
            };

            this.TestDbContext.Test.Add(newTest);
            this.TestDbContext.SaveChanges();
        }

        public void InsertTheNullable() {
            var newTheNullable = new TheNullable
            {
                TheBigInt = null,
                TheBinary = null,
                TheBit = null,
                TheChar = null,
                TheDate = null,
                TheDateTime = null,
                TheDateTime2 = null,
                TheDateTimeOffset = null,
                TheDecimal = null,
                TheFloat = null,
                TheGuid = null,
                TheMoney = null,
                TheNChar = null,
                TheNumeric = null,
                TheNVarChar = null,
                TheReal = null,
                TheSmallDateTime = null,
                TheSmallInt = null,
                TheSmallMoney = null,
                TheTime = null,
                TheTinyInt = null,
                TheVarBinary = null,
                TheVarChar = null,
                TheXml = null
            };

            this.TestDbContext.TheNullable.Add(newTheNullable);
            this.TestDbContext.SaveChanges();
        }

        public Test SelectTest()
        {
            var test = this.TestDbContext.Test.FirstOrDefault();
            return test;
        }

        public TheNullable SelectTheNullable()
        {
            var theNullable = this.TestDbContext.TheNullable.FirstOrDefault();
            return theNullable;
        }
    }
}

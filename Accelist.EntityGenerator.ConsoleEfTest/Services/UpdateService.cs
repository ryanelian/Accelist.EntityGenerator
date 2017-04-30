using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelist.EntityGenerator.ConsoleEfTest.Entities;

namespace Accelist.EntityGenerator.ConsoleEfTest.Services
{
    public class UpdateService
    {
        public TestDbContext TestDbContext = new TestDbContext();

        public void UpdateTest()
        {
            var randomBytes = new Random();
            var bytes = new byte[8];
            randomBytes.NextBytes(bytes);

            var test = this.TestDbContext.Test.FirstOrDefault();
            test.TheBigInt = long.MinValue;
            test.TheBinary = bytes;
            test.TheBit = true;
            test.TheChar = "TheChar";
            test.TheDate = DateTime.Now;
            test.TheDateTime = DateTime.Now;
            test.TheDateTime2 = DateTime.Now;
            test.TheDateTimeOffset = DateTimeOffset.Now;
            test.TheDecimal = 10155.61M;
            test.TheFloat = 203941.2145F;
            test.TheGuid = Guid.NewGuid();
            test.TheMoney = 175000.50M;
            test.TheNChar = "TheNChar";
            test.TheNumeric = 12345.6789M;
            test.TheNVarChar = "TheNVarChar";
            test.TheReal = 12345.12F;
            test.TheSmallDateTime = DateTime.Now;
            test.TheSmallInt = short.MaxValue;
            test.TheSmallMoney = 21700.15M;
            test.TheTime = TimeSpan.FromMinutes(1000);
            test.TheTinyInt = byte.MaxValue;
            test.TheVarBinary = bytes;
            test.TheVarChar = "TheVarChar";
            test.TheXml = "<Test>Hello World!</Test>";

            this.TestDbContext.Test.Update(test);
            this.TestDbContext.SaveChanges();
        }

        public void UpdateTheNullable()
        {
            var randomBytes = new Random();
            var bytes = new byte[8];
            randomBytes.NextBytes(bytes);

            var theNullable = this.TestDbContext.TheNullable.FirstOrDefault();
            theNullable.TheBigInt = long.MinValue;
            theNullable.TheBinary = bytes;
            theNullable.TheBit = true;
            theNullable.TheChar = "TheChar";
            theNullable.TheDate = DateTime.Now;
            theNullable.TheDateTime = DateTime.Now;
            theNullable.TheDateTime2 = DateTime.Now;
            theNullable.TheDateTimeOffset = DateTimeOffset.Now;
            theNullable.TheDecimal = 10155.61M;
            theNullable.TheFloat = 203941.2145F;
            theNullable.TheGuid = Guid.NewGuid();
            theNullable.TheMoney = 175000.50M;
            theNullable.TheNChar = "TheNChar";
            theNullable.TheNumeric = 12345.6789M;
            theNullable.TheNVarChar = "TheNVarChar";
            theNullable.TheReal = 12345.12F;
            theNullable.TheSmallDateTime = DateTime.Now;
            theNullable.TheSmallInt = short.MaxValue;
            theNullable.TheSmallMoney = 21700.15M;
            theNullable.TheTime = TimeSpan.FromMinutes(1000);
            theNullable.TheTinyInt = byte.MaxValue;
            theNullable.TheVarBinary = bytes;
            theNullable.TheVarChar = "TheVarChar";
            theNullable.TheXml = "<Test>Hello World!</Test>";

            this.TestDbContext.TheNullable.Update(theNullable);
            this.TestDbContext.SaveChanges();
        }
    }
}

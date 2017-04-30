using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelist.EntityGenerator.ConsoleEfTest.Entities;

namespace Accelist.EntityGenerator.ConsoleEfTest.Services
{
    public class SelectService
    {
        public TestDbContext TestDbContext = new TestDbContext();

        public Test SelectTest() {
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

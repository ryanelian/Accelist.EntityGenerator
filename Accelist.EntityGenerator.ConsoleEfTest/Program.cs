using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelist.EntityGenerator.ConsoleEfTest.Services;

namespace Accelist.EntityGenerator.ConsoleEfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var insertService = new InsertService();
            var updateService = new UpdateService();
            var selectService = new SelectService();

            insertService.InsertTest();
            selectService.SelectTest();

            insertService.InsertTheNullable();
            selectService.SelectTheNullable();

            updateService.UpdateTest();
            selectService.SelectTest();

            updateService.UpdateTheNullable();
            selectService.SelectTheNullable();

            Console.Read();
        }
    }
}

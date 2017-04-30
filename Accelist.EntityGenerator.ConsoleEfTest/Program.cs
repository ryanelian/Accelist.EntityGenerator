using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelist.EntityGenerator.ConsoleEfTest.Entities;
using Accelist.EntityGenerator.ConsoleEfTest.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Accelist.EntityGenerator.ConsoleEfTest
{
    class Program
    {
        public static IServiceProvider _Resolver;

        public static object _ResolverLock = new object();

        public static IServiceProvider Resolver
        {
            get
            {
                if (_Resolver == null)
                {
                    lock (_ResolverLock)
                    {
                        if (_Resolver == null)
                        {
                            var services = new ServiceCollection();
                            services.AddDbContext<TestDbContext>(options =>
                            {
                                var connectionString = ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString;
                                options.UseSqlServer(connectionString);
                            });
                            services.AddTransient<TestingService>();

                            _Resolver = services.BuildServiceProvider();
                        }
                    }
                }

                return _Resolver;
            }
        }

        public static async Task MainAsync(TestingService tester)
        {
            await tester.Insert();
            await tester.InsertNulls();
            await tester.SelectAll();
        }

        static void Main(string[] args)
        {
            var tester = Resolver.GetService<TestingService>();

            MainAsync(tester).GetAwaiter().GetResult();
        }
    }
}

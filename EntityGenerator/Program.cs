using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGenerator
{
    class Program
    {
        static async Task MainAsync()
        {
            var db = new SqlConnection(@"Data Source=.\SQL2016;Initial Catalog=TANGO;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            var gen = new GeneratorService(db);

            var scans = await gen.QueryScanModels();
            var entities = gen.ConvertScanToEntities(scans);
            gen.WriteEntities(entities, "TAM.Tango.Entities");
        }

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
    }
}

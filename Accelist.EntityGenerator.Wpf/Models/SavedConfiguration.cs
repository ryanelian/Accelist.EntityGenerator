using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator.Wpf.Models
{
    public class SavedConfiguration
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ConnectionString { set; get; }

        public string ProjectNamespace { set; get; }

        public string DbContextName { set; get; }

        public string ExportToFolder { set; get; }
    }
}

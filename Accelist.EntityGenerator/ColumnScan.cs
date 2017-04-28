using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator
{
    public class ColumnScan
    {
        public string SchemaName { set; get; }

        public string TableName { set; get; }

        public string ColumnName { set; get; }

        public bool Nullable { set; get; }

        public string DataType { set; get; }

        public bool IsPrimaryKey { set; get; }
    }
}

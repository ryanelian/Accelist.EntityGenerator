using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator
{
    public class ForeignKeyScan
    {
        public string ConstraintName { set; get; }

        public string TableName { set; get; }

        public string ForeignKey { set; get; }

        public string ReferencedTable { set; get; }

        public string ReferencedKey { set; get; }
    }
}

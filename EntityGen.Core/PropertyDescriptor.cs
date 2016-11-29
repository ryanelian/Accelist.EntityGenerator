using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGen.Core
{
    public class PropertyDescriptor
    {
        public Type DataType { set; get; }

        public string Name { set; get; }

        public bool IsPrimaryKey { set; get; }
    }
}

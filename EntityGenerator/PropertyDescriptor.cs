using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGenerator
{
    public class PropertyDescriptor
    {
        public Type DataType { set; get; }

        public string Name { set; get; }

        public bool PrimaryKey { set; get; }
    }
}

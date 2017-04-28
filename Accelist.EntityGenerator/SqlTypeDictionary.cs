using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator
{
    public class SqlType : Tuple<string, bool>
    {
        public SqlType(string typeName, bool nullable) : base(typeName, nullable)
        {
        }
    }

    public class SqlTypeDictionary : ReadOnlyDictionary<SqlType, Type>
    {
        public SqlTypeDictionary(IDictionary<SqlType, Type> dictionary) : base(dictionary)
        {
        }

        public Type Translate(string type, bool nullable)
        {
            return this[new SqlType(type, nullable)];
        }
    }
}

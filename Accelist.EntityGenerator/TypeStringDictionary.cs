using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator
{
    public class TypeStringDictionary : ReadOnlyDictionary<Type, string>
    {
        public TypeStringDictionary(IDictionary<Type, string> dictionary) : base(dictionary)
        {
        }
    }
}

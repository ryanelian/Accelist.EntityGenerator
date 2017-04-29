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
        private static TypeStringDictionary _Standard;

        private static object _StandardLock = new object();

        public static TypeStringDictionary Standard
        {
            get
            {
                if (_Standard == null)
                {
                    lock (_StandardLock)
                    {
                        if (_Standard == null)
                        {
                            var renders = new Dictionary<Type, string>();

                            renders.Add(typeof(long), "long");
                            renders.Add(typeof(long?), "long?");
                            renders.Add(typeof(int), "int");
                            renders.Add(typeof(int?), "int?");
                            renders.Add(typeof(short), "short");
                            renders.Add(typeof(short?), "short?");
                            renders.Add(typeof(byte), "byte");
                            renders.Add(typeof(byte?), "byte?");
                            renders.Add(typeof(bool), "bool");
                            renders.Add(typeof(bool?), "bool?");
                            renders.Add(typeof(decimal), "decimal");
                            renders.Add(typeof(decimal?), "decimal?");
                            renders.Add(typeof(float), "float");
                            renders.Add(typeof(float?), "float?");
                            renders.Add(typeof(double), "double");
                            renders.Add(typeof(double?), "double?");
                            renders.Add(typeof(string), "string");
                            renders.Add(typeof(byte[]), "byte[]");
                            renders.Add(typeof(Guid), "Guid");
                            renders.Add(typeof(Guid?), "Guid?");
                            renders.Add(typeof(DateTime), "DateTime");
                            renders.Add(typeof(DateTime?), "DateTime?");
                            renders.Add(typeof(TimeSpan), "TimeSpan");
                            renders.Add(typeof(TimeSpan?), "TimeSpan?");

                            _Standard = new TypeStringDictionary(renders);
                        }
                    }
                }

                return _Standard;
            }
        }

        public TypeStringDictionary(IDictionary<Type, string> dictionary) : base(dictionary)
        {
        }
    }
}

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
        private static SqlTypeDictionary _Standard;

        private static object _StandardLock = new object();

        public static SqlTypeDictionary Standard
        {
            get
            {
                if (_Standard == null)
                {
                    lock (_StandardLock)
                    {
                        if (_Standard == null)
                        {
                            var typings = new Dictionary<SqlType, Type>();

                            typings.Add(new SqlType("varchar", false), typeof(string));
                            typings.Add(new SqlType("varchar", true), typeof(string));
                            typings.Add(new SqlType("char", false), typeof(string));
                            typings.Add(new SqlType("char", true), typeof(string));
                            typings.Add(new SqlType("nvarchar", false), typeof(string));
                            typings.Add(new SqlType("nvarchar", true), typeof(string));
                            typings.Add(new SqlType("nchar", false), typeof(string));
                            typings.Add(new SqlType("nchar", true), typeof(string));

                            typings.Add(new SqlType("bigint", false), typeof(long));
                            typings.Add(new SqlType("bigint", true), typeof(long?));

                            typings.Add(new SqlType("int", false), typeof(int));
                            typings.Add(new SqlType("int", true), typeof(int?));

                            typings.Add(new SqlType("smallint", false), typeof(short));
                            typings.Add(new SqlType("smallint", true), typeof(short?));

                            typings.Add(new SqlType("tinyint", false), typeof(byte));
                            typings.Add(new SqlType("tinyint", true), typeof(byte?));

                            typings.Add(new SqlType("numeric", false), typeof(decimal));
                            typings.Add(new SqlType("numeric", true), typeof(decimal?));
                            typings.Add(new SqlType("decimal", false), typeof(decimal));
                            typings.Add(new SqlType("decimal", true), typeof(decimal?));
                            typings.Add(new SqlType("smallmoney", false), typeof(decimal));
                            typings.Add(new SqlType("smallmoney", true), typeof(decimal?));
                            typings.Add(new SqlType("money", false), typeof(decimal));
                            typings.Add(new SqlType("money", true), typeof(decimal?));

                            typings.Add(new SqlType("real", false), typeof(float)); // float(24)
                            typings.Add(new SqlType("real", true), typeof(float?)); // float(24)

                            typings.Add(new SqlType("float", false), typeof(double));
                            typings.Add(new SqlType("float", true), typeof(double?));

                            typings.Add(new SqlType("bit", false), typeof(bool));
                            typings.Add(new SqlType("bit", true), typeof(bool?));

                            typings.Add(new SqlType("uniqueidentifier", false), typeof(Guid));
                            typings.Add(new SqlType("uniqueidentifier", true), typeof(Guid?));

                            typings.Add(new SqlType("binary", false), typeof(byte[]));
                            typings.Add(new SqlType("binary", true), typeof(byte[]));
                            typings.Add(new SqlType("varbinary", false), typeof(byte[]));
                            typings.Add(new SqlType("varbinary", true), typeof(byte[]));

                            typings.Add(new SqlType("smalldatetime", false), typeof(DateTime));
                            typings.Add(new SqlType("smalldatetime", true), typeof(DateTime?));
                            typings.Add(new SqlType("datetime", false), typeof(DateTime));
                            typings.Add(new SqlType("datetime", true), typeof(DateTime?));
                            typings.Add(new SqlType("datetime2", false), typeof(DateTime));
                            typings.Add(new SqlType("datetime2", true), typeof(DateTime?));

                            typings.Add(new SqlType("time", false), typeof(TimeSpan));
                            typings.Add(new SqlType("time", true), typeof(TimeSpan?));

                            // Unmapped: DATETIMEOFFSET, ROWVERSION, XML

                            _Standard = new SqlTypeDictionary(typings);
                        }
                    }
                }

                return _Standard;
            }
        }

        public SqlTypeDictionary(IDictionary<SqlType, Type> dictionary) : base(dictionary)
        {
        }

        public Type Translate(string type, bool nullable)
        {
            return this[new SqlType(type, nullable)];
        }
    }
}

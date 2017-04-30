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
                            typings.Add(new SqlType("xml", false), typeof(string));
                            typings.Add(new SqlType("xml", true), typeof(string));

                            typings.Add(new SqlType("bigint", false), typeof(long));
                            typings.Add(new SqlType("bigint", true), typeof(long?));
                            typings.Add(new SqlType("int", false), typeof(int));
                            typings.Add(new SqlType("int", true), typeof(int?));
                            typings.Add(new SqlType("smallint", false), typeof(short));
                            typings.Add(new SqlType("smallint", true), typeof(short?));
                            typings.Add(new SqlType("tinyint", false), typeof(byte));
                            typings.Add(new SqlType("tinyint", true), typeof(byte?));
                            typings.Add(new SqlType("bit", false), typeof(bool));
                            typings.Add(new SqlType("bit", true), typeof(bool?));

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

                            typings.Add(new SqlType("uniqueidentifier", false), typeof(Guid));
                            typings.Add(new SqlType("uniqueidentifier", true), typeof(Guid?));

                            // For some bizarre reason, ROWVERSION columns return TIMESTAMP as type when queried. Include both, to be safe!
                            typings.Add(new SqlType("timestamp", false), typeof(byte[]));
                            typings.Add(new SqlType("timestamp", true), typeof(byte[]));
                            typings.Add(new SqlType("rowversion", false), typeof(byte[]));
                            typings.Add(new SqlType("rowversion", true), typeof(byte[]));

                            typings.Add(new SqlType("binary", false), typeof(byte[]));
                            typings.Add(new SqlType("binary", true), typeof(byte[]));
                            typings.Add(new SqlType("varbinary", false), typeof(byte[]));
                            typings.Add(new SqlType("varbinary", true), typeof(byte[]));

                            typings.Add(new SqlType("datetimeoffset", false), typeof(DateTimeOffset));
                            typings.Add(new SqlType("datetimeoffset", true), typeof(DateTimeOffset?));

                            typings.Add(new SqlType("date", false), typeof(DateTime));
                            typings.Add(new SqlType("date", true), typeof(DateTime?));
                            typings.Add(new SqlType("smalldatetime", false), typeof(DateTime));
                            typings.Add(new SqlType("smalldatetime", true), typeof(DateTime?));
                            typings.Add(new SqlType("datetime", false), typeof(DateTime));
                            typings.Add(new SqlType("datetime", true), typeof(DateTime?));
                            typings.Add(new SqlType("datetime2", false), typeof(DateTime));
                            typings.Add(new SqlType("datetime2", true), typeof(DateTime?));

                            typings.Add(new SqlType("time", false), typeof(TimeSpan));
                            typings.Add(new SqlType("time", true), typeof(TimeSpan?));

                            // https://msdn.microsoft.com/en-us/library/dn236441(v=sql.120).aspx
                            // Warning about client side usage of GEOMETRY, GEOGRAPHY and HIERARCHYID
                            // The assembly Microsoft.SqlServer.Types.dll, which contains the spatial data types...

                            // EF Core does not support spatial data types yet. So those 3 data types are not yet mapped.
                            // https://github.com/aspnet/EntityFramework/issues/1100

                            //https://docs.microsoft.com/en-us/sql/t-sql/data-types/ntext-text-and-image-transact-sql
                            // IMPORTANT! ntext, text, and image data types will be removed in a future version of SQL Server. 
                            // Avoid using these data types in new development work, and plan to modify applications that currently use them.
                            // Use nvarchar(max), varchar(max), and varbinary(max) instead. 

                            // Hence, those 3 data types are also not supported.

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

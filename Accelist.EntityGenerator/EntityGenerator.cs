using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace Accelist.EntityGenerator
{
    /// <summary>
    /// Provide methods for translating SQL tables to C# POCO classes, and outputting them.
    /// </summary>
    public class EntityGenerator
    {
        private static SqlTypeDictionary _Typings;
        private static object _TypingsLock = new object();

        private static TypeStringDictionary _Renders;
        private static object _RendersLock = new object();

        public static SqlTypeDictionary Typings
        {
            get
            {
                if (_Typings == null)
                {
                    lock (_TypingsLock)
                    {
                        if (_Typings == null)
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

                            typings.Add(new SqlType("real", false), typeof(double)); // float(24)
                            typings.Add(new SqlType("real", true), typeof(double?)); // float(24)
                            typings.Add(new SqlType("float", false), typeof(double));
                            typings.Add(new SqlType("float", true), typeof(double?));

                            typings.Add(new SqlType("bit", false), typeof(bool));
                            typings.Add(new SqlType("bit", true), typeof(bool?));

                            typings.Add(new SqlType("uniqueidentifier", false), typeof(Guid));
                            typings.Add(new SqlType("uniqueidentifier", true), typeof(Guid?));

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

                            _Typings = new SqlTypeDictionary(typings);
                        }
                    }
                }

                return _Typings;
            }
        }

        public static TypeStringDictionary Renders
        {
            get
            {
                if (_Renders == null)
                {
                    lock (_RendersLock)
                    {
                        if (_Renders == null)
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

                            _Renders = new TypeStringDictionary(renders);
                        }
                    }
                }

                return _Renders;
            }
        }

        public static string EntitiesFolderName => "Entities";

        private readonly SqlConnection DB;

        /// <summary>
        /// Constructs a new GeneratorService using a provided SQL Server database.
        /// </summary>
        /// <param name="db"></param>
        public EntityGenerator(SqlConnection db)
        {
            this.DB = db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ColumnScan>> Scan()
        {
            var query = DB.QueryAsync<ColumnScan>(@"
WITH PK AS
(
	SELECT 
		ind.object_id as ObjectId,
		col.name as ColumnName
	FROM sys.indexes ind
	JOIN sys.index_columns ic ON ind.object_id = ic.object_id and ind.index_id = ic.index_id 
	JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
	WHERE ind.is_primary_key = 1
)
SELECT DISTINCT
	sch.name as SchemaName,
	t.name as TableName,
	col.name as ColumnName,
	col.is_nullable as Nullable,
	dt.name as DataType,
	CAST(CASE WHEN EXISTS(SELECT TOP 1 1 FROM PK WHERE t.object_id = PK.ObjectId AND col.name = PK.ColumnName) THEN 1 ELSE 0 END AS BIT) as IsPrimaryKey
FROM sys.tables t
JOIN sys.schemas sch ON t.schema_id = sch.schema_id
JOIN sys.columns col ON t.object_id = col.object_id
JOIN sys.types dt ON col.system_type_id = dt.system_type_id
WHERE
	t.is_ms_shipped = 0
	AND NOT (t.name = 'sysdiagrams')
ORDER BY t.name, col.name
");
            return (await query).ToList();
        }
    }
}

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
        private readonly SqlConnection DB;

        private static SqlTypeDictionary _Typings;
        private static TypeStringDictionary _TypeStrings;
        private static string _EntitiesFolderName;

        public static SqlTypeDictionary TypeMapper
        {
            get => _Typings ?? SqlTypeDictionary.Standard;
            set => _Typings = value;
        }

        public static TypeStringDictionary TypeStrings
        {
            get => _TypeStrings ?? TypeStringDictionary.Standard;
            set => _TypeStrings = value;
        }

        public static string EntitiesFolderName
        {
            get => _EntitiesFolderName ?? "Entities";
            set => _EntitiesFolderName = value;
        }

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
	AND NOT (dt.name = 'sysname')
ORDER BY t.name, col.name
");
            return (await query).ToList();
        }
    }
}

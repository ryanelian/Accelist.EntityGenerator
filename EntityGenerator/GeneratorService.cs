using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGenerator
{
    using System.IO;
    using EntityDictionary = Dictionary<string, List<PropertyDescriptor>>;
    using TypeDictionary = Dictionary<Tuple<string, bool>, Type>;

    public class GeneratorService
    {
        private readonly SqlConnection DB;
        private readonly TypeDictionary Typings;
        private readonly Dictionary<Type, string> TypeNamings;

        public GeneratorService(SqlConnection db)
        {
            this.DB = db;
            this.Typings = CreateNewSqlTypeDictionary();
            this.TypeNamings = CreateNewCSharpTypeDictionary();
        }

        public Dictionary<Type, string> CreateNewCSharpTypeDictionary()
        {
            var namings = new Dictionary<Type, string>();
            namings.Add(typeof(int), "int");
            namings.Add(typeof(int?), "int?");
            namings.Add(typeof(bool), "bool");
            namings.Add(typeof(bool?), "bool?");
            namings.Add(typeof(decimal), "decimal");
            namings.Add(typeof(decimal?), "decimal?");
            namings.Add(typeof(string), "string");
            namings.Add(typeof(byte[]), "byte[]");
            namings.Add(typeof(Guid), "Guid");
            namings.Add(typeof(DateTime), "DateTime");
            namings.Add(typeof(DateTime?), "DateTime?");

            return namings;
        }

        public TypeDictionary CreateNewSqlTypeDictionary()
        {
            // First Key = SQL Server Type Name
            // Second Key = Is Nullable

            var typings = new TypeDictionary();

            typings.Add(Tuple.Create("varchar", false), typeof(string));
            typings.Add(Tuple.Create("varchar", true), typeof(string));
            typings.Add(Tuple.Create("char", false), typeof(string));
            typings.Add(Tuple.Create("char", true), typeof(string));

            typings.Add(Tuple.Create("int", false), typeof(int));
            typings.Add(Tuple.Create("int", true), typeof(int?));

            typings.Add(Tuple.Create("decimal", false), typeof(decimal));
            typings.Add(Tuple.Create("decimal", true), typeof(decimal?));

            typings.Add(Tuple.Create("bit", false), typeof(bool));
            typings.Add(Tuple.Create("bit", true), typeof(bool?));

            typings.Add(Tuple.Create("uniqueidentifier", false), typeof(Guid));
            typings.Add(Tuple.Create("uniqueidentifier", true), typeof(Guid?));

            typings.Add(Tuple.Create("varbinary", false), typeof(byte[]));
            typings.Add(Tuple.Create("varbinary", true), typeof(byte[]));

            typings.Add(Tuple.Create("datetime", false), typeof(DateTime));
            typings.Add(Tuple.Create("datetime2", false), typeof(DateTime));
            typings.Add(Tuple.Create("datetime", true), typeof(DateTime?));
            typings.Add(Tuple.Create("datetime2", true), typeof(DateTime?));

            return typings;
        }

        public void WriteEntities(EntityDictionary entities, string space)
        {
            if (Directory.Exists("Entities") == false)
            {
                Directory.CreateDirectory("Entities");
            }

            foreach (var existing in Directory.EnumerateFiles("Entities"))
            {
                File.Delete(existing);
            }

            foreach (var entity in entities)
            {
                File.WriteAllText(Path.Combine("Entities", entity.Key + ".cs"), ToCSharpClass(entity, space));
            }
        }

        private string ToCSharpClass(KeyValuePair<string, List<PropertyDescriptor>> entity, string space)
        {
            return $@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {space}
{{
    public class {entity.Key}
    {{
{ToCSharpProperties(entity.Value)}    }}
}}
";
        }

        private string ToCSharpProperties(List<PropertyDescriptor> properties)
        {
            var sb = new StringBuilder();

            var singleKey = properties.Count(Q => Q.PrimaryKey) == 1;

            foreach (var property in properties)
            {
                if (property.PrimaryKey && singleKey)
                {
                    sb.AppendLine("        [Key]");
                }

                sb.AppendLine($"        public {TypeNamings[property.DataType]} {property.Name} {{ get; set; }}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public async Task<List<ScanModel>> QueryScanModels()
        {
            var query = DB.QueryAsync<ScanModel>(@"
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
	t.object_id as ObjectId,
	t.name as TableName,
	col.name as ColumnName,
	col.is_nullable as Nullable,
	dt.name as DataType,
	CAST(IIF(EXISTS(SELECT TOP 1 1 FROM PK WHERE t.object_id = PK.ObjectId AND col.name = PK.ColumnName), 1, 0) AS BIT) as PK
FROM sys.tables t
JOIN sys.columns col ON t.object_id = col.object_id
JOIN sys.types dt ON col.system_type_id = dt.system_type_id
WHERE
	t.is_ms_shipped = 0
	AND NOT (t.name = 'sysdiagrams')
ORDER BY t.object_id, col.name
");
            return (await query).ToList();
        }

        public EntityDictionary ConvertScanToEntities(List<ScanModel> scans)
        {
            return scans.GroupBy(Q => Q.ObjectId)
                .ToDictionary(
                    Q => Q.First().TableName,
                    Q => Q.Select(C => CastToPropertyDescriptors(C)).ToList()
                );
        }

        public PropertyDescriptor CastToPropertyDescriptors(ScanModel scan)
        {
            var prop = new PropertyDescriptor();
            prop.Name = scan.ColumnName;
            prop.PrimaryKey = scan.PK;
            prop.DataType = Typings[Tuple.Create(scan.DataType, scan.Nullable)];

            return prop;
        }
    }
}

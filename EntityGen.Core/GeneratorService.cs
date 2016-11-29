using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGen.Core
{
    using ClassDescriptor = KeyValuePair<string, List<PropertyDescriptor>>;
    using EntityDictionary = Dictionary<string, List<PropertyDescriptor>>;
    using TypeDictionary = Dictionary<Tuple<string, bool>, Type>;
    using VirtualFile = Tuple<string, string>;

    /// <summary>
    /// 
    /// </summary>
    public class GeneratorService
    {
        private readonly SqlConnection DB;
        private readonly TypeDictionary Typings;
        private readonly Dictionary<Type, string> TypeStrings;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public GeneratorService(SqlConnection db)
        {
            this.DB = db;
            this.Typings = CreateNewSqlTypeDictionary();
            this.TypeStrings = CreateNewCSharpTypeDictionary();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<Type, string> CreateNewCSharpTypeDictionary()
        {
            var namings = new Dictionary<Type, string>();
            namings.Add(typeof(int), "int");
            namings.Add(typeof(int?), "int?");
            namings.Add(typeof(bool), "bool");
            namings.Add(typeof(bool?), "bool?");
            namings.Add(typeof(decimal), "decimal");
            namings.Add(typeof(decimal?), "decimal?");
            namings.Add(typeof(double), "double");
            namings.Add(typeof(double?), "double?");
            namings.Add(typeof(string), "string");
            namings.Add(typeof(byte[]), "byte[]");
            namings.Add(typeof(Guid), "Guid");
            namings.Add(typeof(DateTime), "DateTime");
            namings.Add(typeof(DateTime?), "DateTime?");

            return namings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

            typings.Add(Tuple.Create("float", false), typeof(double));
            typings.Add(Tuple.Create("float", true), typeof(double?));

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="folder"></param>
        public void WriteVirtualFilesToOutputFolder(VirtualFile[] files, string folder, bool clean)
        {
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }

            if (clean)
            {
                var existing = Directory.EnumerateFiles(folder);
                if (existing.Any())
                {
                    foreach (var delete in existing)
                    {
                        File.Delete(delete);
                    }
                }
            }

            foreach (var file in files)
            {
                var target = Path.Combine(folder, file.Item1);
                File.WriteAllText(target, file.Item2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        private string ToCSharpClass(ClassDescriptor entity, string space)
        {
            var singleKey = entity.Value.Count(Q => Q.IsPrimaryKey) == 1;

            var lines = entity.Value.Select(property =>
                {
                    var s = $"        public {TypeStrings[property.DataType]} {property.Name} {{ get; set; }}";
                    if (singleKey && property.IsPrimaryKey)
                    {
                        s = "        [Key]\r\n" + s;
                    }
                    return s;
                })
                .ToList();

            var properties = string.Join("\r\n\r\n", lines);

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
{properties}
    }}
}}
";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
CAST(CASE WHEN EXISTS(SELECT TOP 1 1 FROM PK WHERE t.object_id = PK.ObjectId AND col.name = PK.ColumnName) THEN 1 ELSE 0 END AS BIT) as IsPrimaryKey
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scans"></param>
        /// <returns></returns>
        public EntityDictionary ConvertScanToEntities(List<ScanModel> scans)
        {
            return scans.GroupBy(Q => Q.ObjectId)
                .ToDictionary(
                    Q => Q.First().TableName,
                    Q => Q.Select(C => CastToPropertyDescriptors(C)).ToList()
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scan"></param>
        /// <returns></returns>
        public PropertyDescriptor CastToPropertyDescriptors(ScanModel scan)
        {
            var prop = new PropertyDescriptor();
            prop.Name = scan.ColumnName;
            prop.IsPrimaryKey = scan.IsPrimaryKey;
            prop.DataType = Typings[Tuple.Create(scan.DataType, scan.Nullable)];

            return prop;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="space"></param>
        /// <param name="dbContextName"></param>
        /// <returns></returns>
        public Task<VirtualFile[]> GenerateEntityVirtualFiles(EntityDictionary entities, string space, string dbContextName)
        {
            var parallel = new List<Task<VirtualFile>>();

            foreach (var entity in entities)
            {
                var gen = Task.Run(() =>
                {
                    var filename = entity.Key + ".cs";
                    var content = ToCSharpClass(entity, space);
                    return Tuple.Create(filename, content);
                });
                parallel.Add(gen);
            }

            var ctx = Task.Run(() =>
            {
                return Tuple.Create(dbContextName + ".cs", GenerateDbContext(space, dbContextName, entities));
            });
            parallel.Add(ctx);

            return Task.WhenAll(parallel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="space"></param>
        /// <param name="dbContextName"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public string GenerateDbContext(string space, string dbContextName, EntityDictionary entities)
        {
            return $@"using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace {space}
{{
    public class {dbContextName} : DbContext
    {{
        public {dbContextName}(DbContextOptions<{dbContextName}> options) : base(options) {{ }}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {{
{GenerateCompositeKey(entities)}
        }}

{GenerateDbSetProperty(entities)}
    }}
}}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public string GenerateCompositeKey(EntityDictionary entities)
        {
            var lines = entities
                .Where(entity => entity.Value.Count(property => property.IsPrimaryKey) > 1)
                .OrderBy(Q => Q.Key)
                .Select(entity => $@"            modelBuilder.Entity<{entity.Key}>().HasKey(entity => new
            {{
{GenerateCompositeKeyProperties(entity)}
            }});")
                .ToList();

            return string.Join("\r\n\r\n", lines);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GenerateCompositeKeyProperties(ClassDescriptor entity)
        {
            var keys = entity.Value
                .Where(Q => Q.IsPrimaryKey)
                .Select(key => $@"                entity.{ key.Name },")
                .ToList();

            return string.Join("\r\n", keys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public string GenerateDbSetProperty(EntityDictionary entities)
        {
            var lines = entities
                .OrderBy(Q => Q.Key)
                .Select(entity => $"        public virtual DbSet<{entity.Key}> {entity.Key} {{ get; set; }}")
                .ToList();

            return string.Join("\r\n\r\n", lines);
        }
    }
}

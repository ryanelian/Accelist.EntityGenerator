using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator
{
    public static class EntityGeneratorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scans"></param>
        /// <returns></returns>
        public static List<Entity> ToEntities(this List<ColumnScan> scans)
        {
            return scans.AsParallel()
                .GroupBy(Q => Q.TableName)
                .Select(table => Entity.CreateFromColumns(table))
                .OrderBy(Q => Q.Name)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static string GenerateCompositeKeys(this List<Entity> entities)
        {
            var lines = entities.Where(entity => entity.Properties.Count(property => property.IsPrimaryKey) > 1)
                .Select(entity => entity.WriteDbContextPrimaryKeys())
                .ToList();

            return string.Join("\r\n\r\n", lines);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static string GenerateDbSets(this List<Entity> entities)
        {
            var lines = entities
                .Select(entity => entity.WriteDbSet())
                .OrderBy(Q => Q)
                .ToList();

            return string.Join("\r\n\r\n", lines);
        }

        public static string ToDbContext(this List<Entity> entities, string projectNamespace, string dbContextName)
        {
            return $@"using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace {projectNamespace}.{EntityGenerator.EntitiesFolderName}
{{
    public class {dbContextName} : DbContext
    {{
        public {dbContextName}(DbContextOptions<{dbContextName}> options) : base(options) {{ }}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {{
{entities.GenerateCompositeKeys()}
        }}

{entities.GenerateDbSets()}
    }}
}}";
        }

        public static List<VirtualFile> ToVirtualFiles(this List<Entity> entities, string projectNamespace, string dbContextName)
        {
            var bag = new ConcurrentBag<VirtualFile>();

            Parallel.ForEach(entities, entity =>
            {
                var file = entity.Name + ".cs";
                var content = entity.Serialize(projectNamespace);

                bag.Add(new VirtualFile(file, content));
            });

            {
                var file = dbContextName + ".cs";
                var content = entities.ToDbContext(projectNamespace, dbContextName);
                bag.Add(new VirtualFile(file, content));
            }

            return bag.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="path"></param>
        /// <param name="clean"></param>
        public static void WriteToFolder(this List<VirtualFile> files, string path, bool clean = true)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            if (clean)
            {
                var existing = Directory.EnumerateFiles(path);
                if (existing.Any())
                {
                    foreach (var delete in existing)
                    {
                        File.Delete(delete);
                    }
                }
            }

            // Don't parallel this or HDD-based system will freeze.
            foreach (var file in files)
            {
                var target = Path.Combine(path, file.Item1);
                File.WriteAllText(target, file.Item2);
            }
        }
    }
}

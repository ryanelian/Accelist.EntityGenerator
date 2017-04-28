using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator
{
    public class Entity
    {
        public string Schema { set; get; }

        public string Name { set; get; }

        public List<EntityProperty> Properties { set; get; }

        public bool Validate()
        {
            return Properties.Any(Q => Q.IsPrimaryKey);
        }

        /// <summary>
        /// Convert this object into a Primary Key fluent mapper for Entity Framework Core.
        /// </summary>
        /// <returns></returns>
        public string WriteDbContextPrimaryKeys()
        {
            var lines = Properties.Where(Q => Q.IsPrimaryKey)
                .Select(key => $@"                entity.{ key.Name },")
                .ToList();

            var keys = string.Join("\r\n", lines);

            return $@"            modelBuilder.Entity<{Name}>().HasKey(entity => new
            {{
{keys}
            }});";
        }

        /// <summary>
        /// Convert this object into a DbSet entry for Entity Framework Core.
        /// </summary>
        /// <returns></returns>
        public string WriteDbSet()
        {
            return $"        public virtual DbSet<{Name}> {Name} {{ get; set; }}";
        }

        /// <summary>
        /// Convert this object into a C# Class string output.
        /// </summary>
        /// <param name="projectNamespace"></param>
        /// <returns></returns>
        public string Serialize(string projectNamespace)
        {
            return $@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {projectNamespace}.{EntityGenerator.EntitiesFolderName}
{{
    {OptionalTableMapper()}public class {this.Name}
    {{
{SerializeProperties()}
    }}
}}
";
        }

        public string OptionalTableMapper()
        {
            if (Schema != "dbo")
            {
                return $"[Table(nameof({Name}), Schema = \"{Schema}\")]\r\n    ";
            }

            return string.Empty;
        }

        public string SerializeProperties()
        {
            var singleKey = this.Properties.Count(Q => Q.IsPrimaryKey) == 1;

            var lines = this.Properties.Select(property =>
            {
                var s = $"        public {EntityGenerator.Renders[property.DataType]} {property.Name} {{ get; set; }}";
                if (singleKey && property.IsPrimaryKey)
                {
                    s = "        [Key]\r\n" + s;
                }
                return s;
            }).ToList();

            return string.Join("\r\n\r\n", lines);
        }
    }

    public class EntityProperty
    {
        public Type DataType { set; get; }

        public string Name { set; get; }

        public bool IsPrimaryKey { set; get; }
    }

}

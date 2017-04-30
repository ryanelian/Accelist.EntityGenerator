using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelist.EntityGenerator.ConsoleEfTest.Entities
{
    [Table(nameof(TheNullable), Schema = "CustomSchema")]
    public class TheNullable
    {
        public long? TheBigInt { get; set; }

        public byte[] TheBinary { get; set; }

        public bool? TheBit { get; set; }

        public string TheChar { get; set; }

        public DateTime? TheDate { get; set; }

        public DateTime? TheDateTime { get; set; }

        public DateTime? TheDateTime2 { get; set; }

        public DateTimeOffset? TheDateTimeOffset { get; set; }

        public decimal? TheDecimal { get; set; }

        public double? TheFloat { get; set; }

        public Guid? TheGuid { get; set; }

        public decimal? TheMoney { get; set; }

        public string TheNChar { get; set; }

        [Key]
        public int TheNullableId { get; set; }

        public decimal? TheNumeric { get; set; }

        public string TheNVarChar { get; set; }

        public float? TheReal { get; set; }

        [Timestamp]
        public byte[] TheRowVersion { get; set; }

        public DateTime? TheSmallDateTime { get; set; }

        public short? TheSmallInt { get; set; }

        public decimal? TheSmallMoney { get; set; }

        public TimeSpan? TheTime { get; set; }

        public byte? TheTinyInt { get; set; }

        public byte[] TheVarBinary { get; set; }

        public string TheVarChar { get; set; }

        public string TheXml { get; set; }
    }
}

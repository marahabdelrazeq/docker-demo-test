using Microsoft.EntityFrameworkCore;
using Redis.OM.Modeling;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonRepo.Infrastructure.Caching.Models
{
    [Document(StorageType = StorageType.Hash, Prefixes = new[] { "Tbls" }, IndexName = "TablesWithViews")]
    [Keyless]
    public class TablesWithViews
    {
        [NotMapped]
        [RedisIdField] public Ulid Id { get; set; }
        [Indexed] public string TABLE_NAME { get; set; }
        [Indexed] public string VIEW_NAME { get; set; }
        [Searchable] public string Type_NAME { get; set; }
        [Column("VIEW_CATALOG")]
        [Indexed] public string DB_NAME { get; set; }

        //The foreign_key_column may have been removed or you might need to manually add it.
        [NotMapped]
        [Indexed] public string KEY { get; set; }

        [NotMapped]
        [Indexed] public string COLUMNS { get; set; }



    }

    [Keyless]
    public class KeyViews
    {
        public string foreign_key_column { get; set; }

    }
    [Keyless]
    public class COLUMN_NAME
    {
        [Column("name")]
        public string ColumnName { get; set; }

    }
}
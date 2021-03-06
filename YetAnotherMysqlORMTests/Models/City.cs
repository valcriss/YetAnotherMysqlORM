using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YetAnotherMysqlORM.Attributes;
using YetAnotherMysqlORM.Models;

namespace YetAnotherMysqlORMTests.Models
{
    [Table("villes")]
    public class City : Table<City>
    {
        [Field("id", true)]
        public int Id { get; set; }
        [Field("name")]
        public string? Name { get; set; }
        [Field("link")]
        public string? Link { get; set; }

        public async Task<List<Street>> GetStreets()
        {
            return await GetLinkedRecords<Street>();
        }
    }
}

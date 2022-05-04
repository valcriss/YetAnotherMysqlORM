using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YetAnotherMysqlORM.Attributes;
using YetAnotherMysqlORM.Models;

namespace YetAnotherMysqlORMTests.Models
{
    [Table("rues")]
    public class Street : Table<Street>
    {
        [Field("id", true)]
        public int Id { get; set; }

        [Field("villes_id")]
        [Foreign(typeof(City))]
        public int CityId { get; set; }

        [Field("street")]
        public string? StreetName { get; set; }

        [Field("creationDate")]
        public DateTime Creation { get; set; }

        [Field("updateDate", false, true)]
        public DateTime? Update { get; set; }

        public async Task<City> GetCity()
        {
            return await GetLinkedRecord<City>();
        }
    }
}

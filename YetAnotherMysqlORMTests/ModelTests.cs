using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using YetAnotherMysqlORM;
using YetAnotherMysqlORMTests.Models;

namespace YetAnotherMysqlORMTests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public async Task AddModelTest()
        {
            Database.Initialize("localhost", "sexemodel", "root", "password");

            City city = new City() { Name = "Tours", Link = "https://google.fr" };
            bool result = await city.Save();
            Assert.IsTrue(result);
        }
    }
}
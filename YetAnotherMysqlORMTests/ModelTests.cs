using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetAnotherMysqlORM;
using YetAnotherMysqlORMTests.Models;

namespace YetAnotherMysqlORMTests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public async Task AddRemoveModelTest()
        {
            Database.Initialize("localhost", "testdb", "root", "password");

            City city = new City() { Name = Faker.LocationFaker.City(), Link = Faker.InternetFaker.Url() };
            bool addResult = await city.Save();
            Assert.IsTrue(addResult);
            int id = city.Id;
            city = null;
            city = await City.Load(id);
            Assert.IsNotNull(city);
            Assert.AreEqual(id, city.Id);

            bool deleteResult = await city.Delete();

            Assert.IsTrue(deleteResult);
        }

        [TestMethod]
        public async Task ListModelTest()
        {
            Database.Initialize("localhost", "testdb", "root", "password");

            for (int i = 0; i < 10; i++)
            {
                City city = new City() { Name = Faker.LocationFaker.City(), Link = Faker.InternetFaker.Url() };
                bool addResult = await city.Save();
                Assert.IsTrue(addResult);
            }

            List<City> cities = await City.Select();

            Assert.IsNotNull(cities);
            Assert.AreEqual(10, cities.Count);

            foreach(City city in cities)
            {
                bool deleteResult =  await city.Delete();
                Assert.IsTrue(deleteResult);
            }

        }
    }
}
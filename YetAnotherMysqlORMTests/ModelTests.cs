using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetAnotherMysqlORM;
using YetAnotherMysqlORM.Models;
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

            foreach (City city in cities)
            {
                bool deleteResult = await city.Delete();
                Assert.IsTrue(deleteResult);
            }

        }

        [TestMethod]
        public void DateParseTest()
        {
            string date = "2022-12-25";
            DateTime? dateTime = Table<City>.ParseDate(date);
            Assert.IsNotNull(dateTime);
        }

        [TestMethod]
        public async Task LinkedModelsTest()
        {
            Database.Initialize("localhost", "testdb", "root", "password");

            City city = new City() { Name = Faker.LocationFaker.City(), Link = Faker.InternetFaker.Url() };
            bool addResult = await city.Save();
            Assert.IsTrue(addResult);

            for (int i = 0; i < 10; i++)
            {
                Street street = new Street() { CityId = city.Id, StreetName = Faker.LocationFaker.StreetName(), Creation = DateTime.Now };
                bool addStreetResult = await street.Save();
                Assert.IsTrue(addStreetResult);
            }

            List<Street> streetList = await city.GetStreets();
            Assert.IsNotNull(streetList);
            Assert.AreEqual(10, streetList.Count);

            Street firstStreet = streetList[0];
            City foreignCity = await firstStreet.GetCity();

            Assert.IsNotNull(foreignCity);
            Assert.AreEqual(city.Id, foreignCity.Id);

            foreach (Street street in streetList)
            {
                bool deleteStreetResult = await street.Delete();
                Assert.IsTrue(deleteStreetResult);
            }

            bool deleteCityResult = await city.Delete();
            Assert.IsTrue(deleteCityResult);
        }

        [TestMethod]
        public async Task UpdateModelTest()
        {
            Database.Initialize("localhost", "testdb", "root", "password");

            City city = new City() { Name = Faker.LocationFaker.City(), Link = Faker.InternetFaker.Url() };
            bool addResult = await city.Save();
            Assert.IsTrue(addResult);

            Street street = new Street() { CityId = city.Id, StreetName = Faker.LocationFaker.StreetName(), Creation = DateTime.Now };
            bool addStreetResult = await street.Save();
            Assert.IsTrue(addStreetResult);

            city.Name = Faker.LocationFaker.City();
            bool updateCityResult = await city.Save();
            Assert.IsTrue(updateCityResult);

            street.Update = DateTime.Now;
            bool updateStreetResult = await street.Save();
            Assert.IsTrue(updateStreetResult);

            bool deleteStreetResult = await street.Delete();
            Assert.IsTrue(deleteStreetResult);

            bool deleteCityResult = await city.Delete();
            Assert.IsTrue(deleteCityResult);
        }

        [TestMethod]
        public async Task VariableInitializationTest()
        {
            Database.Initialize("localhost", "testdb", "root", "password");

            City city = new City() { Name = Faker.LocationFaker.City(), Link = Faker.InternetFaker.Url() };
            bool addResult = await city.Save();
            Assert.IsTrue(addResult);

            Street street = new Street() { CityId = city.Id, StreetName = Faker.LocationFaker.StreetName(), Creation = DateTime.Now };
            bool addStreetResult = await street.Save();
            Assert.IsNull(street.Update);
            Assert.IsTrue(addStreetResult);

            street = await Street.Load(street.Id);

            Assert.IsNull(street.Update);

            List<Street> streets = await city.GetStreets();

            foreach(Street item in streets)
            {
                Assert.IsNull(item.Update);
            }

            bool deleteStreetResult = await street.Delete();
            Assert.IsTrue(deleteStreetResult);

            bool deleteCityResult = await city.Delete();
            Assert.IsTrue(deleteCityResult);
        }
    }
}
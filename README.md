# YetAnotherMysqlORM [![.NET](https://github.com/valcriss/YetAnotherMysqlORM/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/valcriss/YetAnotherMysqlORM/actions/workflows/dotnet.yml)

A very minimalist ORM for mysql, not for production purpose, i use it for prototyping applications.

---
This library allows to manipulate database throw objects. Some datatypes have been implemented, others will come according to needs or requests.
The library targets multiple frameworks : 
- .Net Core 3.1
- .Net Core 5
- .Net Core 6
- .NET Standard 2.0
- .NET 4.8

---
### Installation
Simply download and restore nuget packages https://www.nuget.org/packages/YetAnotherMysqlORM/ or install it from package manager
```
PM> Install-Package YetAnotherMysqlORM -Version x.x.x
```

---
### Initialize the Database
```C#
Database.Initialize("localhost", "database", "user", "password");
```

---
### Declare a Table Object
```C#
[Table("city")]
public class City : Table<City>
{
    [Field("id", true)]
    public int Id { get; set; }

    [Field("name")]
    public string Name { get; set; }

    [Field("link")]
    public string Link { get; set; }
    
    public async Task<List<Street>> GetStreets()
    {
        return await GetLinkedRecords<Street>();
    }
}

[Table("street")]
public class Street : Table<Street>
{
    [Field("id", true)]
    public int Id { get; set; }

    [Field("city_id")]
    [Foreign(typeof(City))]
    public int CityId { get; set; }

    [Field("name")]
    public string Name { get; set; }
    
    public async Task<City> GetCity()
    {
        return await GetLinkedRecord<City>();
    }
}
```
---
### List records
```C#
List<City> cities = await City.Select();
```
### List records filtered
```C#
List<City> cities = await City.Select("name LIKE '%Paris%'");
```
### Load a record by primary value
```C#
City city = await City.Load(id);
```
### Create a record
```C#
City city = new City() { Name = Faker.LocationFaker.City(), Link = Faker.InternetFaker.Url() };
bool addResult = await city.Save();
```
### Update a record
```C#
City city = await City.Load(id);
city.Name = "Paris";
bool updateResult = await city.Save();
```
### Delete a record
```C#
bool deleteResult = await city.Delete();
```

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YetAnotherMysqlORM.Attributes;

namespace YetAnotherMysqlORM.Models
{
    public class Table<T> where T : Table<T>, new()
    {
        public T Loaded { get; set; }
        private string TableName { get; set; }

        public Table()
        {
            TableName = typeof(T).GetCustomAttribute<TableAttribute>() != null ? typeof(T).GetCustomAttribute<TableAttribute>().Name : null;
        }

        public async Task<bool> Save()
        {
            if (Loaded == null)
            {
                return await Insert();
            }

            return false;
        }

        public async Task<bool> Delete()
        {
            PropertyInfo property = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Primary).FirstOrDefault();
            if (property != null)
            {
                int id = (int)property.GetValue(this);
                FieldAttribute fieldAttribute = property.GetCustomAttribute<FieldAttribute>();
                string query = QueryBuilder.DeleteQuery(TableName, fieldAttribute.Name, id);
                return await Database.Delete(query);
            }
            return false;
        }

        private async Task<bool> Insert()
        {
            Dictionary<string, string> fieldValues = GetFieldValues(this, false);
            string query = QueryBuilder.InsertQuery(TableName, fieldValues);
            int? id = await Database.Insert(query);
            if (id != null)
            {
                return UpdatePrimaryKey(id.Value);
            }
            return false;
        }

        public async static Task<T> Load(int id)
        {
            PropertyInfo property = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Primary).FirstOrDefault();
            string tableName = typeof(T).GetCustomAttribute<TableAttribute>() != null ? typeof(T).GetCustomAttribute<TableAttribute>().Name : null;
            if (property != null && tableName != null)
            {
                Dictionary<string, string> fieldValues = GetFieldValues(null, true);
                FieldAttribute fieldAttribute = property.GetCustomAttribute<FieldAttribute>();
                string query = QueryBuilder.LoadQuery(tableName, fieldValues, fieldAttribute.Name, id);
                Dictionary<string, string> record = await Database.QueryOne(query);
                return LoadFromDictionnary(record);
            }
            return null;
        }

        public async static Task<List<T>> Select()
        {
            List<T> tmp = new List<T>();
            Dictionary<string, string> fieldValues = GetFieldValues(null, true);
            string tableName = typeof(T).GetCustomAttribute<TableAttribute>() != null ? typeof(T).GetCustomAttribute<TableAttribute>().Name : null;
            string query = QueryBuilder.LoadSelect(tableName, fieldValues);
            List<Dictionary<string, string>> records = await Database.QueryMultiple(query);
            foreach(Dictionary<string, string> record in records)
            {
                tmp.Add(LoadFromDictionnary(record));
            }
            return tmp;
        }

        private static T LoadFromDictionnary(Dictionary<string, string> record)
        {
            T obj = new T();

            foreach (KeyValuePair<string, string> column in record)
            {
                PropertyInfo property = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Name == column.Key).FirstOrDefault();
                if (property != null)
                {
                    if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(obj, int.Parse(column.Value));
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(obj, column.Value);
                    }
                }
            }
            obj.Loaded = obj;
            return obj;
        }

        private bool UpdatePrimaryKey(int value)
        {
            PropertyInfo property = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Primary).FirstOrDefault();
            if (property != null)
            {
                property.SetValue(this, value);
                return true;
            }
            return false;
        }

        private static Dictionary<string, string> GetFieldValues(object obj, bool includePrimary)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            foreach (var property in typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null))
            {
                FieldAttribute fieldAttribute = property.GetCustomAttribute<FieldAttribute>();
                if (!includePrimary && fieldAttribute.Primary)
                {
                    continue;
                }
                System.Diagnostics.Debug.WriteLine(property.Name);
                string value = obj != null && property.GetValue(obj) != null ? property.GetValue(obj).ToString() : null;
                values.Add(fieldAttribute.Name, value);
            }

            return values;
        }


    }
}

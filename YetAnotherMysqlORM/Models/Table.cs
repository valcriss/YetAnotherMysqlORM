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
        internal bool Loaded { get; set; }
        private string TableName { get; set; }

        public Table()
        {
            TableName = typeof(T).GetCustomAttribute<TableAttribute>() != null ? typeof(T).GetCustomAttribute<TableAttribute>().Name : null;
        }

        public async Task<bool> Save()
        {
            if (!Loaded)
            {
                return await Insert();
            }
            else
            {
                return await Update();
            }
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
            Dictionary<string, string> fieldValues = GetFieldValues<T>(this, false);
            string query = QueryBuilder.InsertQuery(TableName, fieldValues);
            int? id = await Database.Insert(query);
            if (id != null)
            {
                return UpdatePrimaryKey(id.Value);
            }
            return false;
        }

        private async Task<bool> Update()
        {
            PropertyInfo primary = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Primary).FirstOrDefault();
            if (primary != null)
            {
                FieldAttribute primaryFieldAttribute = primary.GetCustomAttribute<FieldAttribute>();
                int primaryValue = (int)primary.GetValue(this);
                Dictionary<string, string> fieldValues = GetFieldValues<T>(this, true);
                fieldValues.Remove(primaryFieldAttribute.Name);
                string query = QueryBuilder.UpdateQuery(TableName, fieldValues, primaryFieldAttribute.Name, primaryValue);
                return await Database.Update(query);
            }
            return false;
        }

        public async static Task<T> Load(int id)
        {
            PropertyInfo property = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Primary).FirstOrDefault();
            string tableName = typeof(T).GetCustomAttribute<TableAttribute>() != null ? typeof(T).GetCustomAttribute<TableAttribute>().Name : null;
            if (property != null && tableName != null)
            {
                Dictionary<string, string> fieldValues = GetFieldValues<T>(null, true);
                FieldAttribute fieldAttribute = property.GetCustomAttribute<FieldAttribute>();
                string query = QueryBuilder.LoadQuery(tableName, fieldValues, fieldAttribute.Name, id);
                Dictionary<string, string> record = await Database.QueryOne(query);
                return LoadFromDictionnary<T>(record);
            }
            return null;
        }

        public async static Task<List<T>> Select(string filter = null)
        {
            List<T> tmp = new List<T>();
            Dictionary<string, string> fieldValues = GetFieldValues<T>(null, true);
            string tableName = typeof(T).GetCustomAttribute<TableAttribute>() != null ? typeof(T).GetCustomAttribute<TableAttribute>().Name : null;
            string query = QueryBuilder.LoadSelect(tableName, fieldValues, filter);
            List<Dictionary<string, string>> records = await Database.QueryMultiple(query);
            foreach (Dictionary<string, string> record in records)
            {
                tmp.Add(LoadFromDictionnary<T>(record));
            }
            return tmp;
        }

        protected async Task<List<U>> GetLinkedRecords<U>(string filter = null) where U : Table<U>, new()
        {
            string remoteTable = typeof(U).GetCustomAttribute<TableAttribute>() != null ? typeof(U).GetCustomAttribute<TableAttribute>().Name : null;
            PropertyInfo foreignProperty = typeof(U).GetProperties().Where(c => c.GetCustomAttribute<ForeignAttribute>() != null && c.GetCustomAttribute<ForeignAttribute>().ForeignType == typeof(T)).FirstOrDefault();
            PropertyInfo primary = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Primary).FirstOrDefault();
            if (foreignProperty != null && primary != null)
            {
                int value = (int)primary.GetValue(this);

                Dictionary<string, string> fieldValues = GetFieldValues<U>(null, true);
                string remoteField = foreignProperty.GetCustomAttribute<FieldAttribute>().Name;
                string query = QueryBuilder.LinkedRecords(remoteTable, fieldValues, remoteField, value, filter);
                List<Dictionary<string, string>> records = await Database.QueryMultiple(query);
                List<U> tmp = new List<U>();
                foreach (Dictionary<string, string> record in records)
                {
                    tmp.Add(LoadFromDictionnary<U>(record));
                }
                return tmp;

            }
            return null;
        }

        protected async Task<U> GetLinkedRecord<U>() where U : Table<U>, new()
        {
            string remoteTable = typeof(U).GetCustomAttribute<TableAttribute>() != null ? typeof(U).GetCustomAttribute<TableAttribute>().Name : null;
            PropertyInfo property = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<ForeignAttribute>() != null && c.GetCustomAttribute<ForeignAttribute>().ForeignType == typeof(U)).FirstOrDefault();
            if (property != null)
            {
                int value = (int)property.GetValue(this);
                PropertyInfo foreignPrimary = typeof(U).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Primary).FirstOrDefault();
                if (foreignPrimary != null)
                {
                    Dictionary<string, string> fieldValues = GetFieldValues<U>(null, true);
                    string remotePrimary = foreignPrimary.GetCustomAttribute<FieldAttribute>().Name;
                    string query = QueryBuilder.LinkedRecord(remoteTable, fieldValues, remotePrimary, value);
                    Dictionary<string, string> record = await Database.QueryOne(query);
                    return LoadFromDictionnary<U>(record);
                }
            }
            return null;
        }

        private static Z LoadFromDictionnary<Z>(Dictionary<string, string> record) where Z : Table<Z>, new()
        {
            Z obj = new Z();

            foreach (KeyValuePair<string, string> column in record)
            {
                PropertyInfo property = typeof(Z).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Name == column.Key).FirstOrDefault();
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
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        property.SetValue(obj, ParseDate(column.Value));
                    }
                }
            }
            obj.Loaded = true;
            return obj;
        }

        public static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (value.Length == 10)
            {
                if (value.IndexOf('-') >= 0)
                {
                    string[] tab = value.Split('-');
                    return new DateTime(int.Parse(tab[0]), int.Parse(tab[1]), int.Parse(tab[2]));
                }
                else
                {
                    string[] tab = value.Split('/');
                    return new DateTime(int.Parse(tab[2]), int.Parse(tab[1]), int.Parse(tab[0]));
                }
                
            }
            else
            {
                string[] tab1 = value.Split(' ');
                if (tab1[0].IndexOf('-') >= 0)
                {
                    string[] tab = tab1[0].Split('-');
                    return new DateTime(int.Parse(tab[0]), int.Parse(tab[1]), int.Parse(tab[2]));
                }
                else
                {
                    string[] tab = tab1[0].Split('/');
                    return new DateTime(int.Parse(tab[2]), int.Parse(tab[1]), int.Parse(tab[0]));
                }                
            }
        }

        private bool UpdatePrimaryKey(int value)
        {
            PropertyInfo property = typeof(T).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null && c.GetCustomAttribute<FieldAttribute>().Primary).FirstOrDefault();
            if (property != null)
            {
                property.SetValue(this, value);
                this.Loaded = true;
                return true;
            }
            return false;
        }

        private static Dictionary<string, string> GetFieldValues<Z>(object obj, bool includeAllFields)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            foreach (var property in typeof(Z).GetProperties().Where(c => c.GetCustomAttribute<FieldAttribute>() != null))
            {
                FieldAttribute fieldAttribute = property.GetCustomAttribute<FieldAttribute>();
                if (!includeAllFields && (fieldAttribute.Primary || fieldAttribute.IgnoreOnCreate))
                {
                    continue;
                }

                string value = null;
                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    value = obj != null && property.GetValue(obj) != null ? ((DateTime)property.GetValue(obj)).ToString("yyyy-MM-dd") : null;
                }
                else
                {
                    value = obj != null && property.GetValue(obj) != null ? property.GetValue(obj).ToString() : null;
                }
                values.Add(fieldAttribute.Name, value);
            }

            return values;
        }


    }
}

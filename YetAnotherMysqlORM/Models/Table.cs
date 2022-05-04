using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YetAnotherMysqlORM.Attributes;

namespace YetAnotherMysqlORM.Models
{
    public class Table<T> where T : class, new()
    {
        private T Loaded { get; set; }
        private string TableName { get; set; }

        public Table()
        {
            Loaded = null;
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

        private async Task<bool> Insert()
        {
            Dictionary<string, string> fieldValues = GetFieldValues(false);
            string query = QueryBuilder.InsertQuery(TableName, fieldValues);
            bool result = await Database.Execute(query);
            if(result)
            {

            }
            return result;
        }

        private Dictionary<string, string> GetFieldValues(bool includePrimary)
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
                string value = property.GetValue(this) != null ? property.GetValue(this).ToString() : null;
                values.Add(fieldAttribute.Name, value);
            }

            return values;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherMysqlORM.Models
{
    internal class QueryBuilder
    {
        public static string InsertQuery(string table, Dictionary<string, string> fieldValues)
        {
            string fields = string.Join(", ", fieldValues.Keys);
            string values = string.Join(", ", UpdateFields(fieldValues).Values);
            return $"INSERT INTO {table} ({fields}) VALUES ({values})";
        }

        private static Dictionary<string, string> UpdateFields(Dictionary<string, string> fieldValues)
        {
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in fieldValues)
            {
                if (kvp.Value != null)
                {
                    tmp.Add(kvp.Key, "\"" + kvp.Value + "\"");
                }
                else
                {
                    tmp.Add(kvp.Key, "null");
                }
            }
            return tmp;
        }
    }
}

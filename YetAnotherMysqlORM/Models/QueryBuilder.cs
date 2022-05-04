using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static string UpdateQuery(string table, Dictionary<string, string> fieldValues, string primaryField, int primaryValue)
        {
            Dictionary<string, string> updatedFieldValues = UpdateFields(fieldValues);
            string updates = string.Empty;
            for (int i = 0; i < updatedFieldValues.Keys.Count; i++)
            {
                string key = updatedFieldValues.Keys.ToArray()[i];
                string value = updatedFieldValues[key];
                updates += $"{key}={value}" + (i < updatedFieldValues.Keys.Count - 1 ? ", " : null);
            }
            return $"UPDATE {table} SET {updates} WHERE {primaryField}={primaryValue}";
        }

        public static string DeleteQuery(string table, string primaryField, int primaryValue)
        {
            return $"DELETE FROM {table} WHERE {primaryField}={primaryValue}";
        }

        public static string LoadQuery(string table, Dictionary<string, string> fieldValues, string primaryField, int primaryValue)
        {
            string fields = string.Join(", ", fieldValues.Keys);
            return $"SELECT {fields} FROM {table} WHERE {primaryField}={primaryValue}";
        }

        public static string LoadSelect(string table, Dictionary<string, string> fieldValues, string filter = null)
        {
            string fields = string.Join(", ", fieldValues.Keys);
            return $"SELECT {fields} FROM {table}" + (filter != null ? $" WHERE {filter}" : null);
        }

        public static string LinkedRecord(string remoteTable, Dictionary<string, string> fieldValues, string remotePrimary, int value)
        {
            string fields = string.Join(", ", fieldValues.Keys);
            return $"SELECT {fields} FROM {remoteTable} WHERE {remotePrimary}={value}";
        }

        public static string LinkedRecords(string remoteTable, Dictionary<string, string> fieldValues, string remotePrimary, int value, string filter = null)
        {
            string fields = string.Join(", ", fieldValues.Keys);
            return $"SELECT {fields} FROM {remoteTable} WHERE {remotePrimary}={value}" + (filter != null ? $" AND ({filter})" : null);
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

using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherMysqlORM.Attributes
{
    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Primary { get; set; }
        public bool IgnoreOnCreate { get; set; }
        public FieldAttribute(string name, bool primary = false,bool ignoreOnCreate = false)
        {
            Name = name;
            Primary = primary;
            IgnoreOnCreate = ignoreOnCreate;
        }
    }
}

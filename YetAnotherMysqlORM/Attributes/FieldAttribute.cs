using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherMysqlORM.Attributes
{
    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Primary { get; set; }
        public FieldAttribute(string name, bool primary = false)
        {
            Name = name;
            Primary = primary;
        }
    }
}

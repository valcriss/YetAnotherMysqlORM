using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherMysqlORM.Attributes
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}

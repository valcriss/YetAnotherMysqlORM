using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherMysqlORM.Attributes
{
    public class ForeignAttribute : Attribute
    {
        public Type ForeignType { get; set; }
        public ForeignAttribute(Type foreignType)
        {
            ForeignType = foreignType;
        }
    }
}

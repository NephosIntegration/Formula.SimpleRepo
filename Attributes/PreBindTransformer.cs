using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Formula.SimpleRepo
{
    public delegate object TransformerDelegate(object value);

    [AttributeUsage(AttributeTargets.Property)]
    public class PreBindTransformer : System.Attribute
    {
        public PreBindTransformer(Type delegateType, string delegateName, TypeCode dataType)
        {
            DelegateType = delegateType;
            DelegateName = delegateName;
            TransformerDelegate = (TransformerDelegate)Delegate.CreateDelegate(typeof(TransformerDelegate), delegateType, delegateName);
            DataType = dataType;
        }

        public Type DelegateType { get; set; }
        public string DelegateName { get; set; }
        public TransformerDelegate TransformerDelegate { get; set; }
        public TypeCode DataType { get; set; }
    }
}

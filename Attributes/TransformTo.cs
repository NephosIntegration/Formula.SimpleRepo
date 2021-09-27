using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Formula.SimpleRepo
{
    public delegate object TransformToDelegate(object value);

    [AttributeUsage(AttributeTargets.Property)]
    public class TransformTo : System.Attribute
    {
        public TransformTo(Type delegateType, string delegateName, TypeCode dataType)
        {
            DelegateType = delegateType;
            DelegateName = delegateName;
            TransformToDelegate = (TransformToDelegate)Delegate.CreateDelegate(typeof(TransformToDelegate), delegateType, delegateName);
            DataType = dataType;
        }

        public Type DelegateType { get; set; }
        public string DelegateName { get; set; }
        public TransformToDelegate TransformToDelegate { get; set; }
        public TypeCode DataType { get; set; }
    }
}

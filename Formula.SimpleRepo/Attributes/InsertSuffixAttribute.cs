using System;

namespace Formula.SimpleRepo;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class InsertSuffixAttribute  : Attribute
{
    public string Suffix { get; }

    public InsertSuffixAttribute(string suffix)
    {
        Suffix = suffix;
    }    
}

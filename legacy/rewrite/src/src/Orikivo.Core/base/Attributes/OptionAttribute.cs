using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AccountOptionAttribute : Attribute
    {
        public AccountOption Option { get; }

        public AccountOptionAttribute(AccountOption option)
        {
            Option = option;
        }
    }
}

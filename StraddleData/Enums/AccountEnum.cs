using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleData.Enums
{
    public enum AccountType
    {
        Savings = 1,
        Current,
        Checking
    }

    public enum AccountCurrency
    {
        USD = 1,
        EUR
    }

    public enum AccountStatus
    {
        Active = 1,
        InActive,
        Blocked
    }

    public enum CustomerStatus
    {
        Pending = 1,
        Active
    }

    public enum CustomerType
    {
        Individual = 1,
        Company
    }

    public enum IdentityType
    {
        Passport = 1,
        WorkPermit,
        ResidencePermit,
        NationalIdentity
    }

    public enum TierLevel
    {
        Primary = 1,
        Secondary,
        Tertiary
    }
}

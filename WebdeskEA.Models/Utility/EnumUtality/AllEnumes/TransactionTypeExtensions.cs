using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;

namespace WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes
{
    public static class TransactionTypeExtensions
    {
        private static readonly Dictionary<TransactionType, string> TransactionTypeMap = new Dictionary<TransactionType, string>
        {
            { TransactionType.Credit, "CRE" },
            { TransactionType.Debit, "DEB" }
        };

        public static string GetCode(this TransactionType transactionType)
        {
            return TransactionTypeMap[transactionType];
        }
    }
}

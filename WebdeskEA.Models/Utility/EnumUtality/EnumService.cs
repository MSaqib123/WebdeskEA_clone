using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;


namespace WebdeskEA.Models.Utility.EnumUtality
{
    public class EnumService : IEnumService
    {
        public IEnumerable<KeyValuePair<string, string>> GetTransactionTypes()
        {
            var types = Enum.GetValues(typeof(TransactionType))
                    .Cast<TransactionType>()
                    .Select(e => new KeyValuePair<string, string>(e.GetCode(), e.ToString()));
            return types;
        }

        public IEnumerable<KeyValuePair<string, string>> GetPaymentTypes()
        {
            var types = Enum.GetValues(typeof(PaymentType))
                    .Cast<PaymentType>()
                    .Select(e => new KeyValuePair<string, string>(e.GetCode(), e.ToString()));
            return types;
        }
    }
}

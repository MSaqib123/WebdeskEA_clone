using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager;

namespace WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes
{
    public static class PaymentTypeExtensions
    {
        private static readonly Dictionary<PaymentType, string> PaymentTypeMap = new Dictionary<PaymentType, string>
        {
            { PaymentType.Online, "Online" },
            { PaymentType.Cheque, "Cheque" }
        };

        public static string GetCode(this PaymentType PaymentType)
        {
            return PaymentTypeMap[PaymentType];
        }
    }
}

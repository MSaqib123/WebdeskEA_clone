using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.Utilitytility.EnumUtality.AllEnumes.DataManager
{
    public enum TransactionType
    {
        Credit,
        Debit
    }

    public enum TypeView
    {
        Create,
        Edit
    }

    public enum PaymentType
    {
        Cheque,
        Online
    }


    public enum InvoiceType
    {
        PurchaseInvoice,
        SaleInvoice
    }



    public enum Coatypes
    {
        [Description("Asset")]
        Asset,

        [Description("Equity")]
        Equity,

        [Description("Expenses")]
        Expenses,

        [Description("Liabilities & Credit Cards")]
        LiabilitiesAndCreditCards,

        [Description("Income")]
        Income
    }
    public static class CoatypesExtensions
    {
        private static readonly Dictionary<Coatypes, string> CoatypesNames = new()
        {
            { Coatypes.Asset, "Asset" },
            { Coatypes.Equity, "Equity" },
            { Coatypes.Expenses, "Expenses" },
            { Coatypes.LiabilitiesAndCreditCards, "Liabilities & Credit Cards" },
            { Coatypes.Income, "Income" }
        };

        public static string GetCoatypesName(this Coatypes coatype)
        {
            return CoatypesNames[coatype];
        }
    }


    public enum DeleteType
    {
        SoftDelete = 1,
        PermanentDelete = 2,
        SoftDeleteIfNoNestedRelation = 3,
        PermanentDeleteIfNoNestedRelation = 4
    }

}

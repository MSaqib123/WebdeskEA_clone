using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.Utility
{
    //static detail files
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        //_______ Order Status __________
        public static string StatusPending = "Pending";
        public static string StatusApproved = "Approved";
        public static string StatusInProcess = "Proccessing";
        public static string StatusShipped = "Shipped";
        public static string StatusCancelled = "Cancelled";
        public static string StatusRefunded = "Refunded";

        public static string PaymentStatusPending = "Pending";
        public static string PaymentStatusApproved = "Approved";
        public static string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public static string PaymentStatusRejected = "Rejected";

        //_________ Session __________
        public static string SessionCart = "SessionShoppingCart";
    }
}

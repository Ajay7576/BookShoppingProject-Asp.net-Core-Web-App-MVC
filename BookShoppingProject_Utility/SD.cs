using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject_Utility
{
     public static class SD
    {
        //stored procedure
        public const string Pro_CoverType_Create = "Sp_CreateCoverType";
        public const string Pro_CoverType_Update = "Sp_UpdateCoverType";
        public const string Pro_CoverType_Delete = "Sp_DeleteCoverType";
        public const string Pro_CoverType_GetCoverTypes = "Sp_GetCoverTypes";
        public const string Pro_CoverType_GetCoverType = "Sp_GetCoverType";

        // Roles

        public const string Role_Admin = "Admin";
        public const string Role_Employee= "Employee User";
        public const string Role_Company = "Company User";
        public const string Role_Individual= "Individual User";

        // Session 
        public const string Ss_Session = "Cart Count Session";

        // method
        public static double GetPriceBasedQuantity(double quantity,double Price,double Price50,double Price100)
        {
            if (quantity < 50)
                return Price;
            else if (quantity < 100)
                return Price50;
            else
                return Price100;
        }
        // C sharp  html raw 
        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
                int arrayIndex = 0;
            bool inside = false;
             for (int i=0; i<source.Length;i++)
            {
                char let = source[i];
                if(let=='<')
                {
                    inside = true;
                    continue;
                }
                if(let=='>')
                {
                    inside = false;
                    continue;
                }
                if(!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);

        }
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusProcessing = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";

        // payment status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelay = "PaymentStatusDelay";
        public const string PaymentStatusRejected = "Rejected";
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject_Models
{
   public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;

        }
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [Range(1,1000,ErrorMessage ="please enter value between 1 to 1000")]
        public int Count { get; set; }
        [NotMapped]
        public double Price { get; set; }

    }
}

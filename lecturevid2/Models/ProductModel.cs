using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace lecturevid2.Models
{
    public class ProductModel
    {
       
        public int prod_id { get; set; }

        [Required(ErrorMessage ="Product name is required")] 
        public string prod_name { get; set; }
        [Required(ErrorMessage = "Product price is required")]
        public int prod_price { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace lecturevid2.Models
{
    public class User
    {
               
              public int user_id { get; set; }
              [Required(ErrorMessage ="Fullname is required")]
              public string user_fullname { get; set; }
               [Required(ErrorMessage ="Email is required")]
      [DataType(DataType.EmailAddress)]
              public string user_email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        // [MinLength()
        public string user_pwd { get; set; }
    }
}

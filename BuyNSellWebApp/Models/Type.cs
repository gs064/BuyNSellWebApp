using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNSellWebApp.Models
{
    public class Type
    {
        [Key]
        public int TypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; }

        public ICollection<SubType> SubTypes { get; set; }

    }
}

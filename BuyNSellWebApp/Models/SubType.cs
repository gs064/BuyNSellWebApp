using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNSellWebApp.Models
{
    public class SubType
    {
        [Key]
        public int SubTypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string SubTypeName { get; set; }

        [Required]
        public int TypeID { get; set; }

        [ForeignKey("TypeID")]
        [InverseProperty("SubTypes")]
        public Type Types { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}

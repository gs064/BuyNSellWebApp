using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNSellWebApp.Models
{
    public class Request
    {
        [Key]
        public int RequestID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        public float Price { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(20)]
        public string ContactNo { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        public DateTime RequestDate { get; set; }

        [Required]
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        [InverseProperty("Requests")]
        public Product Product { get; set; }
    }
}

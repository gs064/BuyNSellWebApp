using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNSellWebApp.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(50)]
        public string ExtName { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public int SubTypeID { get; set; }

        [ForeignKey("SubTypeID")]
        [InverseProperty("Products")]
        public SubType SubType { get; set; }

        [NotMapped]
        public ImageUpload FileUpload { get; set; }

        public ICollection<Request> Requests { get; set; }
    }
    public class ImageUpload
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}

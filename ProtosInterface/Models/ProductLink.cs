using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface.Models
{
    class ProductLink
    {
        [Column("parent_product_id")]
        public int ParentProductId { get; set; }   // Внешний ключ + часть PK
        
        [ForeignKey("ParentProductId")]
        public Product ParentProduct { get; set; }  // Навигационное свойство
        
        [Column("included_product_id")]
        public int IncludedProductId { get; set; }  // Внешний ключ + часть PK
        
        [ForeignKey("IncludedProductId")]
        public Product IncludedProduct { get; set; } // Навигационное свойство

        [Column("amount")]
        public double Amount { get; set; }
    }
}

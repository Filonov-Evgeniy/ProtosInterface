using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface.Models
{
    internal class Operation
    {
        [Key]
        public int Id { get; set; }

        public int Code { get; set; }

        public int TypeId { get; set; }

        public int ProductId { get; set; }

        public int CoopStatusId { get; set; }

        public string? Description { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("TypeId")]
        public OperationType OperationType { get; set; }
    }
}

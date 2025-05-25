using Microsoft.EntityFrameworkCore;
using ProtosInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface
{
    internal class ListFill
    {
        private dbDataLoader loader;
        private AppDbContext _context;
        public List<MenuItem> ItemOperations(int itemId)
        {
            List<MenuItem> result = new List<MenuItem>();
            IQueryable operations = _context.Operations.Include(o => o.OperationType).Where(o => o.ProductId == itemId);
            foreach (Operation operation in operations)
            {
                if (operation.OperationType != null)
                {
                    if (operation.Code < 10)
                        result.Add(new MenuItem { Title = "0" + operation.Code + " | " + operation.OperationType.Name, Id = operation.Id });
                    else
                        result.Add(new MenuItem { Title = operation.Code + " | " + operation.OperationType.Name, Id = operation.Id });
                }
            }
            return result;
        }

        public List<MenuItem> OperationEquipment(int opId)
        {
            List<MenuItem> result = new List<MenuItem>();

            var equipments = _context.Equipment
                .Join(
                    _context.OperationVariantComponents,
                    e => e.Id,
                    ovc => ovc.EquipmentId,
                    (e, ovc) => new { Equipment = e, OVC = ovc }
                )
                .Join(
                    _context.OperationVariants,
                    combined => combined.OVC.OperationVariantId,
                    ov => ov.Id,
                    (combined, ov) => new { combined.Equipment, OV = ov }
                )
                .Where(x => x.OV.OperationId == opId)
                .Select(x => new{
                    x.Equipment.Id,
                    x.Equipment.Name
                })
                .Distinct()
                .ToList();

            foreach (var equipment in equipments)
            {
                if (equipment.Id != null)
                {
                    result.Add(new MenuItem { Title = equipment.Name, Id = equipment.Id });
                }
            }
            return result;
        }

        public ListFill()
        {
            //this.productId = productId;
            _context = new AppDbContext();
            loader = new dbDataLoader();
        }
    }
}

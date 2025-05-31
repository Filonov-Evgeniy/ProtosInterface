using Microsoft.EntityFrameworkCore;
using ProtosInterface.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProtosInterface
{
    class dbDataLoader
    {
        private AppDbContext _context;

        public dbDataLoader()
        {
            _context = new AppDbContext();
        }

        public List<MenuItem> getOperationData()
        {
            IQueryable operations = _context.Operations.Include(o => o.OperationType);

            List<MenuItem> items = new List<MenuItem>();
            foreach (Operation operation in operations)
            {
                int id = operation.Id;
                int code = operation.Code;
                string name = operation.OperationType.Name;
                name = code + " | " + name;
                items.Add(new MenuItem(id, name));
            }

            return items;
        }

        public List<MenuItem> getProductData()
        {
            IQueryable products = _context.Products;

            List<MenuItem> items = new List<MenuItem>();
            foreach (Product product in products)
            {
                int id = product.Id;
                string name = product.Name;
                items.Add(new MenuItem(id, name));
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (isHasChildren(items[i].itemId))
                {
                    MenuItem item = items[i];
                    buildMenuItems(items[i].itemId, ref item);
                    items[i] = item;
                }
            }

            return items;
        }

        public void buildMenuItems(int productId, ref MenuItem item)
        {
            IQueryable products = _context.ProductLinks.Where(p => p.ParentProductId == productId);
            var productIdList = sqlToDictionary(products);
            foreach (KeyValuePair<int, double> id in productIdList)
            {
                //for (int i = 0; i < id.Value; i++)
                //{
                    MenuItem doughterItem = new MenuItem() { Title = getProductName(id.Key), Amount = id.Value };
                    doughterItem.Parent = item;
                    doughterItem.itemId = id.Key;
                    item.Items.Add(doughterItem);
                    if (isHasChildren(id.Key))
                    {
                        buildMenuItems(id.Key, ref doughterItem);
                    }
                //}
            }
        }

        public bool isHasChildren(int productId)
        {
            var childs = _context.ProductLinks.Where(p => p.ParentProductId == productId);
            if (childs.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public string getProductName(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return "undefined";
            }
            string name = product.Name;
            return name;
        }

        public Dictionary<int, double> sqlToDictionary(IQueryable products)
        {
            Dictionary<int, double> dbRows = new Dictionary<int, double>();
            foreach (ProductLink product in products)
            {
                dbRows.Add(product.IncludedProductId, product.Amount);
            }
            return dbRows;
        }
    }
}

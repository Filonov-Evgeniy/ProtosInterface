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

        public List<ProductType> getProductTypeData()
        {
            List<ProductType> productTypes = _context.ProductTypes.ToList();
            return productTypes;
        }

        public List<MenuItem> getOperationData()
        {
            IQueryable operations = _context.OperationTypes;

            List<MenuItem> items = new List<MenuItem>();
            foreach (Models.OperationType operation in operations)
            {
                int id = operation.Id;
                string name = operation.Name;
                items.Add(new MenuItem(id, name));
            }

            return items;
        }
        
        public async Task<List<MenuItem>> GetProductDataAsync()
        {
            return await Task.Run(async () =>
            {
                List<MenuItem> items = new List<MenuItem>();

                // Главный контекст для загрузки продуктов
                using (var mainContext = new AppDbContext())
                {
                    var products = mainContext.Products.AsNoTracking().ToList();
                    items.AddRange(products.Select(p => new MenuItem(p.Id, p.Name)));
                }

                return items;
            });
        }

        public async Task<List<MenuItem>> getProductData(List<MenuItem> dataList = null)
        {
            return await Task.Run(async () =>
            {
                IQueryable products = _context.Products;

                List<MenuItem> items = new List<MenuItem>();
                foreach (Product product in products)
                {
                    int id = product.Id;
                    string name = product.Name;
                    items.Add(new MenuItem(id, name));
                    dataList.Add(new MenuItem(id, name));
                }

                for (int i = 0; i < items.Count; i++)
                {
                    if (isHasChildren(items[i].itemId))
                    {
                        MenuItem item = items[i];
                        buildMenuItems(items[i].itemId, ref item);
                        items[i] = item;
                        dataList[i] = item;
                    }
                }

                return items;
            });
        }

        public List<MenuItem> getProductByTypeId(int typeId)
        {
            IQueryable products = _context.Products.Where(x=>x.TypeId == typeId);
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

        public List<MenuItem> ConvertToMenuItemFromProducts(IQueryable query)
        {
            List<MenuItem> items = new List<MenuItem>();
            foreach (Product product in query)
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

        public async Task<List<MenuItem>> GetOperationDataAsync()
        {
            return await Task.Run(async () =>
            {
                List<MenuItem> items = new List<MenuItem>();

                // Главный контекст для загрузки продуктов
                using (var mainContext = new AppDbContext())
                {
                    var operations = mainContext.OperationTypes.AsNoTracking().ToList();
                    items.AddRange(operations.Select(p => new MenuItem(p.Id, p.Name)));
                    items.Remove(items[0]);
                }

                return items;
            });
        }

        private void BuildMenuItems(AppDbContext context, int productId, ref MenuItem item)
        {
            IQueryable products = context.ProductLinks.Where(p => p.ParentProductId == productId);
            var productIdList = sqlToDictionary(products);
            foreach (KeyValuePair<int, double> id in productIdList)
            {
                MenuItem doughterItem = new MenuItem() { Title = getProductName(context, id.Key), Amount = id.Value };
                doughterItem.Parent = item;
                doughterItem.itemId = id.Key;
                item.Items.Add(doughterItem);
                if (isHasChildren(context, id.Key))
                {
                    BuildMenuItems(context, id.Key, ref doughterItem);
                }
            }
        }

        public void buildMenuItems(int productId, ref MenuItem item)
        {
            IQueryable products = _context.ProductLinks.Where(p => p.ParentProductId == productId);
            var productIdList = sqlToDictionary(products);
            foreach (KeyValuePair<int, double> id in productIdList)
            {
                MenuItem doughterItem = new MenuItem() { Title = getProductName(id.Key), Amount = id.Value };
                doughterItem.Parent = item;
                doughterItem.itemId = id.Key;
                item.Items.Add(doughterItem);
                if (isHasChildren(id.Key))
                {
                    buildMenuItems(id.Key, ref doughterItem);
                }
            }
        }

        public bool isHasChildren(AppDbContext context, int productId)
        {
            var childs = context.ProductLinks.Where(p => p.ParentProductId == productId);
            if (childs.Count() > 0)
            {
                return true;
            }
            return false;
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

        public string getProductName(AppDbContext context, int productId)
        {
            var product = context.Products.Find(productId);
            if (product == null)
            {
                return "undefined";
            }
            string name = product.Name;
            return name;
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
            foreach (Models.ProductLink product in products)
            {
                dbRows.Add(product.IncludedProductId, product.Amount);
            }
            return dbRows;
        }
    }
}

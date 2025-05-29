using Microsoft.EntityFrameworkCore;
using ProtosInterface.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace ProtosInterface
{
    class TreeMenu
    {
        private static MenuItem elementToCopy;
        private AppDbContext _context;
        private dbDataLoader loader;
        int productId;

        public MenuItem createMenu()
        {
            MenuItem root = new MenuItem() 
            { 
                Title = loader.getProductName(productId),
                itemId = productId
            };

            if (loader.isHasChildren(productId))
            {
                loader.buildMenuItems(productId, ref root);
            }
            
            return root;
        }

        public static void copyMenuItem(MenuItem item)
        {
            if (item != null)
                elementToCopy = (MenuItem)item.Clone();
        }

        public static MenuItem InsertMenuItem()
        {
            if (elementToCopy == null)
            {
                MessageBox.Show("Не выбран элемент для вставки");
                return null;
            }

            MenuItem itemToInsert = (MenuItem)elementToCopy.Clone();
            return itemToInsert;

        }

        public static Dictionary<MenuItem, string> MenuItemSearch(MenuItem root, string search, string currentPath)
        {
            currentPath += root.Title + " -> ";
            Dictionary<MenuItem, string> searchingItems = new Dictionary<MenuItem, string>();
           
            foreach (var item in root.Items)
            {
                string path = currentPath;

                if (item.Title.ToLower().Contains(search.ToLower()))
                {
                    path += item.Title;
                    searchingItems.Add(item, path);
                }

                if (item.Items.Count > 0)
                {
                    var newResults = MenuItemSearch(item, search, path);

                    foreach (var newResult in newResults)
                    {
                        searchingItems.Add(newResult.Key, newResult.Value);
                    }
                }
            }

            return searchingItems;
        }

        public TreeMenu(int productId)
        {
            this.productId = productId;
            _context = new AppDbContext();
            loader = new dbDataLoader();
        }
    }
}

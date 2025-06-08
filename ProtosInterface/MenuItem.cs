using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProtosInterface
{
    public class MenuItem: ICloneable
    {
        public string itemName;
        public int itemId;
        Dictionary<string, int> productComponentDictionary = new Dictionary<string, int>();

        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        public MenuItem(int id, string name)
        {
            itemId = id;
            itemName = name;
            Title = name;
            this.Items = new ObservableCollection<MenuItem>();
        }

        public string Title
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public string ShortName
        {
            get 
            {
                return CodeCleaner.ParseProductName(Title); 
            }
            set { itemName = value; }
        }

        public int Id
        {
            get { return itemId; }
            set { itemId = value; }
        }
        public double Amount { get; set; }
        [JsonIgnore]
        public MenuItem Parent { get; set; }

        public ObservableCollection<MenuItem> Items { get; set; }

        public object Clone()
        {
            var item = new MenuItem(itemName, itemId, DeepCopy(productComponentDictionary), Title, DeepCopy(Items), Amount);
            changeParent(item);
            return item;
        }

        private void changeParent(MenuItem item)
        {
            if (item.Items.Count > 0)
            {
                foreach (MenuItem doughterItem in item.Items)
                {
                    doughterItem.Parent = item;
                    if (doughterItem.Items.Count > 0)
                    {
                        changeParent(doughterItem);
                    }
                }
            }
        }

        public MenuItem(string itemName, int itemId, Dictionary<string, int> productComponentDictionary, string Title, ObservableCollection<MenuItem> Items, double amount)
        {
            this.itemName = itemName;
            this.itemId = itemId;
            this.productComponentDictionary = productComponentDictionary;
            this.Title = Title;
            this.Items = Items;
            this.Amount = amount;
        }

        Dictionary<TKey, TValue> DeepCopy<TKey, TValue>(Dictionary<TKey, TValue> original)
        {
            var json = JsonSerializer.Serialize(original);
            return JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(json);
        }

        ObservableCollection<TValue> DeepCopy<TValue>(ObservableCollection<TValue> original)
        {
            var json = JsonSerializer.Serialize(original);
            return JsonSerializer.Deserialize<ObservableCollection<TValue>>(json);
        }
    }
}

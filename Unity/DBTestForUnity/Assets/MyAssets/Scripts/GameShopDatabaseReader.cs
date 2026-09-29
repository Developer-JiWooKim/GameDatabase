using UnityEngine;
using SQLite;
using System.Collections.Generic;

namespace GameShop
{
    public class ItemRow
    {
        public int itemId { get; set; }
        public string name { get; set; }
        public int price { get; set; }
    }

    public class GameShopDatabaseReader : MonoBehaviour
    {
        [SerializeField] private SQLiteAsset databaseAsset;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            using (SQLiteConnection connection = databaseAsset.CreateConnection())
            {
                List<ItemRow> items = connection.Query<ItemRow>(@"
                    SELECT itemId, name, price 
                    FROM Item"
                );
                if (items.Count == 0)
                {
                    Debug.Log("No Item");
                }
                else
                {
                    foreach (var item in items)
                    {
                        Debug.Log($"ID: {item.itemId}, Item: {item.name}, Price: {item.price}");
                    }
                }

            }
        }
    }
}

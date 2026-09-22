using Microsoft.Data.Sqlite;

namespace GameDatabaseLab
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      string connectionString = "Data Source=GameShop.db";
      string createTableSql = @"
          CREATE TABLE IF NOT EXISTS Player(
            playerId INTEGER PRIMARY KEY,
            name TEXT NOT NULL,
            gold INTEGER NOT NULL CHECK (gold >= 0)
          );
          
          CREATE TABLE IF NOT EXISTS Item(
            itemId INTEGER PRIMARY KEY,
            name TEXT NOT NULL,
            price INTEGER NOT NULL CHECK (price > 0)
          );

          CREATE TABLE IF NOT EXISTS Inventory(
            playerId INTEGER NOT NULL,
            itemId INTEGER NOT NULL,
            quantity INTEGER NOT NULL CHECK(quantity >= 0),
            PRIMARY KEY(playerId, itemId),
            FOREIGN KEY (playerId) REFERENCES Player(playerId),
            FOREIGN KEY (itemId) REFERENCES Item(itemId)
          );
        ";

      using (SqliteConnection connection = new SqliteConnection(connectionString))
      {
        connection.Open();

        using (SqliteCommand command = connection.CreateCommand())
        {
          command.CommandText = "PRAGMA foreign_keys = ON;";
          command.ExecuteNonQuery();

          // 실습 1 INSERT Player, Item Datas
          // command.CommandText += createTableSql;
          // command.CommandText += @"
          //   INSERT OR IGNORE INTO Player (playerId, name, gold) 
          //   VALUES (1, '민지', 100);

          //   INSERT OR IGNORE INTO Item (itemId, name, price) 
          //   VALUES (1, '회복 포션', 30);
          // ";

          // 실습 2 INSERT Item - 철 검
          // command.CommandText += @"
          //   INSERT OR IGNORE INTO Item (itemId, name, price)
          //   VALUES ($itemId, $name, $price);          
          // ";
          // command.CommandText += @"
          //   INSERT OR IGNORE INTO Inventory(playerId, itemId, quantity)
          //   VALUES (1, 2, 1);
          // ";

          // command.Parameters.AddWithValue($"itemId", 2);
          // command.Parameters.AddWithValue($"name", "철 검");
          // command.Parameters.AddWithValue($"price", 100);

          // int changedRows = command.ExecuteNonQuery();
          // Console.WriteLine(changedRows + "건의 아이템을 등록했습니다.");

          //실습 2 SELECT Inventory
          command.CommandText = @"
            SELECT Player.name
            FROM Player
            WHERE Player.playerId = 1;
          ";

          command.Parameters.Clear();

          using (SqliteDataReader reader = command.ExecuteReader())
          {
            if (reader.Read())
            {
              string playerName = reader.GetString(0);
              Console.WriteLine(playerName);
            }
          }

          command.CommandText = @"
            SELECT Item.name, Inventory.Quantity
            FROM Inventory
            JOIN Item ON Inventory.itemId = Item.itemId
            WHERE Inventory.playerId = $playerId;
          ";

          command.Parameters.Clear(); // 이전 파라미터 남아있을 수 있으니 초기화
          command.Parameters.AddWithValue("$playerId", 1);
          // command.ExecuteNonQuery();

          using (SqliteDataReader reader = command.ExecuteReader())
          {
            Console.WriteLine("--- [ Inventory List ] ---");

            while (reader.Read())
            {
              string itemName = reader.GetString(0);
              int quantity = reader.GetInt32(1);

              Console.WriteLine($"Item: {itemName} | Quantity: {quantity}");
            }
          }

          // 실습 1 INSERT Inventory
          // command.CommandText += @"
          //   INSERT OR IGNORE INTO Inventory (playerId, itemId, quantity) 
          //   VALUES ($playerId, $itemId, $quantity);
          // ";

          // command.Parameters.AddWithValue("$playerId", 1);
          // command.Parameters.AddWithValue("$itemId", 1);
          // command.Parameters.AddWithValue("$quantity", 5);

          // command.ExecuteNonQuery();
        }
        // 실습 1
        // Console.WriteLine("Database and tables created Succesfully.");
      }
    }
  }
}
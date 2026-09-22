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
          command.CommandText += createTableSql;
          command.CommandText += @"
            INSERT OR IGNORE INTO Player (playerId, name, gold) 
            VALUES (1, '민지', 100);
            
            INSERT OR IGNORE INTO Item (itemId, name, price) 
            VALUES (1, '회복 포션', 30);
          ";

          command.CommandText += @"
            INSERT OR IGNORE INTO Inventory (playerId, itemId, quantity) 
            VALUES ($playerId, $itemId, $quantity);
          ";

          command.Parameters.AddWithValue("$playerId", 1);
          command.Parameters.AddWithValue("$itemId", 1);
          command.Parameters.AddWithValue("$quantity", 5);

          command.ExecuteNonQuery();
        }

        Console.WriteLine("Database and tables created Succesfully.");
      }
    }
  }
}
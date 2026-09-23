using Microsoft.Data.Sqlite;

namespace GameDatabaseLab
{
  public static class DatabaseManager
  {
    public const string ConnectionString = "Data Source=GameShop.db";

    /// <summary>
    /// Create Database Table
    /// </summary>
    public static void InitializeDatabase()
    {
      using (SqliteConnection connection = new SqliteConnection(ConnectionString))
      {
        connection.Open();

        // Foreign Key 활성화
        using (SqliteCommand pragma = connection.CreateCommand())
        {
          pragma.CommandText = "PRAGMA foreign_keys = ON";
          pragma.ExecuteNonQuery();
        }

        string createTablePlayerSql = @"
                CREATE TABLE IF NOT EXISTS Player(
                  playerId INTEGER PRIMARY KEY,
                  name TEXT NOT NULL,
                  gold INTEGER NOT NULL CHECK (gold >= 0)
                );
        ";

        string createTableItemSql = @"
                CREATE TABLE IF NOT EXISTS Item(
                  itemId INTEGER PRIMARY KEY,
                  name TEXT NOT NULL,
                  price INTEGER NOT NULL CHECK (price > 0)
                );
        ";

        string createTableInventorySql = @"
                CREATE TABLE IF NOT EXISTS Inventory(
                  playerId INTEGER NOT NULL,
                  itemId INTEGER NOT NULL,
                  quantity INTEGER NOT NULL CHECK(quantity >= 0),
                  PRIMARY KEY(playerId, itemId),
                  FOREIGN KEY(playerId) REFERENCES Player(playerId),
                  FOREIGN KEY(itemId) REFERENCES Item(itemId)
                );
        ";

        using (SqliteCommand createTableCommand = connection.CreateCommand())
        {
          createTableCommand.CommandText = createTablePlayerSql;
          createTableCommand.CommandText += createTableItemSql;
          createTableCommand.CommandText += createTableInventorySql;

          createTableCommand.ExecuteNonQuery();
        }

        // 각 테이블에 기본 데이터 삽입
        InsertData(connection);
      }
      Console.WriteLine("Database and tables initialized successfully.");
    }

    private static void InsertData(SqliteConnection connection)
    {
      string insertDataSql = @"
              INSERT OR IGNORE INTO Player(playerId, name, gold)
              VALUES (1, '민지', 100);

              INSERT OR IGNORE INTO Item(itemId, name, price)
              VALUES (1, '회복 포션', 30);

              INSERT OR IGNORE INTO Item(itemId, name, price)
              VALUES (2, '철 검', 100);

              INSERT OR IGNORE INTO Inventory(playerId, itemId, quantity)
              VALUES (1, 1, 5);
      ";

      using (SqliteCommand insertData = connection.CreateCommand())
      {
        insertData.CommandText = insertDataSql;
        insertData.ExecuteNonQuery();
      }
    }

    public static void ResetData()
    {
      using (SqliteConnection connection = new(ConnectionString))
      {
        connection.Open();

        using (SqliteCommand pragma = connection.CreateCommand())
        {
          pragma.CommandText = "PRAGMA foreign_keys = ON";
          pragma.ExecuteNonQuery();
        }

        string updateDataSql = @"
                UPDATE Player
                SET gold = 100;

                UPDATE Inventory
                SET quantity = 0
                WHERE playerId = $playerId AND itemId = $itemId;
        ";

        using (SqliteCommand updateData = connection.CreateCommand())
        {
          updateData.CommandText = updateDataSql;
          updateData.Parameters.AddWithValue("$playerId", 1);
          updateData.Parameters.AddWithValue("$itemId", 1);
          updateData.ExecuteNonQuery();
        }
      }
    }
  }
}
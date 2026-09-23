using Microsoft.Data.Sqlite;

namespace GameDatabaseLab
{
  public class ShopService
  {
    private readonly string _connectionString;
    public ShopService(string connectionString = DatabaseManager.ConnectionString)
    {
      _connectionString = connectionString;
    }

    public void PrintPlayerInventory(int playerId)
    {
      // playerId 조회
      using (SqliteConnection connection = new(_connectionString))
      {
        connection.Open();

        string? playerName = GetPlayerName(connection, playerId);
        if (string.IsNullOrEmpty(playerName))
        {
          Console.WriteLine($"Player ID {playerId}를 찾을 수 없습니다.");
          return;
        }

        Console.WriteLine($"\n----- [ {playerName}의 인벤토리 목록 ] -----");


        // SELECT Sql로 플레이어 아이디와 일치하는 인벤토리 목록을 얻어와 Console로 출력
        string selectInventorySql = @"
                SELECT Item.name, Inventory.quantity
                FROM Inventory
                JOIN Item ON Inventory.itemId = Item.itemId
                WHERE Inventory.playerId = $playerId;
        ";

        using (SqliteCommand selectQueryCommand = connection.CreateCommand())
        {
          selectQueryCommand.CommandText = selectInventorySql;
          selectQueryCommand.Parameters.AddWithValue("$playerId", playerId);

          using (SqliteDataReader reader = selectQueryCommand.ExecuteReader())
          {
            bool hasItem = false;
            while (reader.Read())
            {
              hasItem = true;
              string itemName = reader.GetString(0);
              int quantity = reader.GetInt32(1);

              Console.WriteLine($"Item: {itemName} | Quantity: {quantity}");
            }

            if (!hasItem)
            {
              Console.WriteLine("인벤토리가 비어 있습니다.");
            }
          }
        }
      }
    }

    // Player 테이블에서 일치하는 playerId 조회 메소드
    private string? GetPlayerName(SqliteConnection connection, int playerId)
    {
      string selectPlayerIdSql = @"
              SELECT name
              FROM Player
              WHERE playerId = $playerId;
      ";

      using (SqliteCommand command = connection.CreateCommand())
      {
        command.CommandText = selectPlayerIdSql;
        command.Parameters.AddWithValue("$playerId", playerId);

        var result = command.ExecuteScalar();
        return result != null ? result.ToString() : null;
      }
    }

    public void BuyItemTransaction(int playerId, int itemId, int price)
    {
      using (SqliteConnection connection = new(_connectionString))
      {
        connection.Open();

        using (SqliteCommand command = connection.CreateCommand())
        {
          // Foreign Key 활성화
          using (SqliteCommand pragmaCommand = connection.CreateCommand())
          {
            pragmaCommand.CommandText = "PRAGMA foreign_keys = ON";
            pragmaCommand.ExecuteNonQuery();
          }

          // Transaction 실행
          using (SqliteTransaction transaction = connection.BeginTransaction())
          {
            try
            {
              // 골드 차감 
              using (SqliteCommand spendGold = connection.CreateCommand())
              {
                spendGold.Transaction = transaction;
                spendGold.CommandText = @"
                    UPDATE Player
                    SET gold = gold - $price
                    WHERE playerId = $playerId AND gold >= $price;
                ";

                spendGold.Parameters.AddWithValue("$price", price);
                spendGold.Parameters.AddWithValue("$playerId", playerId);

                if (spendGold.ExecuteNonQuery() != 1)
                {
                  throw new InvalidOperationException("일치하는 플레이어가 없거나 골드가 부족합니다.");
                }
              }

              // 인벤토리 아이템 수량 증가
              using (SqliteCommand addItemToInventory = connection.CreateCommand())
              {
                addItemToInventory.CommandText = @"
                    UPDATE Inventory
                    SET quantity = quantity + 1
                    WHERE playerId = $playerId AND itemId = $itemId;
                ";

                addItemToInventory.Parameters.AddWithValue("$playerId", playerId);
                addItemToInventory.Parameters.AddWithValue("$itemId", itemId);

                if (addItemToInventory.ExecuteNonQuery() != 1)
                {
                  throw new InvalidOperationException("일치하는 아이템이 인벤토리에 없거나 수량 업데이트에 실패했습니다.");
                }
              }

              transaction.Commit();

              Console.WriteLine("구매를 완료했습니다.");
            }
            catch (Exception ex)
            {
              transaction.Rollback();

              Console.WriteLine("구매를 실패했습니다.");
            }
          }
        }
      }
    }

    public void BuyItemTransaction(int playerId, int itemId, int price, int count)
    {
      if (count <= 0)
      {
        Console.WriteLine("구매 실패: 구매 수량은 1개 이상이어야 합니다.");
        return;
      }

      int totalPrice = price * count;

      using (SqliteConnection connection = new(_connectionString))
      {
        connection.Open();

        // Foreign Key 활성화
        using (SqliteCommand pragmaCommand = connection.CreateCommand())
        {
          pragmaCommand.CommandText = "PRAGMA foreign_keys = ON";
          pragmaCommand.ExecuteNonQuery();
        }

        using (SqliteTransaction transaction = connection.BeginTransaction())
        {
          try
          {
            // totalPrice만큼 소지한 골드 차감
            using (SqliteCommand spendGold = connection.CreateCommand())
            {
              spendGold.Transaction = transaction;
              spendGold.CommandText = @"
                  UPDATE Player
                  SET gold = gold - $totalPrice
                  WHERE playerId = $playerId AND gold >= $totalPrice;
              ";
              spendGold.Parameters.AddWithValue("$totalPrice", totalPrice);
              spendGold.Parameters.AddWithValue("$playerId", playerId);

              if (spendGold.ExecuteNonQuery() != 1)
              {
                throw new InvalidOperationException($"골드가 부족합니다. (필요: {totalPrice}G)");
              }
            }

            using (SqliteCommand addItemInventory = connection.CreateCommand())
            {
              addItemInventory.Transaction = transaction;
              addItemInventory.CommandText = @"
                  UPDATE Inventory
                  SET quantity = quantity + $count
                  WHERE playerId = $playerId AND itemId = $itemId;
              ";

              addItemInventory.Parameters.AddWithValue("$count", count);
              addItemInventory.Parameters.AddWithValue("$playerId", playerId);
              addItemInventory.Parameters.AddWithValue("$itemId", itemId);

              if (addItemInventory.ExecuteNonQuery() != 1)
              {
                throw new InvalidOperationException("인벤토리에 해당 아이템이 존재하지 않습니다.");
              }
            }

            transaction.Commit();
            Console.WriteLine($"구매 성공: 아이템 {count}개 구매 완료! (총 {totalPrice}G 소모)");
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            Console.WriteLine($"구매 실패: {ex.Message}");
          }
        }
      }
    }
  }
}
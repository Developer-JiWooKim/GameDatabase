using Microsoft.Data.Sqlite;

namespace GameDatabaseLab
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      DatabaseManager.ResetData();

      //PracticeBuyItemTransaction();
      //PracticeFailBuyItemTransaction();
      AppliedPracticeBuyItemTransaction();
    }

    private static void PracticeBuyItemTransaction()
    {
      // DB 및 데이터 테이블 초기화
      //DatabaseManager.InitializeDatabase();

      ShopService shopService = new();

      shopService.PrintPlayerInventory(playerId: 1);

      Console.WriteLine("\n아이템(회복 포션) 구매 시도");
      shopService.BuyItemTransaction(playerId: 1, itemId: 1, price: 30);

      shopService.PrintPlayerInventory(playerId: 1);
    }

    private static void PracticeFailBuyItemTransaction()
    {
      ShopService shopService = new();

      Console.WriteLine("----- 트랜잭션 롤백 테스트 시작 -----");

      SetPlayerGold(playerId: 1, gold: 10);
      Console.WriteLine("플레이어의 골드를 10으로 변경");

      shopService.PrintPlayerInventory(playerId: 1);

      Console.WriteLine("회복 포션(price:30) 구매 시도(골드 부족 상태)");
      shopService.BuyItemTransaction(playerId: 1, itemId: 1, price: 30);

      shopService.PrintPlayerInventory(playerId: 1);


      SetPlayerGold(playerId: 1, gold: 100);
      Console.WriteLine("플레이어 골드를 100으로 변경(복구)");

      Console.WriteLine("회복 포션(price:30) 구매 시도");
      shopService.BuyItemTransaction(playerId: 1, itemId: 1, price: 30);

      shopService.PrintPlayerInventory(playerId: 1);

    }

    private static void AppliedPracticeBuyItemTransaction()
    {
      ShopService shopService = new();
      Console.WriteLine("----- 응용 실습(구매 수량 입력) 테스트 시작 -----");

      Console.WriteLine("\n회복 포션(price:30) 0개 구매 시도(잘못된 수량 입력 테스트: count <= 0)");
      shopService.BuyItemTransaction(playerId: 1, itemId: 1, price: 30, count: 0);
      shopService.PrintPlayerInventory(playerId: 1);

      Console.WriteLine("\n회복 포션(price:30) 5개 구매 시도(골드 부족 테스트: 소지한 골드 초과 구매 시도)");
      shopService.BuyItemTransaction(playerId: 1, itemId: 1, price: 30, count: 5);
      shopService.PrintPlayerInventory(playerId: 1);

      Console.WriteLine("\n회복 포션(price:30) 3개 구매 시도(정상 다량 구매 테스트)");
      shopService.BuyItemTransaction(playerId: 1, itemId: 1, price: 30, count: 3);
      shopService.PrintPlayerInventory(playerId: 1);

    }

    /// <summary>
    /// 테스트용: Player의 gold를 특정 값으로 변경하는 메소드
    /// </summary>
    private static void SetPlayerGold(int playerId, int gold)
    {
      using (SqliteConnection connection = new(DatabaseManager.ConnectionString))
      {
        connection.Open();
        using (SqliteCommand updateGold = connection.CreateCommand())
        {
          updateGold.CommandText = @"
              UPDATE Player
              SET gold = $gold
              WHERE playerId = $playerId;
          ";
          updateGold.Parameters.AddWithValue("$gold", gold);
          updateGold.Parameters.AddWithValue("$playerId", playerId);
          updateGold.ExecuteNonQuery();
        }
      }
    }
  }
}
using LiteDB;


namespace GameNoSqlLab
{
  public class GameLog
  {
    public int Id { get; set; }
    public string EventType { get; set; } = "";
    public int PlayerId { get; set; }
    public string Message { get; set; } = "";
    public DateTime TimeStamp { get; set; }
  }

  public class QuestProgress
  {
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int KillCount { get; set; }
    public int TargetCount { get; set; }
    public int BonusCount { get; set; }
  }

  internal class Program
  {
    private static void Main(string[] args)
    {
      // PlayerLog("PlayerJoined", 1, "Player 1 has joined the game."); // 플레이어 로그 추가

      // SearchLogs(1); // NoSql _id에 해당하는 데이터를 찾아서 출력
      // SearchLog(10); // 해당하는 id가 없을때는 어떻게 되는지 시도


      // InsertPlayerQuestProgress(2); // 퀘스트 데이터 추가
      // SearchQuestProgresses(); // 모든 데이터 검색 후 출력
      // PlayerQuestUpdate(2); // 해당 플레이어 아이디의 데이터 업데이트
      // SearchQuestProgress(2); // 제대로 업데이트 됐는지 확인
      // DeletePlayerQuest(3); // NoSql _id에 해당하는 데이터 삭제
    }

    /// <summary>
    /// 플레이어 로그 생성 및 DB에 추가
    /// </summary>
    private static void PlayerLog(string eventType, int playerId, string message)
    {
      using (LiteDatabase db = new LiteDatabase("GameLogs.db"))
      {
        ILiteCollection<GameLog> logs = db.GetCollection<GameLog>("logs");
        GameLog logEntry = new GameLog
        {
          EventType = eventType,
          PlayerId = playerId,
          Message = message,
          TimeStamp = DateTime.Now
        };
        logs.Insert(logEntry);
        foreach (GameLog log in logs.FindAll())
        {
          Console.WriteLine($"[SQL ID:{log.Id}][{log.TimeStamp}] {log.EventType} - Player {log.PlayerId}: {log.Message}");
        }
      }
    }

    /// <summary>
    /// 해당하는 Id(NoSql _id)의 데이터 출력
    /// </summary>
    private static void SearchLog(int id)
    {
      using (LiteDatabase db = new LiteDatabase("GameLogs.db"))
      {
        ILiteCollection<GameLog> logs = db.GetCollection<GameLog>("logs");
        GameLog selectedLog = logs.FindById(id);
        if (selectedLog != null)
        {
          Console.WriteLine($"[SQL ID:{selectedLog.Id}][{selectedLog.TimeStamp}] {selectedLog.EventType} - Player {selectedLog.PlayerId}: {selectedLog.Message}");
        }
        else
        {
          Console.WriteLine($"ID:{id} 로그가 없음");
        }
      }
    }

    /// <summary>
    /// 해당하는 playerId의 모든 메세지 출력 
    /// </summary>
    private static void SearchLogs(int playerId)
    {
      using (LiteDatabase db = new("GameLogs.db"))
      {
        ILiteCollection<GameLog> logs = db.GetCollection<GameLog>("logs");
        foreach (GameLog log in logs.Find(x => x.PlayerId == playerId))
        {
          Console.WriteLine(log.Message);
        }
      }
    }

    /// <summary>
    /// 새로운 퀘스트 데이터 생성 및 DB에 추가
    /// </summary>
    private static void InsertPlayerQuestProgress(int id)
    {
      using (LiteDatabase db = new("QuestProgress.db"))
      {
        ILiteCollection<QuestProgress> quests = db.GetCollection<QuestProgress>("quests");
        QuestProgress quest = new QuestProgress
        {
          PlayerId = id,
          KillCount = 0,
          TargetCount = 0,
          BonusCount = 0
        };
        quests.Insert(quest);
      }
    }

    /// <summary>
    /// 모든 퀘스트 데이터 출력
    /// </summary>
    private static void SearchQuestProgresses()
    {
      using (LiteDatabase db = new("QuestProgress.db"))
      {
        ILiteCollection<QuestProgress> quests = db.GetCollection<QuestProgress>("quests");
        foreach (QuestProgress quest in quests.FindAll())
        {
          Console.WriteLine($"[SQL ID:{quest.Id}] - Player: {quest.PlayerId}|[KillCount: {quest.KillCount}][TargetCount: {quest.TargetCount}][BonusCount: {quest.BonusCount}]");
        }
      }
    }

    /// <summary>
    /// 해당하는 ID의 퀘스트 데이터 출력
    /// </summary>
    private static void SearchQuestProgress(int id)
    {
      using (LiteDatabase db = new LiteDatabase("QuestProgress.db"))
      {
        ILiteCollection<QuestProgress> quests = db.GetCollection<QuestProgress>("quests");
        QuestProgress quest = quests.FindById(id);
        if (quest != null)
        {
          Console.WriteLine($"[SQL ID:{quest.Id}] - Player: {quest.PlayerId}|[KillCount: {quest.KillCount}][TargetCount: {quest.TargetCount}][BonusCount: {quest.BonusCount}]");
        }
        else
        {
          Console.WriteLine($"ID:{id}에 해당하는 퀘스트 진행동가 없음");
        }
      }
    }

    /// <summary>
    /// 해당하는 ID의 퀘스트 데이터 Update(KillCount, TargetCount +1)
    /// </summary>
    private static void PlayerQuestUpdate(int id)
    {
      using (LiteDatabase db = new("QuestProgress.db"))
      {
        ILiteCollection<QuestProgress> quests = db.GetCollection<QuestProgress>("quests");
        QuestProgress quest = quests.FindOne(x => x.PlayerId == id);
        if (quest != null)
        {
          quest.KillCount += 1;
          quest.TargetCount += 1;

          quests.Update(quest);
          Console.WriteLine("Update Complete");
        }
      }
    }

    /// <summary>
    /// 해당하는 ID(NoSql _id)의 DB 데이터를 삭제
    /// </summary>
    private static void DeletePlayerQuest(int id)
    {
      using (LiteDatabase db = new("QuestProgress.db"))
      {
        ILiteCollection<QuestProgress> quests = db.GetCollection<QuestProgress>("quests");
        bool deleted = quests.Delete(id);
        Console.WriteLine($"ID:{id}의 퀘스트 데이터 삭제 시도: " + deleted);
      }
    }
  }
}
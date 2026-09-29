using System.IO;
using LiteDB;
using UnityEngine;

namespace GameLog
{
    public class GameLog
    {
        public int Id { get; set; }
        public string EventType { get; set; } = "";
        public int PlayerId { get; set; }
        public string Message { get; set; } = "";
    }

    public class GameLogReader : MonoBehaviour
    {
        private void Start()
        {
            string dbPath = Path.Combine(Application.persistentDataPath, "GameLogs.db");
            if (!File.Exists(dbPath))
            {
                string sourcePath = Path.Combine(Application.streamingAssetsPath, "GameLogs.db");
                if (!File.Exists(sourcePath))
                {
                    Debug.LogError("Sourch database file not found: " + sourcePath);
                    return;
                }

                File.Copy(sourcePath, dbPath);
            }

            using (LiteDatabase db = new LiteDatabase(dbPath))
            {
                ILiteCollection<GameLog> logs = db.GetCollection<GameLog>("logs");
                GameLog log = logs.FindOne(x => x.Id > 0);

                if (log != null)
                {
                    Debug.Log($"[ID:{log.Id}][PlayerID:{log.PlayerId}][EventType:{log.EventType}]:{log.Message}");
                }
                else
                {
                    Debug.Log("No Log");
                }
            }
        }
    }
}

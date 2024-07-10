using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    [Serializable]
    public class GameData
    {
        public Character character;
        public string CurrentTime;
        public string CurrentSceneName;
        public int SceneMetaData;
        public Quest CurrentQuest;
        public int LastQuestID = 0;
    }

    public class GameManager
    {

        public GameData Data = new GameData();
        private string path;
        public float GameSpeed = 1.0f;
        public bool IsGameSpeedUp = false;
        public Action<int> OnKillMonster;

        public int GetGameSleepTime(float SleepTime)
        {
            return (int)(SleepTime / GameSpeed);
        }
        public void Initialize()
        {
            path = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        }

        public void SaveGame(int index)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
            };
            Data.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jsonStr = JsonConvert.SerializeObject(Data, settings);
            File.WriteAllText(path + $"/SaveData{index}.json", jsonStr);
        }

        public bool LoadGame(int index)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.All,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                };
                string file = File.ReadAllText(path + $"/SaveData{index}.json");
                GameData data = JsonConvert.DeserializeObject<GameData>(file, settings);
                if (data != null)
                {
                    this.Data = data;
                    Game.Player = data.character;
                }
                else
                {
                    this.Data = null;
                }
                if (string.IsNullOrEmpty(data.character.Name)) return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
        public void ToggleSpeed()
        {
            if (IsGameSpeedUp == true)
            {
                GameSpeed = 2.0f;
                IsGameSpeedUp = false;
            }
            else
            {
                GameSpeed = 1.0f;
                IsGameSpeedUp = true;
            }
        }

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    class Program
    {
        static void Main(string[] args)
        {

            /*
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
            };
            string file = File.ReadAllText(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "/SaveData.json");
            GameData data = JsonConvert.DeserializeObject<GameData>(file);
            */
            Game g = new Game();

        }
    }
}

using NAudio.CoreAudioApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    class LoadScene : Scene
    {
        public override string Title { get; protected set; } = "";
        public override void EnterScene()
        {
            selectedOptionIndex = 0;
            DrawScene();
            NextScene();
        }

        private bool[] isCorretData = new bool[3];

        public override void NextScene()
        {
            do
            {
                Renderer.DrawBorder(Title);

                var startX = 50;
                for (int i = 0; i < 3; i++)
                {

                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.All,
                        PreserveReferencesHandling = PreserveReferencesHandling.All,
                    };
                    try
                    {
                        string file = File.ReadAllText(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + $"/SaveData{i}.json");
                        GameData data = JsonConvert.DeserializeObject<GameData>(file, settings);
                        Renderer.Draw(startX, i * 9 + 3, $"레벨 : {data.character.Level} ");
                        string pos = "마을";

                        if (data.CurrentSceneName == "Battle")
                        {
                            int stage = (int)data.SceneMetaData;
                            var dungeonName = GameTable.BattleSceneNames[stage / 5];
                            pos = $"{dungeonName}<y{stage % 5 + 1}스테이지w>";
                        }
                        Renderer.Draw(startX, i * 9 + 5, $"위치 : {pos}");
                        Renderer.Draw(startX, i * 9 + 7, $"시간 : {data.CurrentTime}");
                        isCorretData[i] = true;
                    }
                    catch
                    {
                        Renderer.Draw(startX + 8, i * 9 + 5, "데이터 없음");
                        isCorretData[i] = false;
                    }
                }

                int length = 30;

                for (int i = 0; i < 3; i++)
                {
                    char color = 'd';

                    if (i == selectedOptionIndex)
                    {
                        color = 'g';
                    }

                    for (int j = 3; j <= 7; j++)
                    {
                        Renderer.Draw(startX - 3, i * 9 + j, $"{color}│ w");
                        Renderer.Draw(startX + length - 1, i * 9 + j, $"{color}│ w");
                    }
                    Renderer.Draw(startX - 3, i * 9 + 2, $"{color}┌ w");
                    Renderer.Draw(startX + length - 1, i * 9 + 2, $"{color}┐ w");

                    Renderer.Draw(startX - 3, i * 9 + 8, $"{color}└ w");
                    Renderer.Draw(startX + length - 1, i * 9 + 8, $"{color}┘ w");

                    Renderer.Draw(startX - 1, i * 9 + 2, $"{color}{new string('━', length)}w");
                    Renderer.Draw(startX - 1, i * 9 + 8, $"{color}{new string('━', length)}w");

                    Renderer.Draw(startX + 10, i * 9 + 2, $"세이브 {i + 1}");
                }

                GetInput();

                if(lastCommand == Command.Interact && isCorretData[selectedOptionIndex] == false)
                {
                    lastCommand = Command.Nothing;
                }

            } while (lastCommand != Command.Interact);
        }

        protected override void OnCommandMoveTop()
        {
            selectedOptionIndex = (selectedOptionIndex - 1 + 3) % 3;
        }
        protected override void OnCommandMoveBottom()
        {
            selectedOptionIndex = (selectedOptionIndex + 1 + 3) % 3;
        }
        protected override void OnCommandInteract()
        {
            if (isCorretData[selectedOptionIndex] == true)
            {
                Managers.Game.LoadGame(selectedOptionIndex);

                if (Managers.Game.Data.CurrentSceneName == "Main")
                {
                    Managers.Scene.EnterScene<MainScene>();
                }
                else if (Managers.Game.Data.CurrentSceneName == "Battle")
                {
                    BattleScene.DungeonName = GameTable.BattleSceneNames[(int)Managers.Game.Data.SceneMetaData / 5];
                    BattleScene.Stage = (int)Managers.Game.Data.SceneMetaData;
                    Managers.Scene.EnterScene<BattleScene>();
                }
            }
        }

        protected override void DrawScene()
        {


        }

    }
}

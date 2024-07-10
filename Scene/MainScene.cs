using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    class MainScene : Scene
    {
        public override string Title { get; protected set; } = "마을";

        #region Scene

        public override void EnterScene()
        {
            selectedOptionIndex = 0;
            // #1. 선택지 설정.
            Options.Clear();
            //Options.Add(Managers.Scene.GetOption("Inventory"));
            Options.Add(Managers.Scene.GetOption("WorldMap"));
            Options.Add(Managers.Scene.GetOption("ShowInfo"));
            Options.Add(Managers.Scene.GetOption("Inventory"));
            Options.Add(Managers.Scene.GetOption("Shop"));
            Options.Add(Managers.Scene.GetOption("Quest"));
            Options.Add(Managers.Scene.GetOption("Casino"));
            Options.Add(Managers.Scene.GetOption("Rest"));
            Options.Add(Managers.Scene.GetOption("SaveGame"));
            Options.Add(Managers.Scene.GetOption("Quit"));
            //Options.Add(Managers.Scene.GetOption("Shop"));

            BGMPlayer.Instance.music = "Main.mp3";
            BGMPlayer.Instance.PlayAsync(); // 음악파일명, 볼륨
            DrawScene();
        }

        public override void NextScene()
        {
            do
            {
                Renderer.DrawOptionsCenter(6, Options, selectedOptionIndex, Game.lobbyOffsetX);
                Renderer.ShowCharacterInfo();
                GetInput();

                if (lastCommand == Command.Interact && selectedOptionIndex == 5)
                {
                    Renderer.Down("저장중...");
                    lastCommand = Command.Nothing;
                    Renderer.DrawBorder(Title);
                }

            }
            while (lastCommand != Command.Interact);
        }

        protected override void DrawScene()
        {
            Renderer.DrawBorder(Title);
        }

        #endregion

        #region Input

        protected override void OnCommandExit()
        {

        }

        #endregion
    }
}

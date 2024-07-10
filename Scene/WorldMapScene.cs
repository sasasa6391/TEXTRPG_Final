using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class WorldMapScene : Scene
    {
        private const int dungeonCount = 5;

        const int startOffset = 8;

        public override string Title { get; protected set; } = "월드맵";
        public override void EnterScene()
        {
            Managers.Game.GameSpeed = 1.0f;

            selectedOptionIndex = 0;

            Options.Clear();
            for (int i = 1; i <= 5; i++)
            {
                Options.Add(Managers.Scene.GetOption($"Dungeon{i}"));
            }

            BGMPlayer.Instance.music = "WorldMap.mp3";
            BGMPlayer.Instance.PlayAsync(); // 음악파일명, 볼륨

            DrawScene();
        }

        public override void NextScene()
        {
            //Renderer.DrawKeyGuide("[방향키 ↑ ↓: 선택지 이동] [Enter: 결정]");
            Renderer.DrawCenter(startOffset, "y던전 리스트w", Game.lobbyOffsetX);
            while (true)
            {
                Renderer.DrawOptionsCenter(startOffset + 2, Options, selectedOptionIndex, Game.lobbyOffsetX);
                Renderer.ShowCharacterInfo();
                GetInput();
            }
        }
        protected override void DrawScene()
        {
            Renderer.DrawBorder(Title);
        }

        protected override void OnCommandMoveTop()
        {
            selectedOptionIndex = (selectedOptionIndex - 1 + dungeonCount) % dungeonCount;
        }
        protected override void OnCommandMoveBottom()
        {
            selectedOptionIndex = (selectedOptionIndex + 1 + dungeonCount) % dungeonCount;
        }


        protected override void OnCommandExit()
        {
            Managers.Scene.EnterScene<MainScene>();
        }
    }
}

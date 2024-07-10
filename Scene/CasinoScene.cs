using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using 김도명_TEXTRPG.Casino;

namespace 김도명_TEXTRPG
{
    class CasinoScene : Scene
    {
        public override string Title { get; protected set; } = "도 박 장";
        public override void EnterScene()
        {
            selectedOptionIndex = 0;
            BGMPlayer.Instance.music = "Casino.mp3";
            BGMPlayer.Instance.PlayAsync(); // 음악파일명, 볼륨
            DrawScene();
        }

        public override void NextScene()
        {
            while (true)
            {
                Renderer.DrawBorder(Title);
                Renderer.ShowCharacterInfo();

                if (Game.Player.Inventory.Gold == 0)
                {
                    Renderer.DrawCenter(Renderer.Height / 2 - 1, "당신 거지잖아?? 당장 나가!", Game.lobbyOffsetX);
                    Thread.Sleep(3000);
                    break;
                }
                else
                {
                    Options.Clear();
                    Options.Add(new ActionOption("세븐포커", "세븐포커", () =>
                    {
                        Renderer.ConsoleClear();
                        Renderer.DrawBorder();
                        Renderer.StopRenderThread();
                        SevenPoker sPoker = new SevenPoker();
                        sPoker.Init();
                        Renderer.RestartRenderThread();
                    }));
                    Options.Add(Managers.Scene.GetOption("Back"));

                    Renderer.DrawOptionsCenter(12, Options, selectedOptionIndex, Game.lobbyOffsetX);
                    GetInput();
                }
            }
            Managers.Scene.EnterScene<MainScene>();
        }
        protected override void DrawScene()
        {

            // ==== 캐릭터 정보 표시 ====


            //Renderer.PrintKeyGuide("[ESC : 뒤로가기]");
        }
    }
}

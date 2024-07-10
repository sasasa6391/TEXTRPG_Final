using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    class RestScene : Scene
    {
        public override string Title { get; protected set; } = "여 관";

        private bool isExit = false;
        public override void EnterScene()
        {
            //Options.Clear();
            //Options.Add(Managers.Scene.GetOption("NewGame"));
            selectedOptionIndex = 0;
            isExit = false;
            DrawScene();
            NextScene();
        }

        public override void NextScene()
        {
            while (isExit == false)
            {

                var options = new List<string>();

                if (Game.Player.Inventory.Gold < 100)
                {
                    options.Add("d휴식하기 100Gw");
                    selectedOptionIndex = 1;
                }
                else
                {
                    options.Add("휴식하기 100yGw");
                }

                options.Add("나가기");

                Renderer.DrawOptionsCenter(12, options, selectedOptionIndex, Game.lobbyOffsetX);

                GetInput();
            }
            Managers.Scene.EnterScene<MainScene>();

        }

        protected override void OnCommandMoveTop()
        {
            if (selectedOptionIndex > 0) selectedOptionIndex--;
        }
        protected override void OnCommandMoveBottom()
        {
            if (selectedOptionIndex < 1) selectedOptionIndex++;
        }
        protected override void OnCommandInteract()
        {
            if (selectedOptionIndex == 0)
            {
                var player = Game.Player;
                player.Inventory.Gold -= 100;
                player.Hp = player.HpMax;
                player.Mp = player.MpMax;
                Renderer.Down("휴식중...");
                DrawScene();
            }
            else
            {
                OnCommandExit();
            }
        }
        protected override void OnCommandExit()
        {
            isExit = true;
        }


        protected override void DrawScene()
        {
            Renderer.DrawBorder(Title);


            Renderer.ShowCharacterInfo();


        }



    }
}

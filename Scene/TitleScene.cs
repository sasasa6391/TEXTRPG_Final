using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class TitleScene : Scene
    {
        private enum TitleStep
        {
            Main,
            Option,
        }

        TitleStep step;
        
        public override void EnterScene()
        {
            BGMPlayer.Instance.music = "Start.mp3";
            BGMPlayer.Instance.PlayAsync(1f); // 음악파일명, 볼륨

            step = TitleStep.Main;
        }
        public override void NextScene()
        {
            Renderer.DrawKeyGuide("Developed by KDM. released May. 2024");


            selectedOptionIndex = 0;
            Options.Clear();
            Options.Add(Managers.Scene.GetOption("NewGame"));


            
            Options.Add(new ActionOption("LoadGame", "이어하기", () =>
            {
                Managers.Scene.EnterScene<LoadScene>();
            }));
            //Options.Add(new ActionOption("Option", "옵션", () => step = TitleStep.Option));
            Options.Add(Managers.Scene.GetOption("Quit"));


            DrawScene();
            while (step == TitleStep.Main)
            {
                Renderer.DrawOptionsCenter(20, Options, selectedOptionIndex);
                GetInput();
            }

            /*
            selectedOptionIndex = 0;
            Options.Clear();
            Options.Add(new ActionOption("2배속", "게임 2배속 ON", () => Managers.Game.ToggleSpeed()));
            

            DrawScene();
            while(step == TitleStep.Option)
            {
                Renderer.DrawOptionsCenter(20, Options, selectedOptionIndex);
                GetInput();
            }
            */
            
        }

        protected override void DrawScene()
        {
            Renderer.DrawBorder();


            Renderer.DrawCenter(6, @"y████████╗██╗  ██╗███████╗  ████████╗███████╗██╗  ██╗████████╗  ██████╗ ██████╗  ██████╗w ");
            Renderer.DrawCenter(7, @"y╚══██╔══╝██║  ██║██╔════╝  ╚══██╔══╝██╔════╝╚██╗██╔╝╚══██╔══╝  ██╔══██╗██╔══██╗██╔════╝w ");
            Renderer.DrawCenter(8, @"y   ██║   ███████║█████╗       ██║   █████╗   ╚███╔╝    ██║     ██████╔╝██████╔╝██║  ██╗w ");
            Renderer.DrawCenter(9, @"y   ██║   ██╔══██║██╔══╝       ██║   ██╔══╝   ██╔██╗    ██║     ██╔══██╗██╔═══╝ ██║  ╚██╗w");
            Renderer.DrawCenter(10, @"y   ██║   ██║  ██║███████╗     ██║   ███████╗██╔╝╚██╗   ██║     ██║  ██║██║     ╚██████╔╝w");
            Renderer.DrawCenter(11, @"y   ╚═╝   ╚═╝  ╚═╝╚══════╝     ╚═╝   ╚══════╝╚═╝  ╚═╝   ╚═╝     ╚═╝  ╚═╝╚═╝      ╚═════╝ w");
        }
    }
}

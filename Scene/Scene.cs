using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class Scene
    {
        public virtual string Title { get; protected set; }

        protected List<ActionOption> Options { get; set; } = new List<ActionOption>();

        protected int selectedOptionIndex = 0;
        protected Command lastCommand = Command.Nothing;

        /// <summary>
        /// 씬 진입 시 액션.
        /// </summary>
        public virtual void EnterScene()
        {

        }
        /// <summary>
        /// 다음 씬으로 넘어가기 위한 조건.
        /// </summary>
        public virtual void NextScene()
        {

        }
        /// <summary>
        /// 씬 보여주기.
        /// </summary>
        protected virtual void DrawScene()
        {

        }

        protected void GetInput()
        {
            // 입력 버퍼 비우기
            while (Console.KeyAvailable)
            {
                Console.ReadKey(false);
            }

            lastCommand = Command.Nothing;
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Tab:
                    lastCommand = Command.Tab;
                    break;
                case ConsoleKey.UpArrow:
                    SFXPlayer.Instance.music = "[SE]Action.mp3";
                    SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
                    lastCommand = Command.MoveTop;
                    break;
                case ConsoleKey.DownArrow:
                    SFXPlayer.Instance.music = "[SE]Action.mp3";
                    SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
                    lastCommand = Command.MoveBottom;
                    break;
                case ConsoleKey.LeftArrow:
                    SFXPlayer.Instance.music = "[SE]Action.mp3";
                    SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
                    lastCommand = Command.MoveLeft;
                    break;
                case ConsoleKey.RightArrow:
                    SFXPlayer.Instance.music = "[SE]Action.mp3";
                    SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
                    lastCommand = Command.MoveRight;
                    break;
                case ConsoleKey.Enter:
                    SFXPlayer.Instance.music = "[SE]Enter.mp3";
                    SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
                    lastCommand = Command.Interact;
                    break;
                case ConsoleKey.Escape:
                    SFXPlayer.Instance.music = "[SE]Enter.mp3";
                    SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
                    lastCommand = Command.Exit;
                    break;
                case ConsoleKey.F12:
                    SFXPlayer.Instance.music = "[SE]Enter.mp3";
                    SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
                    lastCommand = Command.F12;
                    break;
                default:
                    lastCommand = Command.Nothing;
                    break;
            };

            OnCommand(lastCommand);
        }


        private void OnCommand(Command command)
        {
            switch (command)
            {
                case Command.Tab: OnCommandTab(); break;
                case Command.MoveTop: OnCommandMoveTop(); break;
                case Command.MoveBottom: OnCommandMoveBottom(); break;
                case Command.MoveLeft: OnCommandMoveLeft(); break;
                case Command.MoveRight: OnCommandMoveRight(); break;
                case Command.Interact: OnCommandInteract(); break;
                case Command.Exit: OnCommandExit(); break;
                case Command.F12: OnCommandF12(); break;
            }
        }

        protected virtual void OnCommandTab()
        {
            Managers.Game.ToggleSpeed();
        }

        protected virtual void OnCommandF12()
        {
            Managers.Scene.EnterScene<TitleScene>();
        }
        protected virtual void OnCommandMoveTop()
        {
            selectedOptionIndex = (selectedOptionIndex - 1 + Options.Count) % Options.Count;
        }
        protected virtual void OnCommandMoveBottom()
        {
            selectedOptionIndex = (selectedOptionIndex + 1 + Options.Count) % Options.Count;
        }
        protected virtual void OnCommandMoveLeft()
        {

        }
        protected virtual void OnCommandMoveRight()
        {

        }
        protected virtual void OnCommandInteract()
        {
            if (Options.Count > 0)
                Options[selectedOptionIndex].Execute();
            
        }
        protected virtual void OnCommandExit()
        {
            if (Managers.Scene.PrevScene != null)
                Managers.Scene.GetOption("Back").Execute();
        }
    }
}

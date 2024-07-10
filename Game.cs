using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class Game
    {
        public const int lobbyOffsetX = 20;
        public static Character Player { get; set; }
        public Game()
        {
            GameStart();
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        public void GameStart()
        {
            Renderer rd = new Renderer();
            Managers.Game.Initialize();
            Managers.Scene.Initialize();
            Managers.Scene.EnterScene<TitleScene>();
        }
    }
}

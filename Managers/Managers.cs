using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public static class Managers
    {
        private static SceneManager scene = new SceneManager();
        private static GameManager game = new GameManager();
        public static SceneManager Scene => scene;
        public static GameManager Game => game;
    }   

}
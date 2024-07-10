using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG.Casino
{
    public class Utility
    {
        public static void gotoxy(int x, int y)
        {
            x += 5;
            y += 1;

            Console.SetCursorPosition(x, y);
        }
    }
}

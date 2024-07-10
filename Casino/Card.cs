using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG.Casino
{
    public class Card
    {
        public enum Shape
        {
            None = -1,
            Clover,
            Heart,
            Diamond,
            Spade,
            Last,
        }

        public enum Number
        {
            None = -1,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace,
            Last,
        }

        public Number number;
        public Shape shape;
        public bool isHide = false;

        public Card(Number number, Shape shape)
        {
            this.number = number;
            this.shape = shape;
        }

        public void Render(int x, int y)
        {

            string sNumber = GetNumberString(number);
            string sShape = GetShapeString(shape);

            if (isHide == true)
            {
                sNumber = "?";
                sShape = "??";
            }

            Utility.gotoxy(x, y);
            Console.WriteLine("┌─────────┐");
            Utility.gotoxy(x, y + 1);
            if (sNumber == "10")
            {
                Console.WriteLine("│{0}       │", sNumber);
            }
            else
            {
                Console.WriteLine("│ {0}       │", sNumber);
            }
            Utility.gotoxy(x, y + 2);
            Console.WriteLine("│         │");
            Utility.gotoxy(x, y + 3);
            Console.WriteLine("│         │");
            Utility.gotoxy(x, y + 4);
            Console.WriteLine("│    {0}   │", sShape);
            Utility.gotoxy(x, y + 5);
            Console.WriteLine("│         │");
            Utility.gotoxy(x, y + 6);
            Console.WriteLine("│         │");
            Utility.gotoxy(x, y + 7);

            if (sNumber == "10")
            {
                Console.WriteLine("│       {0}│", sNumber);
            }
            else
            {
                Console.WriteLine("│       {0} │", sNumber);
            }
            Utility.gotoxy(x, y + 8);
            Console.WriteLine("└─────────┘");
        }

        public static void EmptyRender(int x, int y)
        {
            for (int i = y; i < y + 9; i++)
            {
                Utility.gotoxy(x, i);
                Console.WriteLine("                 ");
            }
        }

        public static string GetShapeString(Shape shape)
        {
            switch (shape)
            {
                case Shape.Clover:
                    return "♣";
                case Shape.Heart:
                    return "♥";
                case Shape.Diamond:
                    return "◆";
                case Shape.Spade:
                    return "♠";
                default:
                    return "EMPTY";
            }
        }

        public static string GetNumberString(Number number)
        {
            switch (number)
            {
                case Number.Two:
                    return "2";
                case Number.Three:
                    return "3";
                case Number.Four:
                    return "4";
                case Number.Five:
                    return "5";
                case Number.Six:
                    return "6";
                case Number.Seven:
                    return "7";
                case Number.Eight:
                    return "8";
                case Number.Nine:
                    return "9";
                case Number.Ten:
                    return "10";
                case Number.Jack:
                    return "J";
                case Number.Queen:
                    return "Q";
                case Number.King:
                    return "K";
                case Number.Ace:
                    return "A";
                default:
                    return "EMPTY";
            }
        }


    }
}

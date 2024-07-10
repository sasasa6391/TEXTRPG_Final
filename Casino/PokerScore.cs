using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static 김도명_TEXTRPG.Casino.Card;

namespace 김도명_TEXTRPG.Casino
{
    public class PokerScore
    {
        public enum Score
        {
            None = -1,
            Top,
            OnePair,
            TwoPair,
            Triple,
            Straight,
            BackStraight,
            Mountain,
            Flush,
            FullHouse,
            FourCard,
            StraightFlush,
            BackStraightFlush,
            RoyalStraightFlush,
            Last,
        };

        public Score score;
        public Shape shape;
        public Number number;
        public Number number2;
        public PokerScore(Score score)
        {
            this.score = score;
            shape = Shape.None;
            number = Number.None;
            number2 = Number.None;
        }
        public PokerScore(Score score, Shape shape, Number number)
        {
            this.score = score;
            this.shape = shape;
            this.number = number;
            number2 = Number.None;
        }
        public PokerScore(Score score, Shape shape, Number number, Number number2)
        {
            this.score = score;
            this.shape = shape;
            this.number = number;
            this.number2 = number2;
        }
        public string GetTotalString()
        {
            string result = "";

            result += GetScoreString();
            result += "(";
            result += GetShapeString(shape);
            result += ",";
            result += GetNumberString(number);
            if (number2 != Number.None)
            {
                result += ",";
                result += GetNumberString(number2);
            }
            result += ")";

            return result;
        }

        private string GetScoreString()
        {
            switch (score)
            {
                case Score.RoyalStraightFlush:
                    return "로티플";
                case Score.BackStraightFlush:
                    return "백티플";
                case Score.StraightFlush:
                    return "스티플";
                case Score.FourCard:
                    return "포카드";
                case Score.FullHouse:
                    return "풀하우스";
                case Score.Flush:
                    return "플러쉬";
                case Score.Mountain:
                    return "마운틴";
                case Score.BackStraight:
                    return "백스트레이트";
                case Score.Straight:
                    return "스트레이트";
                case Score.Triple:
                    return "트리플";
                case Score.TwoPair:
                    return "투페어";
                case Score.OnePair:
                    return "원페어";
                case Score.Top:
                    return "탑";
                default:
                    return "EMPTY";
            }
        }

        public static bool CompareScore(PokerScore A, PokerScore B)
        {
            if (A.score != B.score)
            {
                return A.score > B.score;
            }

            if (A.score == Score.FourCard || A.score == Score.Triple || A.score == Score.OnePair || A.score == Score.Top)
            {
                if (A.number != B.number)
                {
                    return A.number > B.number;
                }
                else
                {
                    return A.shape > B.shape;
                }
            }
            else if (A.score == Score.TwoPair || A.score == Score.FullHouse)
            {
                if (A.number != B.number)
                {
                    return A.number > B.number;
                }
                else if (A.number2 != B.number2)
                {
                    return A.number2 > B.number2;
                }
                else
                {
                    return A.shape > B.shape;
                }
            }
            else
            {
                if (A.shape != B.shape)
                {
                    return A.shape > B.shape;
                }
                else
                {
                    return A.number > B.number;
                }
            }
        }

    }
}

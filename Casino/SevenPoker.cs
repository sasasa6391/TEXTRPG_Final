using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static 김도명_TEXTRPG.Casino.Card;
using static 김도명_TEXTRPG.Casino.PokerScore;

namespace 김도명_TEXTRPG.Casino
{
    public class SevenPoker
    {
        private int _playerCardCnt;
        private int _dealerCardCnt;
        private int _deckCnt;
        private int _totalBetMoney;
        private int _seedMoney;
        private Card[] _playerCards = new Card[10];
        private Card[] _dealerCards = new Card[10];
        private Card[] _deckCards = new Card[52];

        public void Exit()
        {
            return;
        }
        public void Init()
        {
            Console.ForegroundColor = ConsoleColor.White;
            //Console.Clear();

            _deckCnt = 0;
            _playerCardCnt = 0;
            _dealerCardCnt = 0;
            _totalBetMoney = 0;

            Utility.gotoxy(0, 0);
            _seedMoney = Game.Player.Inventory.Gold;
            Console.WriteLine("현재 잔액 : {0}", _seedMoney);


            //Console.Clear();

            SetDeck();
            ShowTable();

            for (int i = 0; i < 4; i++)
            {
                AddCard(0);
                AddCard(1, true);
            }

            SelectDropCard();


            Next();
        }

        public void SetDeck()
        {
            for (Shape i = Shape.Clover; i != Shape.Last; ++i)
            {
                for (Number j = Number.Two; j != Number.Last; ++j)
                {
                    _deckCards[_deckCnt++] = new Card(j, i);
                }
            }
            Shuffle();
        }

        void Shuffle()
        {
            for (int i = 0; i < 500; i++)
            {
                var rand = new Random(unchecked((int)DateTime.Now.Ticks) + i);

                int rNumber1 = rand.Next(0, _deckCnt);
                int rNumber2 = rand.Next(0, _deckCnt);

                var t = _deckCards[rNumber1];
                _deckCards[rNumber1] = _deckCards[rNumber2];
                _deckCards[rNumber2] = t;
            }
        }

        public void ShowTable()
        {
            for(int i=0; i<24; i++)
            {
                Utility.gotoxy(0, i);
                Console.WriteLine("                                                                                                        ");
            }

            Utility.gotoxy(0, 0);
            Console.WriteLine("잔액 : {0} 총 배팅 금액 : {1}", _seedMoney, _totalBetMoney);
            Utility.gotoxy(0, 2);
            Console.WriteLine("Dealer                                                                                                ");
            Utility.gotoxy(0, 14);
            Console.WriteLine("Player                                                                                                ");
        }

        public void ShowCardInfo()
        {
            for (int i = 0; i < _dealerCardCnt; i++)
            {
                _dealerCards[i].Render(14 * i + 5, 3);
            }

            for (int i = 0; i < _playerCardCnt; i++)
            {
                _playerCards[i].Render(14 * i + 5, 15);
            }

            PokerScore pScore = GetScore(0);

            Utility.gotoxy(25, 14);
            Console.WriteLine("                                                         ");
            Utility.gotoxy(25, 14);
            Console.WriteLine("플레이어 점수 : {0}", pScore.GetTotalString());
        }

        public void AddCard(int playerNumber, bool cHide = false)
        {
            if (playerNumber == 0)
            {
                _playerCards[_playerCardCnt++] = _deckCards[--_deckCnt];
            }
            else if (playerNumber == 1)
            {
                Card c = _deckCards[--_deckCnt];
                c.isHide = cHide;
                _dealerCards[_dealerCardCnt++] = c;
            }
            ShowCardInfo();
            Thread.Sleep(1000);
        }

        public void SelectDropCard()
        {
            Utility.gotoxy(5, 26);
            Console.WriteLine("버릴 카드를 선택해주세요!");

            int cIdx = 0;
            int cy = 24;

            Utility.gotoxy(10 + cIdx * 14, cy);
            Console.WriteLine("▲");

            bool selected = false;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    Utility.gotoxy(10 + cIdx * 14, cy);
                    Console.Write("  ");
                    ConsoleKeyInfo c = Console.ReadKey();

                    switch (c.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            cIdx--;
                            break;
                        case ConsoleKey.RightArrow:
                            cIdx++;
                            break;
                        case ConsoleKey.Enter:
                            for (int i = cIdx; i < _playerCardCnt - 1; i++)
                            {
                                _playerCards[i] = _playerCards[i + 1];
                            }
                            _playerCardCnt--;
                            selected = true;
                            break;
                    }

                    if (selected) break;

                    cIdx = Math.Max(Math.Min(cIdx, 3), 0);
                    Utility.gotoxy(10 + cIdx * 14, cy);
                    Console.WriteLine("▲");
                    SFXPlayer.Instance.music = "[SE]Action.mp3";
                    SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
                }
            }

            ShowTable();
            ShowCardInfo();

            Thread.Sleep(1000);

            var rand = new Random(unchecked((int)DateTime.Now.Ticks));
            int randD = rand.Next(0, 4);

            for (int i = randD; i < _dealerCardCnt - 1; i++)
            {
                _dealerCards[i] = _dealerCards[i + 1];
            }
            _dealerCardCnt--;

            ShowTable();
            ShowCardInfo();

            Thread.Sleep(1000);

            _dealerCards[2].isHide = false;

            ShowTable();
            ShowCardInfo();

            Thread.Sleep(1000);
        }

        void Next()
        {
            int betMoney = BetMoney();


            if (betMoney != -1)
            {
                _totalBetMoney += betMoney;
                _seedMoney -= betMoney;

                if (_playerCardCnt == 7)
                {
                    ShowResult();
                    return;
                }

                AddCard(0);
                AddCard(1, _dealerCardCnt == 6);
                ShowTable();
                ShowCardInfo();
                Next();
            }
        }

        void ShowResult()
        {
            for (int i = 0; i < _dealerCardCnt; i++)
            {
                _dealerCards[i].isHide = false;
                _dealerCards[i].Render(14 * i + 5, 3);
            }

            for (int i = 0; i < _playerCardCnt; i++)
            {
                _playerCards[i].Render(14 * i + 5, 15);
            }

            PokerScore dScore = GetScore(1);
            PokerScore pScore = GetScore(0);

            Utility.gotoxy(25, 2);
            Console.WriteLine("                                                         ");
            Utility.gotoxy(25, 2);
            Console.WriteLine("딜러 점수 : {0}", dScore.GetTotalString());

            Utility.gotoxy(25, 14);
            Console.WriteLine("                                                         ");
            Utility.gotoxy(25, 14);
            Console.WriteLine("플레이어 점수 : {0}", pScore.GetTotalString());



            if (CompareScore(pScore, dScore) == true)
            {
                PlayerWin();
            }
            else
            {
                DealerWin();
            }
        }

        void PlayerWin()
        {
            PokerScore dScore = GetScore(1);
            PokerScore pScore = GetScore(0);

            _seedMoney += _totalBetMoney * 2;


            Utility.gotoxy(5, 24);
            Console.WriteLine("Dealer의 패 : {0}", dScore.GetTotalString());
            Utility.gotoxy(5, 25);
            Console.WriteLine("Player의 패 : {0}\n", pScore.GetTotalString());
            Utility.gotoxy(5, 26);
            Console.WriteLine("Player 승리!\n");

            Utility.gotoxy(5, 27);
            Console.WriteLine("{0}만큼의 금액을 얻습니다.", _totalBetMoney);
            Game.Player.ChangeGold(_totalBetMoney);
            Thread.Sleep(5000);
            Exit();
        }

        void DealerWin(bool playerDie = false)
        {
            if (playerDie == true)
            {
                for (int i = 0; i < 4; i++)
                {
                    Utility.gotoxy(5, 24 + i);
                    Console.WriteLine("                                                           ");
                }
                Utility.gotoxy(5, 24);
                Console.Write("Player 다이!!");
                Utility.gotoxy(5, 25);
                Console.Write("Dealer 승리!");
            }
            else
            {
                PokerScore dScore = GetScore(1);
                PokerScore pScore = GetScore(0);

                Utility.gotoxy(5, 24);
                Console.WriteLine("Dealer의 패 : {0}", dScore.GetTotalString());
                Utility.gotoxy(5, 25);
                Console.WriteLine("Player의 패 : {0}\n", pScore.GetTotalString());
                Utility.gotoxy(5, 26);
                Console.WriteLine("Dealer의 승리!");
            }
            Utility.gotoxy(5, 27);
            Console.WriteLine("{0}만큼의 금액을 잃습니다.", _totalBetMoney);
            Game.Player.ChangeGold(-_totalBetMoney);
            Thread.Sleep(3000);
            Exit();
        }


        int BetMoney()
        {
            int betMoney;

            while (true)
            {
                Utility.gotoxy(5, 26);
                Console.WriteLine("                                                          ");
                Utility.gotoxy(5, 24);


                Console.Write("배팅을 해주세요 ({0} ~ {1}, Die는 -1 입력) : ", 0, _seedMoney);
                betMoney = int.Parse(Console.ReadLine());

                if (betMoney == -1)
                {
                    DealerWin(true);
                    return -1;
                }
                betMoney = Math.Min(Math.Max(betMoney, 0), _seedMoney);


                Utility.gotoxy(5, 24);
                Console.WriteLine("                                                                       ");
                Utility.gotoxy(5, 26);
                Console.WriteLine("                                                                       ");
                return betMoney;
            }

        }

        PokerScore GetScore(int playerNumber)
        {
            var cardMap_ShapeToNumber = new Dictionary<Shape, List<Number>>();
            var cardMap_NumberToShape = new Dictionary<Number, List<Shape>>();

            for (Shape i = Shape.Clover; i != Shape.Last; i++)
            {
                cardMap_ShapeToNumber.Add(i, new List<Number>());
            }
            for (Number i = Number.Two; i != Number.Last; i++)
            {
                cardMap_NumberToShape.Add(i, new List<Shape>());
            }

            if (playerNumber == 0)
            {
                for (int i = 0; i < _playerCardCnt; i++)
                {
                    cardMap_ShapeToNumber[_playerCards[i].shape].Add(_playerCards[i].number);
                    cardMap_NumberToShape[_playerCards[i].number].Add(_playerCards[i].shape);
                }
            }
            else if (playerNumber == 1)
            {
                for (int i = 0; i < _dealerCardCnt; i++)
                {
                    cardMap_ShapeToNumber[_dealerCards[i].shape].Add(_dealerCards[i].number);
                    cardMap_NumberToShape[_dealerCards[i].number].Add(_dealerCards[i].shape);
                }
            }

            for (Number i = Number.Two; i != Number.Last; i++)
            {
                cardMap_NumberToShape[i] = cardMap_NumberToShape[i].OrderByDescending(e => e).ToList();
            }

            for (Shape i = Shape.Clover; i != Shape.Last; i++)
            {
                cardMap_ShapeToNumber[i] = cardMap_ShapeToNumber[i].OrderByDescending(e => e).ToList();
            }


            PokerScore rtp = CheckRTP(cardMap_ShapeToNumber);

            if (rtp.score != Score.None)
            {
                return rtp;
            }


            PokerScore btp = CheckBTP(cardMap_ShapeToNumber);

            if (btp.score != Score.None)
            {
                return btp;
            }

            PokerScore stp = CheckSTP(cardMap_ShapeToNumber);

            if (stp.score != Score.None)
            {
                return stp;
            }

            PokerScore four = CheckFour(cardMap_NumberToShape);

            if (four.score != Score.None)
            {
                return four;
            }

            PokerScore fullHouse = CheckFullHouse(cardMap_NumberToShape);

            if (fullHouse.score != Score.None)
            {
                return fullHouse;
            }

            PokerScore flush = CheckFlush(cardMap_ShapeToNumber);

            if (flush.score != Score.None)
            {
                return flush;
            }

            PokerScore mountain = CheckMountain(cardMap_NumberToShape);

            if (mountain.score != Score.None)
            {
                return mountain;
            }


            PokerScore backST = CheckBackST(cardMap_NumberToShape);

            if (backST.score != Score.None)
            {
                return backST;
            }

            PokerScore st = CheckST(cardMap_NumberToShape);

            if (st.score != Score.None)
            {
                return st;
            }



            PokerScore triple = CheckTriple(cardMap_NumberToShape);

            if (triple.score != Score.None)
            {
                return triple;
            }

            PokerScore twoPair = CheckTwoPair(cardMap_NumberToShape);

            if (twoPair.score != Score.None)
            {
                return twoPair;
            }


            PokerScore onePair = CheckOnePair(cardMap_NumberToShape);

            if (onePair.score != Score.None)
            {
                return onePair;
            }

            PokerScore noPair = CheckNoPair(cardMap_NumberToShape);

            return noPair;
        }

        PokerScore CheckRTP(Dictionary<Shape, List<Number>> cardMap)
        {
            for (Shape i = Shape.Spade; i != Shape.None; --i)
            {
                var v = cardMap[i];

                if (v.Contains(Number.Ten)
                    && v.Contains(Number.Jack)
                    && v.Contains(Number.Queen)
                    && v.Contains(Number.King)
                    && v.Contains(Number.Ace))
                {
                    return new PokerScore(Score.RoyalStraightFlush, i, Number.Ace);
                }
            }
            return new PokerScore(PokerScore.Score.None);
        }

        PokerScore CheckBTP(Dictionary<Shape, List<Number>> cardMap)
        {
            for (Shape i = Shape.Spade; i != Shape.None; --i)
            {
                var v = cardMap[i];

                if (v.Contains(Number.Ace)
                    && v.Contains(Number.Two)
                    && v.Contains(Number.Three)
                    && v.Contains(Number.Four)
                    && v.Contains(Number.Five))
                {
                    return new PokerScore(Score.BackStraightFlush, i, Number.Ace);
                }
            }
            return new PokerScore(Score.None);
        }

        PokerScore CheckSTP(Dictionary<Shape, List<Number>> cardMap)
        {
            for (Shape i = Shape.Spade; i != Shape.None; --i)
            {
                var v = cardMap[i];

                for (Number j = Number.Two; (int)j < 9; ++j)
                {
                    if (v.Contains(j)
                        && v.Contains(j + 1)
                        && v.Contains(j + 2)
                        && v.Contains(j + 3)
                        && v.Contains(j + 4))
                    {
                        return new PokerScore(Score.StraightFlush, i, j + 4);
                    }
                }
            }
            return new PokerScore(Score.None);
        }


        PokerScore CheckFour(Dictionary<Number, List<Shape>> cardMap)
        {
            for (Number i = Number.Ace; i != Number.None; --i)
            {
                if (cardMap[i].Count >= 4)
                {
                    return new PokerScore(Score.FourCard, Shape.Spade, i);
                }
            }
            return new PokerScore(Score.None);
        }

        PokerScore CheckFullHouse(Dictionary<Number, List<Shape>> cardMap)
        {
            for (Number i = Number.Ace; i >= Number.Three; --i)
            {
                for (Number j = i - 1; j >= Number.Two; --j)
                {
                    if (cardMap[i].Count >= 3 && cardMap[j].Count >= 2)
                    {
                        return new PokerScore(Score.FullHouse, cardMap[i][0], i, j);
                    }
                }
            }

            for (Number i = Number.Two; i < Number.Ace; ++i)
            {
                for (Number j = i + 1; j < Number.Last; ++j)
                {
                    if (cardMap[i].Count >= 3 && cardMap[j].Count >= 2)
                    {
                        return new PokerScore(Score.FullHouse, cardMap[i][0], i, j);
                    }
                }
            }

            return new PokerScore(Score.None);
        }

        PokerScore CheckFlush(Dictionary<Shape, List<Number>> cardMap)
        {
            for (Shape i = Shape.Spade; i != Shape.None; --i)
            {
                var v = cardMap[i];

                if (v.Count >= 5)
                {
                    return new PokerScore(Score.Flush, i, v[0]);
                }
            }

            return new PokerScore(Score.None);
        }

        PokerScore CheckMountain(Dictionary<Number, List<Shape>> cardMap)
        {
            if (cardMap[Number.Ten].Count > 0 && cardMap[Number.Jack].Count > 0 && cardMap[Number.Queen].Count > 0
                && cardMap[Number.King].Count > 0 && cardMap[Number.Ace].Count > 0)
            {
                return new PokerScore(Score.Mountain, cardMap[Number.Ace][0], Number.Ace);
            }
            return new PokerScore(Score.None);
        }

        PokerScore CheckBackST(Dictionary<Number, List<Shape>> cardMap)
        {
            if (cardMap[Number.Ace].Count > 0 && cardMap[Number.Two].Count > 0 && cardMap[Number.Three].Count > 0
                && cardMap[Number.Four].Count > 0 && cardMap[Number.Five].Count > 0)
            {
                return new PokerScore(Score.BackStraight, cardMap[Number.Ace][0], Number.Ace);
            }
            return new PokerScore(Score.None);
        }

        PokerScore CheckST(Dictionary<Number, List<Shape>> cardMap)
        {
            for (Number i = Number.Ten; i != Number.None; --i)
            {
                if (cardMap[i].Count > 0 && cardMap[i + 1].Count > 0 && cardMap[i + 2].Count > 0
                    && cardMap[i + 3].Count > 0 && cardMap[i + 4].Count > 0)
                {
                    return new PokerScore(Score.Straight, cardMap[i + 4][0], i + 4);
                }
            }
            return new PokerScore(Score.None);
        }

        PokerScore CheckTriple(Dictionary<Number, List<Shape>> cardMap)
        {
            for (Number i = Number.Ace; i != Number.None; --i)
            {
                if (cardMap[i].Count >= 3)
                {
                    return new PokerScore(Score.Triple, cardMap[i][0], i);
                }
            }
            return new PokerScore(Score.None);
        }

        PokerScore CheckTwoPair(Dictionary<Number, List<Shape>> cardMap)
        {
            for (Number i = Number.Ace; i != Number.Two; --i)
            {
                for (Number j = i - 1; j >= Number.Two; --j)
                {
                    if (cardMap[i].Count >= 2 && cardMap[j].Count >= 2)
                    {
                        return new PokerScore(Score.TwoPair, cardMap[i][0], i, j);
                    }
                }
            }
            return new PokerScore(Score.None);
        }
        PokerScore CheckOnePair(Dictionary<Number, List<Shape>> cardMap)
        {
            for (Number i = Number.Ace; i >= Number.Two; --i)
            {
                if (cardMap[i].Count >= 2)
                {
                    return new PokerScore(Score.OnePair, cardMap[i][0], i);
                }
            }
            return new PokerScore(Score.None);
        }

        PokerScore CheckNoPair(Dictionary<Number, List<Shape>> cardMap)
        {
            for (Number i = Number.Ace; i >= Number.Two; --i)
            {
                if (cardMap[i].Count >= 1)
                {
                    return new PokerScore(Score.Top, cardMap[i][0], i);
                }
            }

            return new PokerScore(Score.None);
        }


    }
}

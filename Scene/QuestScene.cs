using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace 김도명_TEXTRPG
{
    class QuestScene : Scene
    {

        private List<LerpObject> characterLerpObjects = new List<LerpObject>();
        public override string Title { get; protected set; } = "";
        public override void EnterScene()
        {
            selectedOptionIndex = 0;
            DrawScene();
            NextScene();
        }

        public override void NextScene()
        {
            while (true)
            {

                Renderer.DrawBorder(Title);


                Renderer.ShowCharacterInfo();

                var playerCurrentQuest = Managers.Game.Data.CurrentQuest;

                if (playerCurrentQuest == null)
                {
                    Options.Clear();
                    Options.Add(new ActionOption("의뢰 받기", "의뢰 받기", () =>
                    {
                        Managers.Game.Data.CurrentQuest = new KillMonsterQuest(GameTable.QuestTable[Managers.Game.Data.LastQuestID] as KillMonsterQuest);
                    }));
                    Renderer.DrawOptionsCenter(Renderer.Height / 2, Options, selectedOptionIndex, Game.lobbyOffsetX);
                }
                else
                {


                    var startY = 6;
                    var endY = 14;

                    var startX = 62;

                    if (playerCurrentQuest is KillMonsterQuest kq)
                    {
                        var getProgress = playerCurrentQuest.CurrentProgress();

                        Renderer.Draw(startX, startY + 2, kq.GetDesc());
                        Renderer.Draw(startX, startY + 4, $"위치 : {playerCurrentQuest.Pos}");
                        Renderer.Draw(startX, startY + 6, $"보상 : 경험치 {playerCurrentQuest.RewardExp} / 골드 {playerCurrentQuest.RewardGold}G");
                    }
                    int length = 35;

                    char color = 'd';


                    for (int j = startY + 1; j <= endY - 1; j++)
                    {
                        Renderer.Draw(startX - 3, j, $"{color}│ w");
                        Renderer.Draw(startX + length - 1, j, $"{color}│ w");
                    }
                    Renderer.Draw(startX - 3, startY, $"{color}┌ w");
                    Renderer.Draw(startX + length - 1, startY, $"{color}┐ w");

                    Renderer.Draw(startX - 3, endY, $"{color}└ w");
                    Renderer.Draw(startX + length - 1, endY, $"{color}┘ w");

                    Renderer.Draw(startX - 1, startY, $"{color}{new string('━', length)}w");
                    Renderer.Draw(startX - 1, endY, $"{color}{new string('━', length)}w");

                    Renderer.Draw(startX + 10, startY, $"진행중인 의뢰");

                    if (playerCurrentQuest.CheckEnd() == true)
                    {
                        Options.Clear();
                        Options.Add(new ActionOption("의뢰 완료", "의뢰 완료", () =>
                        {
                            Renderer.Down("",false);

                            var logStart = 2;

                            Renderer.ConsoleClear();
                            Renderer.DrawBorder();
                            Renderer.DrawCenter(logStart, "y의뢰 완료w");
                            logStart += 2;
                            Thread.Sleep(Managers.Game.GetGameSleepTime(500));

                            var player = Game.Player;

                            Renderer.DrawCenter(logStart, $"경험치 y{playerCurrentQuest.RewardExp}w 획득!");
                            logStart += 2;
                            Thread.Sleep(Managers.Game.GetGameSleepTime(500));

                            Renderer.DrawCenter(logStart, $"골드 y{playerCurrentQuest.RewardGold}Gw 획득!");
                            logStart += 2;
                            Thread.Sleep(Managers.Game.GetGameSleepTime(500));

                            DrawActorInfo();

                            logStart += 16;

                            characterLerpObjects.Add(new LerpObject(player.totalExp, player.totalExp + playerCurrentQuest.RewardExp, Math.Min(playerCurrentQuest.RewardExp, 30), player, 2));
                            SFXPlayer.Instance.music = "[Effect]Healing1_panop.mp3";
                            SFXPlayer.Instance.PlayAsync();
                            Game.Player.ChangeGold(playerCurrentQuest.RewardGold);

                            CheckLerp();

                            var newSkill = player.PlayerSkill.CheckNewSkill(player.Level);

                            if (newSkill.Count > 0)
                            {
                                Renderer.DrawCenter(logStart, "새로운 스킬을 획득했다!");
                                logStart += 2;
                                var skillList = "";
                                foreach (var e in newSkill)
                                {
                                    skillList += $"y{e}w ";
                                }
                                Renderer.DrawCenter(logStart, skillList);
                                logStart += 2;
                            }

                            Thread.Sleep(Managers.Game.GetGameSleepTime(3000));



                            Managers.Game.Data.LastQuestID = playerCurrentQuest.ID;
                            Managers.Game.Data.CurrentQuest = null;
                            Renderer.Down();
                        }));
                        Renderer.DrawOptionsCenter(endY + 2, Options, selectedOptionIndex, Game.lobbyOffsetX);
                    }
                }

                GetInput();
            }
        }


        public void CheckLerp()
        {
            while (characterLerpObjects.Count > 0)
            {
                for (int i = 0; i < characterLerpObjects.Count; i++)
                {
                    var obj = characterLerpObjects[i];
                    if (obj.IsEnd() == true)
                    {
                        characterLerpObjects.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        obj.Lerp();
                    }
                }
                DrawActorInfo();
                Thread.Sleep(Managers.Game.GetGameSleepTime(20));
            }

        }

        public void DrawActorInfo()
        {
            var player = Game.Player;

            var startOffset = 10;

            Renderer.DrawCenter(startOffset, $"이름 : g{Game.Player.Name}w ( y{Game.Player.Job.String()}w )");
            Renderer.DrawCenter(startOffset + 2, $"레벨 : y{Game.Player.Level}w {Renderer.GetBarString(player.totalExp, player.nextLevelExp, 'n', 25)}");
            Renderer.DrawCenter(startOffset + 4, $"경험치 : y{player.totalExp.ToString().PadLeft(3)} / {player.nextLevelExp.ToString().PadLeft(3)}w");
            Renderer.DrawCenter(startOffset + 6, $"체  력 : y{Game.Player.Hp} / {Game.Player.DefaultHpMax}w");
            Renderer.DrawCenter(startOffset + 8, $"마  나 : y{Game.Player.Mp} / {Game.Player.DefaultMpMax}w");
            Renderer.DrawCenter(startOffset + 10, $"공격력 : y{Game.Player.DefaultDamage}w");
            Renderer.DrawCenter(startOffset + 12, $"방어력 : y{Game.Player.DefaultDefense}w");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class LerpObject
    {
        private int from;
        private int dest;
        private int count;
        private int lerpCount;
        private int lerpType;
        public Actor target;
        public LerpObject(int from, int dest, int lerpCount, Actor target, int lerpType)
        {
            count = 0;
            this.from = from;
            this.dest = dest;
            this.lerpCount = lerpCount;
            this.target = target;
            this.lerpType = lerpType;
        }
        public bool IsEnd()
        {
            return count == lerpCount;
        }

        public void Lerp()
        {
            count++;
            if (lerpType == 0)
            {
                target.Hp = (int)Mathf.Lerp(from, dest, (float)count / (float)lerpCount);
            }
            else if (lerpType == 1)
            {
                if (target is Character ch)
                {
                    ch.Mp = (int)Mathf.Lerp(from, dest, (float)count / (float)lerpCount);
                }
            }
            else if (lerpType == 2)
            {
                if (target is Character ch)
                {
                    ch.totalExp = (int)Mathf.Lerp(from, dest, (float)count / (float)lerpCount);
                    if (ch.totalExp >= ch.nextLevelExp)
                    {
                        ch.totalExp -= ch.nextLevelExp;
                        from -= ch.nextLevelExp;
                        dest -= ch.nextLevelExp;
                        ch.LevelUp(1);
                    }
                }
            }
        }
    }
    public class TP
    {
        public float CurrentTP;
        public float MaxTP;
        public float TpSpeed;

        public TP(float tpSpeed)
        {
            TpSpeed = Math.Max(tpSpeed, 0.5f);
            CurrentTP = 0;
            MaxTP = 30.0f;
        }

        public void Progress()
        {
            CurrentTP += TpSpeed;
            CurrentTP = Math.Min(CurrentTP, MaxTP);
        }

        public bool IsMax()
        {
            return MaxTP == CurrentTP;
        }
    }

    class BattleScene : Scene
    {
        private BattleStep step = BattleStep.SelectAction;
        public override string Title { get; protected set; } = "전투";

        private bool isExit = false;
        private const int monsterStartY = 6;
        private const int playerStartY = 19;
        private int selectedSkill;
        private int selectedItem;

        private List<LerpObject> characterLerpObjects = new List<LerpObject>();
        private List<LerpObject> monsterLerpObjects = new List<LerpObject>();
        private bool[] realMonsterDead;

        public static string DungeonName = "";
        public static int Stage = 0;

        private Character player;

        private TP playerTP;
        private List<TP> monsterTP;

        private enum BattleStep
        {
            SelectAction,
            SelectAttack,
            SelectTarget,
            SelectItem,
            Win,
            Defeat,
            Exit
        }

        private List<Monster> monsters;

        public override void EnterScene()
        {
            player = Game.Player;
            selectedOptionIndex = 0;
            step = BattleStep.SelectAction;
            isExit = false;

            monsters = new List<Monster>();

            foreach (var e in GameTable.MonsterGroups[Stage])
            {
                monsters.Add(new Monster(GameTable.Monsters.FirstOrDefault(i => i.ID == e)));
            }

            BGMPlayer.Instance.music = "Encounter.mp3";
            BGMPlayer.Instance.PlayAsync(); // 음악파일명, 볼륨
            ShowStageInfo();

            Thread.Sleep(1000);
            if (Stage % 5 == 4)
            {
                BGMPlayer.Instance.music = "BossBattle.mp3";
            }
            else
            {
                BGMPlayer.Instance.music = "Battle1.mp3";
            }
            BGMPlayer.Instance.PlayAsync(); // 음악파일명, 볼륨

            var maxSpeed = GetMaxSpeed();

            playerTP = new TP((float)player.DefaultSpeed / maxSpeed);
            monsterTP = new List<TP> { new TP((float)monsters[0].DefaultSpeed / maxSpeed), new TP((float)monsters[1].DefaultSpeed / maxSpeed), new TP((float)monsters[2].DefaultSpeed / maxSpeed) };
            realMonsterDead = new bool[3] { false, false, false };


            DrawScene();
            NextScene();
        }

        public float GetMaxSpeed()
        {
            float maxSpeed = (float)player.Speed;
            for (int i = 0; i < monsters.Count; i++)
            {
                maxSpeed = Math.Max((float)monsters[i].DefaultSpeed, maxSpeed);
            }
            return maxSpeed;
        }

        public override void NextScene()
        {
            Renderer.DrawCenter(2, $"{DungeonName}<y{Stage % 5 + 1}스테이지w>");
            while (true)
            {
                RenderSelectTarget(-1);
                ClearLogLine();

                var currentQuest = Managers.Game.Data.CurrentQuest;
                if (currentQuest != null)
                {
                    Renderer.Draw(2, 3, $"진행중인 의뢰 : {currentQuest.GetDesc()}");
                }

                int deadMonsterCount = 0;
                for (int i = 0; i < monsters.Count; i++)
                {
                    if (monsters[i].IsDead() == true)
                    {
                        deadMonsterCount++;
                    }
                }

                if (deadMonsterCount == monsters.Count)
                {
                    step = BattleStep.Win;
                    break;
                }

                if (playerTP.IsMax())
                {
                    selectedOptionIndex = 0;

                    while (step == BattleStep.SelectAction)
                    {
                        DrawStep();
                        GetInput();
                    };

                    if (isExit == true)
                    {
                        break;
                    }

                    while (step == BattleStep.SelectAttack)
                    {
                        while (player.PlayerSkill.MpCost[selectedOptionIndex] > player.Mp)
                        {
                            selectedOptionIndex = (selectedOptionIndex + 1) % player.PlayerSkill.Count;
                        }


                        DrawStep();
                        GetInput();
                    }

                    while (step == BattleStep.SelectItem)
                    {
                        DrawStep();
                        GetInput();
                    }

                    if (step == BattleStep.SelectTarget)
                    {
                        while (monsters[selectedOptionIndex].IsDead() == true)
                        {
                            selectedOptionIndex = (selectedOptionIndex + 1) % monsters.Count;
                        }

                        while (step == BattleStep.SelectTarget)
                        {

                            DrawStep();
                            GetInput();
                        }
                    }

                    CheckLerp();
                    if (lastCommand != Command.Exit)
                    {
                        playerTP.CurrentTP = 0;
                    }
                }
                else
                {
                    int logLine = playerStartY;
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (monsters[i].IsDead() == false)
                        {
                            if (monsterTP[i].IsMax() == true)
                            {
                                monsterTP[i].CurrentTP = 0;
                                DrawActorInfo();
                                DrawAttackLine(i, -1);
                                characterLerpObjects.Add(monsters[i].Attack(player, logLine, 1.0f));
                                ShakePlayer();
                                CheckLerp();
                                logLine += 2;
                                Thread.Sleep(Managers.Game.GetGameSleepTime(1000));
                            }
                        }
                    }

                    if (player.Hp <= 0)
                    {
                        step = BattleStep.Defeat;
                        break;
                    }

                    playerTP.Progress();
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (monsters[i].IsDead() == false)
                        {
                            monsterTP[i].Progress();
                        }
                    }
                    DrawActorInfo();
                    Thread.Sleep(Managers.Game.GetGameSleepTime(30));
                }
            }
            if (step == BattleStep.Defeat)
            {
                Renderer.Down("", true);

                var logStart = 2;
                Renderer.DrawCenter(logStart, $"{DungeonName}<y{Stage % 5 + 1}스테이지w>");
                logStart += 2;
                Renderer.DrawCenter(logStart, "y전투 결과w");
                logStart += 2;
                Thread.Sleep(500);
                Renderer.DrawCenter(logStart, "몬스터 무리에게 패배하였다..");
                logStart += 2;
                Thread.Sleep(500);
                Renderer.DrawCenter(logStart, $"골드를 {(int)(player.Inventory.Gold * 0.9f)}G 만큼 잃었다..");
                logStart += 2;
                Game.Player.ChangeGold(-(int)(player.Inventory.Gold * 0.9f));
                Thread.Sleep(500);
                Renderer.DrawCenter(logStart, $"마을로 돌아갑니다..");
                logStart += 2;
                Thread.Sleep(1000);
                player.Hp = player.HpMax;
                player.Mp = player.MpMax;
                Managers.Scene.EnterScene<MainScene>();

                player.Inventory.Gold -= (int)((float)player.Inventory.Gold * 0.9f);
            }
            else if (step == BattleStep.Win)
            {
                Renderer.Down();
                ShowResult();
                Stage++;
                if (Stage % 5 == 0)
                {
                    Managers.Scene.EnterScene<WorldMapScene>();
                }
                else
                {
                    Renderer.Down();
                    Renderer.DrawBorder("쉼터", 0);
                    BGMPlayer.Instance.music = "Middle.mp3";
                    BGMPlayer.Instance.PlayAsync(); // 음악파일명, 볼륨

                    selectedOptionIndex = 0;
                    do
                    {
                        Renderer.DrawOptionsCenter(12, new List<string> { "1. 다음 스테이지로", "2. 마을로 돌아가기", "3. 저장하기" }, selectedOptionIndex);
                        GetInput();
                    } while (lastCommand != Command.Interact);
                }
            }
            else
            {
                Managers.Scene.EnterScene<WorldMapScene>();
            }

        }

        protected override void DrawScene()
        {
            Renderer.DrawBorder();
        }


        int consumeListCount = 0;

        private void ShowStageInfo()
        {
            Renderer.Down("", true);
            Renderer.DrawCenter(8, $"{DungeonName}<y{Stage % 5 + 1}스테이지w>에 진입했다..");
            Thread.Sleep(500);
            var monsterDict = new Dictionary<string, int>();
            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsterDict.ContainsKey(monsters[i].Name) == false)
                {
                    monsterDict.Add(monsters[i].Name, 1);
                }
                else
                {
                    monsterDict[monsters[i].Name]++;
                }
            }

            int cnt = 0;
            foreach (var e in monsterDict)
            {
                Renderer.DrawCenter(8 + ++cnt * 2, $"{e.Key} {e.Value}마리 등장!!");
            }
            Thread.Sleep(1000);
        }
        private void DrawStep()
        {
            ClearLogLine();

            DrawActorInfo();

            currentLog = playerStartY;

            if (step == BattleStep.SelectAction)
            {
                Renderer.Draw(60, playerStartY - 2, "y선택지 입력w");
                Renderer.DrawOptions(60, playerStartY, new List<string> { "1. 공격", "2. 아이템 사용", "3. 도망가기" }, selectedOptionIndex);
                RenderSelectTarget(-1);
            }
            else if (step == BattleStep.SelectAttack)
            {
                Renderer.Draw(60, playerStartY - 2, "y스킬 선택w");
                var skillStr = new List<string>();
                int cnt = 0;
                foreach (var e in player.PlayerSkill.Names)
                {
                    string skillType = "";

                    switch (player.PlayerSkill.SkillTypes[cnt])
                    {
                        case SkillType.Target:
                            skillType = "단일공격";
                            break;
                        case SkillType.AllTarget:
                            skillType = "전체공격";
                            break;
                        case SkillType.RecoveryMP:
                            skillType = "MP회복";
                            break;
                    }

                    if (player.PlayerSkill.MpCost[cnt] > player.Mp)
                    {
                        skillStr.Add('d' + e + $" (MP : {player.PlayerSkill.MpCost[cnt]}, {skillType})" + 'w');
                    }
                    else
                    {
                        skillStr.Add(e + $" (MP : b{player.PlayerSkill.MpCost[cnt]}w, {skillType})");
                    }
                    cnt++;
                }
                Renderer.DrawOptions(60, playerStartY, skillStr, selectedOptionIndex);
            }
            else if (step == BattleStep.SelectItem)
            {
                Renderer.Draw(60, playerStartY - 2, "y아이템 사용w");
                consumeListCount = 0;
                var itemStr = new List<string>();
                foreach (var e in player.Inventory.Items)
                {
                    if (e.Type == ItemType.ConsumeItem)
                    {
                        itemStr.Add(e.Name + " : " + e.StackCount);
                        consumeListCount++;
                    }
                }

                if (consumeListCount == 0)
                {
                    Renderer.Draw(60, playerStartY, "아이템이 없음");
                }
                else
                {
                    Renderer.DrawOptions(60, playerStartY, itemStr, selectedOptionIndex);
                }
            }
            else if (step == BattleStep.SelectTarget)
            {
                RenderSelectTarget(selectedOptionIndex);
            }

        }

        public void DrawActorInfo()
        {
            RenderSelectTarget(-1);
            var maxPad = Math.Max(player.HpPad, player.MpPad);

            Renderer.Draw(6, playerStartY, $"g{player.Name}w");
            Renderer.Draw(6, playerStartY + 1, $"{Renderer.GetBarString((int)playerTP.CurrentTP, (int)playerTP.MaxTP, 'n', 25)}");
            Renderer.Draw(6, playerStartY + 2, $"HP:{player.Hp.ToString().PadLeft(maxPad)}/{player.HpMax.ToString().PadLeft(maxPad)}");
            Renderer.Draw(6, playerStartY + 3, $"{Renderer.GetBarString(player.Hp, player.DefaultHpMax, 'x')}");
            Renderer.Draw(6, playerStartY + 4, $"MP:{player.Mp.ToString().PadLeft(maxPad)}/{player.MpMax.ToString().PadLeft(maxPad)}");
            Renderer.Draw(6, playerStartY + 5, $"{Renderer.GetBarString(player.Mp, player.DefaultMpMax, 'z')}");

            //Renderer.Draw(6, 29, $"TP:{idx}/{100}{Renderer.GetBarString(idx, 100, 'n')}");

            for (int i = 0; i < monsters.Count; i++)
            {
                var e = monsters[i];

                if (realMonsterDead[i] == true) continue;

                Renderer.Draw(6 + 40 * i, monsterStartY, $"{e.Name}");
                Renderer.Draw(6 + 40 * i, monsterStartY + 1, $"{Renderer.GetBarString((int)monsterTP[i].CurrentTP, (int)monsterTP[i].MaxTP, 'n', 25)}");
                Renderer.Draw(6 + 40 * i, monsterStartY + 2, $"HP:{e.Hp.ToString().PadLeft(e.HpPad)}/{e.DefaultHpMax.ToString().PadLeft(e.HpPad)}");
                Renderer.Draw(6 + 40 * i, monsterStartY + 3, $"{Renderer.GetBarString(e.Hp, e.DefaultHpMax, 'x')}");
            }
        }

        public void DrawResultActorInfo()
        {
            var startOffset = 10;

            Renderer.DrawCenter(startOffset, $"이름 : g{Game.Player.Name}w ( y{Game.Player.Job.String()}w )");
            Renderer.DrawCenter(startOffset + 2, $"레벨 : y{Game.Player.Level}w {Renderer.GetBarString(player.totalExp, player.nextLevelExp, 'n', 25)}");
            Renderer.DrawCenter(startOffset + 4, $"경험치 : y{player.totalExp.ToString().PadLeft(3)} / {player.nextLevelExp.ToString().PadLeft(3)}w");
            Renderer.DrawCenter(startOffset + 6, $"체  력 : y{Game.Player.Hp} / {Game.Player.DefaultHpMax}w");
            Renderer.DrawCenter(startOffset + 8, $"마  나 : y{Game.Player.Mp} / {Game.Player.DefaultMpMax}w");
            Renderer.DrawCenter(startOffset + 10, $"공격력 : y{Game.Player.DefaultDamage}w");
            Renderer.DrawCenter(startOffset + 12, $"방어력 : y{Game.Player.DefaultDefense}w");
        }
        public void ShowResult()
        {
            var logStart = 2;

            Renderer.ConsoleClear();
            Renderer.DrawBorder();
            Renderer.DrawCenter(logStart, $"{DungeonName}<y{Stage % 5 + 1}스테이지w>");
            logStart += 2;
            Renderer.DrawCenter(logStart, "y전투 결과w");
            logStart += 2;
            Thread.Sleep(Managers.Game.GetGameSleepTime(500));

            int totalRewardExp = 0;
            int totalRewardGold = 0;

            foreach (var e in monsters)
            {
                totalRewardExp += e.Exp;
                totalRewardGold += e.Gold;
            }

            Renderer.DrawCenter(logStart, $"경험치 y{totalRewardExp}w 획득!");
            logStart += 2;
            Thread.Sleep(Managers.Game.GetGameSleepTime(500));

            Renderer.DrawCenter(logStart, $"골드 y{totalRewardGold}Gw 획득!");
            logStart += 2;
            Thread.Sleep(Managers.Game.GetGameSleepTime(500));

            DrawResultActorInfo();

            logStart += 14;

            characterLerpObjects.Add(new LerpObject(player.totalExp, player.totalExp + totalRewardExp, Math.Min(totalRewardExp, 30), player, 2));
            SFXPlayer.Instance.music = "[Effect]Healing1_panop.mp3";
            SFXPlayer.Instance.PlayAsync();
            player.Inventory.Gold += totalRewardGold;

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

        }

        private void RenderSelectTarget(int idx)
        {
            char color = 'd';

            for (int i = 0; i < 3; i++)
            {
                if (monsters[i].IsDead() == true) continue;

                if (i == idx)
                {
                    color = 'g';
                    Renderer.Draw(17 + i * 40, monsterStartY + 6, $"{color}↑↑↑ w");
                }
                else
                {
                    color = 'd';
                    Renderer.Draw(17 + i * 40, monsterStartY + 6, $"         ");
                }


                for (int j = monsterStartY - 1; j <= monsterStartY + 5; j++)
                {
                    if (j == monsterStartY - 1)
                    {
                        Renderer.Draw(4 + i * 40, j, $"{color}┌w");
                        Renderer.Draw(32 + i * 40, j, $"{color}┐ w");
                    }
                    else if (j == monsterStartY + 5)
                    {
                        Renderer.Draw(4 + i * 40, j, $"{color}└w");
                        Renderer.Draw(32 + i * 40, j, $"{color}┘ w");
                    }
                    else
                    {
                        Renderer.Draw(4 + i * 40, j, $"{color}│ w");
                        Renderer.Draw(32 + i * 40, j, $"{color}│ w");
                    }
                }

                Renderer.Draw(5 + i * 40, monsterStartY - 1, color + new string('━', 27) + 'w');
                Renderer.Draw(5 + i * 40, monsterStartY + 5, color + new string('━', 27) + 'w');
            }

            color = 'd';

            for (int j = playerStartY - 1; j <= playerStartY + 7; j++)
            {
                if (j == playerStartY - 1)
                {
                    Renderer.Draw(4, j, $"{color}┌w");
                    Renderer.Draw(32, j, $"{color}┐ w");
                }
                else if (j == playerStartY + 7)
                {
                    Renderer.Draw(4, j, $"{color}└w");
                    Renderer.Draw(32, j, $"{color}┘ w");
                }
                else
                {
                    Renderer.Draw(4, j, $"{color}│ w");
                    Renderer.Draw(32, j, $"{color}│ w");
                }
            }

            Renderer.Draw(5, playerStartY - 1, color + new string('━', 27) + 'w');
            Renderer.Draw(5, playerStartY + 7, color + new string('━', 27) + 'w');
        }
        private void ClearLogLine()
        {
            Renderer.ClearLine(55, Renderer.Width - 2, playerStartY - 2, Renderer.Height - 2);
        }

        protected override void OnCommandMoveTop()
        {
            if (step == BattleStep.SelectAction)
            {
                selectedOptionIndex = (selectedOptionIndex - 1 + 3) % 3;
            }
            if (step == BattleStep.SelectAttack)
            {
                selectedOptionIndex = (selectedOptionIndex - 1 + player.PlayerSkill.Names.Count) % player.PlayerSkill.Names.Count;

                while (player.PlayerSkill.MpCost[selectedOptionIndex] > player.Mp)
                {
                    selectedOptionIndex = (selectedOptionIndex - 1 + player.PlayerSkill.Names.Count) % player.PlayerSkill.Names.Count;
                }
            }
            if (step == BattleStep.SelectItem)
            {
                selectedOptionIndex = (selectedOptionIndex - 1 + consumeListCount) % consumeListCount;
            }
            if (step == BattleStep.Win)
            {
                selectedOptionIndex = (selectedOptionIndex - 1 + 3) % 3;
            }
        }
        protected override void OnCommandMoveBottom()
        {
            if (step == BattleStep.SelectAction)
            {
                selectedOptionIndex = (selectedOptionIndex + 1 + 3) % 3;
            }
            if (step == BattleStep.SelectAttack)
            {
                selectedOptionIndex = (selectedOptionIndex + 1 + player.PlayerSkill.Names.Count) % player.PlayerSkill.Names.Count;

                while (player.PlayerSkill.MpCost[selectedOptionIndex] > player.Mp)
                {
                    selectedOptionIndex = (selectedOptionIndex + 1 + player.PlayerSkill.Names.Count) % player.PlayerSkill.Names.Count;
                }
            }
            if (step == BattleStep.SelectItem)
            {
                selectedOptionIndex = (selectedOptionIndex + 1 + consumeListCount) % consumeListCount;
            }
            if (step == BattleStep.Win)
            {
                selectedOptionIndex = (selectedOptionIndex + 1 + 3) % 3;
            }
        }

        protected override void OnCommandMoveLeft()
        {
            if (step == BattleStep.SelectTarget)
            {
                selectedOptionIndex = (selectedOptionIndex - 1 + monsters.Count) % monsters.Count;
                while (monsters[selectedOptionIndex].IsDead() == true)
                {
                    selectedOptionIndex = (selectedOptionIndex - 1 + monsters.Count) % monsters.Count;
                }
            }
        }

        protected override void OnCommandMoveRight()
        {
            if (step == BattleStep.SelectTarget)
            {
                selectedOptionIndex = (selectedOptionIndex + 1) % monsters.Count;
                while (monsters[selectedOptionIndex].IsDead() == true)
                {
                    selectedOptionIndex = (selectedOptionIndex + 1) % monsters.Count;
                }
            }
        }


        int currentLog = 0;

        protected override void OnCommandInteract()
        {
            if (step == BattleStep.SelectAction)
            {
                if (selectedOptionIndex == 0)
                {
                    step = BattleStep.SelectAttack;
                }

                else if (selectedOptionIndex == 1)
                {
                    step = BattleStep.SelectItem;
                }
                else if (selectedOptionIndex == 2)
                {
                    step = BattleStep.Exit;
                    isExit = true;
                    Renderer.ConsoleClear();
                }

                selectedOptionIndex = 0;
            }
            else if (step == BattleStep.SelectAttack)
            {
                selectedSkill = selectedOptionIndex;

                ClearLogLine();

                if (player.PlayerSkill.SkillTypes[selectedSkill] != SkillType.Target)
                {
                    Renderer.ShowText(60, currentLog, $"g{player.Name}w은 y{player.PlayerSkill.Names[selectedSkill]}w을 시전하였다!!");
                    currentLog += 2;
                }

                if (player.PlayerSkill.SkillTypes[selectedSkill] == SkillType.AllTarget)
                {
                    ActiveAllTargetSkill();
                    step = BattleStep.SelectAction;
                }
                else if (player.PlayerSkill.SkillTypes[selectedSkill] == SkillType.RecoveryMP)
                {
                    Thread.Sleep(Managers.Game.GetGameSleepTime(500));
                    Renderer.ShowText(60, currentLog, $"g{player.Name}w은 b{(int)player.PlayerSkill.DamageRate[selectedSkill]}w만큼 MP를 회복하였다.");
                    characterLerpObjects.Add(new LerpObject(player.Mp, Math.Min(player.Mp + (int)player.PlayerSkill.DamageRate[selectedSkill], player.MpMax), 15, player, 1));
                    Thread.Sleep(Managers.Game.GetGameSleepTime(1000));
                    SFXPlayer.Instance.music = "[Effect]Healing1_panop.mp3";
                    SFXPlayer.Instance.PlayAsync();
                    step = BattleStep.SelectAction;
                }
                else if (player.PlayerSkill.SkillTypes[selectedSkill] == SkillType.Target)
                {
                    step = BattleStep.SelectTarget;
                }
            }
            else if (step == BattleStep.SelectItem)
            {
                ClearLogLine();

                selectedItem = selectedOptionIndex;

                int cnt = 0;
                foreach (var e in player.Inventory.Items)
                {
                    if (e.Type == ItemType.ConsumeItem)
                    {
                        if (selectedItem == cnt)
                        {
                            var con = e as ConsumeItem;

                            characterLerpObjects.Add(con.UseEffect(player));

                            break;
                        }
                        cnt++;
                    }
                }
                step = BattleStep.SelectAction;
            }
            else if (step == BattleStep.SelectTarget)
            {
                Renderer.ShowText(60, currentLog, $"g{player.Name}w은 y{player.PlayerSkill.Names[selectedSkill]}w을 시전하였다!!");

                characterLerpObjects.Add(new LerpObject(player.Mp, player.Mp - player.PlayerSkill.MpCost[selectedSkill], 15, player, 1));

                RenderSelectTarget(-1);

                CheckLerp();

                for (int i = 0; i < player.PlayerSkill.AttackCount[selectedSkill]; i++)
                {
                    if (realMonsterDead[selectedOptionIndex] == true)
                    {
                        break;
                    }
                    currentLog += 2;

                    DrawAttackLine(selectedOptionIndex, 1, 50);

                    monsterLerpObjects.Add(Game.Player.Attack(monsters[selectedOptionIndex], currentLog, player.PlayerSkill.DamageRate[selectedSkill]));
                    ShakeMonster(selectedOptionIndex);
                    CheckLerp();
                }

                step = BattleStep.SelectAction;
            }
            else if (step == BattleStep.Win)
            {
                if (selectedOptionIndex == 0)
                {
                    EnterScene();
                }
                else if (selectedOptionIndex == 1)
                {
                    Managers.Scene.EnterScene<MainScene>();
                }
                else if (selectedOptionIndex == 2)
                {
                    Managers.Game.Data.CurrentSceneName = "Battle";
                    Managers.Game.Data.SceneMetaData = (int)Stage;
                    Managers.Scene.EnterScene<SaveScene>();
                }
            }
        }

        protected override void OnCommandExit()
        {
            if (step == BattleStep.SelectAttack || step == BattleStep.SelectItem)
            {
                step = BattleStep.SelectAction;
            }

            if (step == BattleStep.SelectTarget)
            {
                step = BattleStep.SelectAttack;
                RenderSelectTarget(-1);
                selectedOptionIndex = 0;
            }
        }

        public void ActiveAllTargetSkill()
        {
            characterLerpObjects.Add(new LerpObject(player.Mp, player.Mp - player.PlayerSkill.MpCost[selectedSkill], 15, player, 1));

            RenderSelectTarget(-1);

            CheckLerp();

            DrawAttackLine(0, 2);

            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].IsDead() == true) continue;

                monsterLerpObjects.Add(Game.Player.Attack(monsters[i], currentLog, player.PlayerSkill.DamageRate[selectedSkill]));
                currentLog += 2;
                ShakeMonster(i);
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
                if (step != BattleStep.Win)
                {
                    DrawActorInfo();
                }
                else
                {
                    DrawResultActorInfo();
                }
                Thread.Sleep(Managers.Game.GetGameSleepTime(20));
            }

            while (monsterLerpObjects.Count > 0)
            {
                for (int i = 0; i < monsterLerpObjects.Count; i++)
                {
                    var obj = monsterLerpObjects[i];

                    if (obj.target.IsDead() == true)
                    {
                        for (int j = 0; j < monsters.Count; j++)
                        {
                            if (obj.target == monsters[j])
                            {
                                DrawActorInfo();
                                DeleteMonster(j);
                                break;
                            }
                        }
                    }

                    if (obj.IsEnd() == true)
                    {
                        monsterLerpObjects.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        obj.Lerp();
                    }
                }
                if (step != BattleStep.Win)
                {
                    DrawActorInfo();
                }
                else
                {
                    DrawResultActorInfo();
                }
                Thread.Sleep(Managers.Game.GetGameSleepTime(20));
            }
        }

        public void DrawAttackLine(int number, int direction, int sleep = 50)
        {
            if (direction == -1)
            {
                int cnt = -1;

                switch (number)
                {
                    case 0:
                        // 1번라인
                        for (int i = 11; i <= 24; i++)
                        {
                            if (i > 17) Renderer.Draw(17, i - 6, " ");

                            if (i <= 18 && i >= 12)
                            {
                                Renderer.Draw(17, i, "y☆ w");
                            }
                            Thread.Sleep(Managers.Game.GetGameSleepTime(sleep));
                        }
                        //Renderer.Draw(17, 10, " ");
                        break;
                    case 1:
                        for (int i = 11; i <= 24; i++)
                        {
                            if (i > 17) Renderer.Draw(57 - (cnt - 6) * 6, i - 6, " ");

                            if (i <= 18 && i >= 12)
                            {
                                Renderer.Draw(57 - cnt * 6, i, "y☆ w");
                            }
                            cnt++;
                            Thread.Sleep(Managers.Game.GetGameSleepTime(sleep));
                        }
                        //Renderer.Draw(57 - cnt * 6, 10, " ");
                        break;
                    case 2:
                        for (int i = 11; i <= 24; i++)
                        {
                            if (i > 17) Renderer.Draw(97 - (cnt - 6) * 13, i - 6, " ");

                            if (i <= 18 && i >= 12)
                            {
                                Renderer.Draw(97 - cnt * 13, i, "y☆ w");
                            }
                            cnt++;
                            Thread.Sleep(Managers.Game.GetGameSleepTime(sleep));
                        }
                        break;
                }
            }
            else if (direction == 1)
            {
                int cnt = 0;

                switch (number)
                {
                    case 0:
                        // 1번라인
                        for (int i = 17; i >= 5; i--)
                        {
                            if (i < 12) Renderer.Draw(17, i + 6, " ");

                            if (i >= 11)
                            {
                                Renderer.Draw(17, i, "y☆ w");
                            }
                            Thread.Sleep(Managers.Game.GetGameSleepTime(sleep));
                        }
                        // Renderer.Draw(17, 11, " ");
                        break;
                    case 1:
                        for (int i = 17; i >= 5; i--)
                        {
                            if (i < 12) Renderer.Draw(21 + (cnt - 6) * 6, i + 6, " ");

                            if (i >= 11)
                            {
                                Renderer.Draw(21 + cnt * 6, i, "y☆ w");
                            }
                            cnt++;
                            Thread.Sleep(Managers.Game.GetGameSleepTime(sleep));
                        }
                        //Renderer.Draw(15 + (cnt - 1) * 6, 11, " ");
                        break;
                    case 2:
                        for (int i = 17; i >= 5; i--)
                        {
                            if (i < 12) Renderer.Draw(19 + (cnt - 6) * 13, i + 6, " ");

                            if (i >= 11)
                            {
                                Renderer.Draw(19 + cnt * 13, i, "y☆ w");
                            }
                            cnt++;
                            Thread.Sleep(Managers.Game.GetGameSleepTime(sleep));
                        }
                        //Renderer.Draw(19 + (cnt - 1) * 13, 11, " ");
                        break;
                }
            }
            else if (direction == 2)
            {
                int cnt = 0;

                for (int i = 17; i >= 6; i--)
                {
                    if (monsters[0].IsDead() == false)
                    {
                        if (i < 12) Renderer.Draw(17, i + 6, " ");

                        if (i >= 11)
                        {
                            Renderer.Draw(17, i, "y☆ w");
                        }
                    }
                    if (monsters[1].IsDead() == false)
                    {
                        if (i < 12) Renderer.Draw(21 + (cnt - 6) * 6, i + 6, " ");

                        if (i >= 11)
                        {
                            Renderer.Draw(21 + cnt * 6, i, "y☆ w");
                        }
                    }
                    if (monsters[2].IsDead() == false)
                    {
                        if (i < 12) Renderer.Draw(19 + (cnt - 6) * 13, i + 6, " ");

                        if (i >= 11)
                        {
                            Renderer.Draw(19 + cnt * 13, i, "y☆ w");
                        }
                    }
                    cnt++;
                    Thread.Sleep(Managers.Game.GetGameSleepTime(sleep));
                }
            }
        }

        public void DeleteMonster(int i)
        {
            if (realMonsterDead[i]) return;

            Managers.Game.OnKillMonster?.Invoke(monsters[i].ID);
            realMonsterDead[i] = true;
            SFXPlayer.Instance.music = "[Effect]Knockdown2_panop.mp3";
            SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
            Thread.Sleep(Managers.Game.GetGameSleepTime(200));
            Renderer.DeleteVertical(4 + 40 * i, Math.Min(6 + 40 * (i + 1) - 7, Renderer.Width - 6), monsterStartY - 1, monsterStartY + 5);
            Thread.Sleep(Managers.Game.GetGameSleepTime(500));
        }

        public void ShakeMonster(int i)
        {
            Renderer.ShakeHorizontal(4 + 40 * i, Math.Min(6 + 40 * (i + 1) - 7, Renderer.Width - 6), monsterStartY - 1, monsterStartY + 5);
        }
        public void ShakePlayer()
        {
            Renderer.ShakeHorizontal(4, Math.Min(42, Renderer.Width - 6), playerStartY - 1, playerStartY + 7);
        }
    }
}

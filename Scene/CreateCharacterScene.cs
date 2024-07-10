using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    class CreateCharacterScene : Scene
    {
        public override string Title { get; protected set; } = "캐릭터 생성";

        private enum CreateStep
        {
            Name,
            Job,
            CreateCharacter,
            Exit
        }

        private Character selectPlayer;
        private CreateStep step = CreateStep.Name;
        private string createName = string.Empty;
        private string errorMessage = string.Empty;

        #region Scene

        public override void EnterScene()
        {
            selectedOptionIndex = 0;
            step = CreateStep.Name;
            //formatters = Managers.Table.GetFormatters<Character>(new string[] { "Job", "Damage", "Defense", "HpMax", "MpMax" });
            DrawScene();
        }

        public override void NextScene()
        {
            while (step == CreateStep.Name)
            {
                DrawStep();
                ReadName();
            }
            while (step == CreateStep.Job)
            {
                DrawStep();
                GetInput();
            }
        }
        protected override void DrawScene()
        {
            Renderer.DrawBorder(Title, 0);
        }

        private void DrawStep()
        {
            switch (step)
            {
                case CreateStep.Name:
                    Renderer.DrawCenter(10, "당신의 이름을 입력해주세요.");
                    Renderer.DrawCenter(13, errorMessage);
                    break;
                case CreateStep.Job:
                    Renderer.DrawBorder("직업 선택", 0);
                    DrawSelectCharacter();
                    break;
            }
        }

        #endregion

        #region Step

        private void NextStep()
        {
            errorMessage = string.Empty;
            step += 1;
            if (step == CreateStep.CreateCharacter)
            {
                CreateCharacter();
            }
            else if (step == CreateStep.Exit)
            {
                Managers.Scene.EnterScene<MainScene>();
            }
        }

        #endregion

        #region Input

        protected override void OnCommandMoveTop()
        {
            if (step == CreateStep.Job)
            {
                selectedOptionIndex = (selectedOptionIndex - 1 + GameTable.Characters.Length) % GameTable.Characters.Length;
            }
        }
        protected override void OnCommandMoveBottom()
        {
            if (step == CreateStep.Job)
            {
                selectedOptionIndex = (selectedOptionIndex + 1 + GameTable.Characters.Length) % GameTable.Characters.Length;
            }
        }
        protected override void OnCommandInteract()
        {
            if (step == CreateStep.Job)
            {
                ReadJob();
            }
        }
        protected override void OnCommandExit()
        {

        }

        #endregion

        #region Read

        /// <summary>
        /// 이름 입력
        /// </summary>
        private void ReadName()
        {
            Renderer.StopRenderThread();

            Console.CursorVisible = true;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 3, 12);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            var name = Console.ReadLine();
            OnNameChanged(name);

            SFXPlayer.Instance.music = "[SE]Enter.mp3";
            SFXPlayer.Instance.PlayAsync(0.5f); // 음악파일명, 볼륨

            Console.CursorVisible = false;
            Renderer.RestartRenderThread();
        }

        /// <summary>
        /// 직업 선택지 입력
        /// </summary>
        private void ReadJob()
        {
            if (selectedOptionIndex == 1)
            {
                selectPlayer = GameTable.Characters[selectedOptionIndex];
                NextStep();
            }
        }

        #endregion

        #region OnValueChanged

        /// <summary>
        /// 올바른 이름을 입력했는지 체크하고, 이름을 변경합니다.
        /// </summary>
        /// <param name="name">입력한 이름</param>
        public void OnNameChanged(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                errorMessage = "오류: 이름을 입력해 주세요.";
                return;
            }

            if (Renderer.GetPrintingLength(name) > 10)
            {
                errorMessage = "오류: 이름이 너무 깁니다. 10글자 이내로 작성해 주세요.";
                return;
            }

            // 이름 결정
            createName = name;
            NextStep();
        }

        /// <summary>
        /// 올바른 직업을 선택했는지 체크하고, 직업을 선택합니다.
        /// </summary>
        /// <param name="job">선택한 직업 인덱스</param>
        public void OnJobChanged(string job)
        {
            if (string.IsNullOrEmpty(job))
            {
                errorMessage = "오류: 숫자를 입력해 주세요.";
                return;
            }

            bool isInt = Int32.TryParse(job, out int idx);

            if (!isInt)
            {
                errorMessage = "오류: 숫자가 아닌 문자를 입력했습니다. 다시 입력해 주세요.";
                return;
            }

            if (idx < 0 || idx > GameTable.Characters.Length)
            {
                errorMessage = "오류: 선택지 범위 내의 숫자를 입력해 주세요.";
                return;
            }

            // 직업 결정
            //selectPlayer = Game.Characters[idx];
            NextStep();
        }

        #endregion

        public void DrawSelectCharacter()
        {
            //Renderer.DrawKeyGuide("[방향키 ↑ ↓: 선택지 이동] [Enter: 결정]");
            Renderer.DrawCenter(2, "<직업 목록>");

            var characters = GameTable.Characters;

            List<string> jobSelectStr = new List<string>();
            string jobName = "";
            string jobDesc = "";
            string jobBaseHp = "";
            string jobBaseMp = "";
            string jobBaseAtk = "";
            string jobBaseDef = "";
            string jobBaseSpeed = "";



            for (int i = 0; i < characters.Count(); i++)
            {
                if(i!= 1)
                {
                    jobSelectStr.Add(characters[i].Job.String() + "(미구현)");
                }
                else
                {
                    jobSelectStr.Add(characters[i].Job.String());
                }
                if (i == selectedOptionIndex)
                {
                    jobName = $"직업 : y{characters[i].Job.String()}w";
                    switch (i)
                    {
                        case 0:
                            jobName += "(미구현)";
                            jobDesc = "특징 : y높은 체력과 방어력을 바탕으로 적과 근거리에서 싸우는 탱커 역할을 담당합니다.w";
                            break;
                        case 1:
                            jobDesc = "특징 : y다양한 원거리 마법을 사용해 강력한 공격을 퍼부으며, 광역 공격에 능합니다.w";
                            break;
                        case 2:
                            jobName += "(미구현)";
                            jobDesc = "특징 : y은신과 빠른 기동성을 이용해 적을 기습하고 치명적인 공격을 가합니다.w";
                            break;
                        case 3:
                            jobName += "(미구현)";
                            jobDesc = "특징 : y활과 화살을 사용해 먼 거리에서 적을 공격하며, 높은 명중률을 자랑합니다.w";
                            break;
                    }
                    jobBaseHp = $"HP : y{characters[i].Hp}w";
                    jobBaseMp = $"MP : y{characters[i].Mp}w";
                    jobBaseAtk = $"공격력 : y{characters[i].Damage}w";
                    jobBaseDef = $"방어력 : y{characters[i].DefaultDefense}w";
                    jobBaseSpeed = $"순발력 : y{characters[i].DefaultSpeed}w";
                }
            }


            var startY = 12;

            Renderer.DrawOptionsCenter(4, jobSelectStr, selectedOptionIndex);
            Renderer.Draw(16, startY, jobName);
            Renderer.Draw(16, startY + 2, jobDesc);
            Renderer.Draw(16, startY + 4, "<기초 스텟>");
            Renderer.Draw(16, startY + 6, jobBaseHp);
            Renderer.Draw(16, startY + 8, jobBaseMp);
            Renderer.Draw(16, startY + 10, jobBaseAtk);
            Renderer.Draw(16, startY + 12, jobBaseDef);
            Renderer.Draw(16, startY + 14, jobBaseSpeed);

        }

        /// <summary>
        /// 완성된 캐릭터 생성
        /// </summary>
        private void CreateCharacter()
        {
            Game.Player = new Character
            (
                createName,
                selectPlayer.Job,
                selectPlayer.Level,
                (int)selectPlayer.DefaultDamage,
                (int)selectPlayer.DefaultDefense,
                (int)selectPlayer.DefaultSpeed,
                (int)selectPlayer.DefaultHpMax,
                (int)selectPlayer.DefaultMpMax,
                selectPlayer.PlayerSkill
            );

            //게임 클래스에 저장된 아이템 등록

            for(int i=0; i<6; i+=5)
            {
                if (GameTable.Items[i].Type == ItemType.Gear)
                {
                    Game.Player.Inventory.Add(new Gear(GameTable.Items[i] as Gear));
                }
                else
                {
                    Game.Player.Inventory.Add(new ConsumeItem(GameTable.Items[i] as ConsumeItem));
                }
            }
            //Game.Player.Inventory.Add(GameTable.Items[0]);
            //Game.Player.Inventory.Add(GameTable.Items[5]);

            //Gear basicWeapon = Game.Items[0].DeepCopy() as Gear;
            //Gear basicTop = Game.Items[1].DeepCopy() as Gear;
            //Game.Player.Inventory.Add(basicWeapon);
            //Game.Player.Inventory.Add(basicTop);
            //Game.Player.Equipment.Equip((GearSlot)basicWeapon.GearType, basicWeapon);
            //Game.Player.Equipment.Equip((GearSlot)basicTop.GearType, basicTop);

            Managers.Game.Data.character = Game.Player;
            Managers.Game.Data.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //Managers.Game.data.stage = Game.Stage;
            //Managers.Game.SaveGame();


            NextStep();
        }

    }
}

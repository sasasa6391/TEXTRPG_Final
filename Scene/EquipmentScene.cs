using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class EquipmentScene : Scene
    {
        public override string Title { get; protected set; } = "인벤토리";

        // 장비 씬에서 단계 구분
        private enum EquipStep
        {
            Show,       // 기본 모드
            Equipment,  // 관리 모드
        }

        private EquipStep step;
        private List<Item> gearList = new List<Item>();
        private List<Item> consumeList = new List<Item>();

        #region Scene

        private int selectedItemType = 0;
        private int selectedItemIndex = 0;
        private int gearCount = 0;

        public override void EnterScene()
        {
            // #1. 씬 설정.
            step = EquipStep.Show;
            selectedOptionIndex = 0;

            // #2. 선택지 설정.
            Options.Clear();
            Options.Add(Managers.Scene.GetOption("Back"));

            // #3. 테이블 및 아이템 설정.
            gearList = Game.Player.Inventory.Items.Where(item => item.Type == ItemType.Gear).ToList();  // 장비 아이템만 따로 리스트 복제

            Renderer.DrawBorder(Title);
        }

        public override void NextScene()
        {
            do
            {
                DrawStep();
                GetInput();
            }
            while (Managers.Scene.CurrentScene is EquipmentScene);
        }

        private void DrawStep()
        {
            const int startX = 15;
            const int startY = 8;

            Renderer.ClearLine(startX, Renderer.Width - 3, startY, startY + 10);

            int diffAtk = 0;
            int diffDef = 0;
            int diffSpeed = 0;

            Renderer.DrawCenter(startY, "y착용 장비w");


            var actionList = new List<string>();

            actionList.Add($"y무기w - {Game.Player.Equipment.Equipped[GearType.Weapon].Name}");
            actionList.Add($"y모자w - {Game.Player.Equipment.Equipped[GearType.Hat].Name}");
            actionList.Add($"y갑옷w - {Game.Player.Equipment.Equipped[GearType.Armor].Name}");
            actionList.Add($"y신발w - {Game.Player.Equipment.Equipped[GearType.Shoes].Name}");
            actionList.Add($"y장갑w - {Game.Player.Equipment.Equipped[GearType.Gloves].Name}");
            actionList.Add($"y소비 아이템w");

            Renderer.DrawOptionsCenter(startY + 2, actionList, step == EquipStep.Show ? selectedItemType : -1, 0);

            if (selectedItemType < 5)
            {
                var gearType = (GearType)selectedItemType;
                var gearTitle = gearType.String();

                gearCount = 0;

                var actionList2 = new List<string>();


                Renderer.DrawCenter(startY, $"y{gearTitle}w", 30);

                var addedAtk = 0;
                var addedDef = 0;
                var addedSpeed = 0;
                for (int i = 0; i < gearList.Count; i++)
                {
                    if (gearList[i] is Gear g)
                    {
                        if (g.GearType == gearType)
                        {
                            if (g.IsEquip == true)
                            {
                                addedAtk = g.Atk;
                                addedDef = g.Def;
                                addedSpeed = g.Speed;
                            }
                        }
                    }
                }
                for (int i = 0; i < gearList.Count; i++)
                {
                    if (gearList[i] is Gear g)
                    {
                        if (g.GearType == gearType)
                        {
                            if (g.IsEquip == true)
                            {
                                actionList2.Add("g[E]w" + gearList[i].Name + " : " + gearList[i].StackCount);
                            }
                            else
                            {
                                actionList2.Add(gearList[i].Name + " : " + gearList[i].StackCount);
                                if (step == EquipStep.Equipment && gearCount == selectedItemIndex)
                                {
                                    diffAtk = g.Atk - addedAtk;
                                    diffDef = g.Def - addedDef;
                                    diffSpeed = g.Speed - addedSpeed;
                                }
                            }
                            gearCount++;
                        }
                    }
                }

                Renderer.DrawOptionsCenter(startY + 2, actionList2, step == EquipStep.Equipment ? selectedItemIndex : -1, 30);
            }
            else
            {
                var actionList2 = new List<string>();

                Renderer.DrawCenter(startY, $"y소비 아이템w", 30);

                consumeList = Game.Player.Inventory.Items.Where(item => item.Type == ItemType.ConsumeItem).ToList();  // 소비 아이템만 따로 리스트 복제

                for (int i = 0; i < consumeList.Count; i++)
                {
                    actionList2.Add(consumeList[i].Name + " : " + consumeList[i].StackCount);
                }

                Renderer.DrawOptionsCenter(startY + 2, actionList2, step == EquipStep.Equipment ? selectedItemIndex : -1, 30);
            }


            Renderer.ShowCharacterInfo(diffAtk, diffDef, diffSpeed);

            if (step == EquipStep.Show)
            {
                int row = 4;
                //Renderer.DrawKeyGuide("[Enter : ] [ESC : 뒤로가기]");
            }
            else
            {
                int row = 4;

            }
        }

        #endregion

        #region Input

        protected override void OnCommandMoveTop()
        {
            if (step == EquipStep.Show)
            {
                selectedItemType = (selectedItemType - 1 + 6) % 6;
                gearCount = 0;
                selectedItemIndex = 0;
            }
            else if (step == EquipStep.Equipment)
            {
                if (gearCount == 0) return;

                if (selectedItemType < 5)
                {
                    selectedItemIndex = (selectedItemIndex - 1 + gearCount) % gearCount;
                }
                else
                {
                    selectedItemIndex = (selectedItemIndex - 1 + consumeList.Count) % consumeList.Count;
                }
            }
        }
        protected override void OnCommandMoveBottom()
        {
            if (step == EquipStep.Show)
            {
                selectedItemType = (selectedItemType + 1 + 6) % 6;
                gearCount = 0;
                selectedItemIndex = 0;
            }
            else if (step == EquipStep.Equipment)
            {
                if (gearCount == 0) return;

                if (selectedItemType < 5)
                {
                    selectedItemIndex = (selectedItemIndex + 1 + gearCount) % gearCount;
                }
                else
                {
                    selectedItemIndex = (selectedItemIndex + 1 + consumeList.Count) % consumeList.Count;
                }
            }

        }
        protected override void OnCommandInteract()
        {
            if (step == EquipStep.Show) step = EquipStep.Equipment;
            else if (step == EquipStep.Equipment)
            {
                if (selectedItemType < 5)
                {
                    EquipFromInventory();
                }
            }
        }
        protected override void OnCommandExit()
        {
            if (step == EquipStep.Show) Options[0].Execute();
            else if (step == EquipStep.Equipment) step = EquipStep.Show;
        }

        #endregion

        #region Equipment

        /// <summary>
        /// 인벤토리에서 아이템 장착
        /// </summary>
        private void EquipFromInventory()
        {
            int cnt = 0;
            for (int i = 0; i < gearList.Count; i++)
            {
                if (gearList[i] is Gear g)
                {
                    if ((int)g.GearType == selectedItemType)
                    {
                        if (cnt == selectedItemIndex)
                        {
                            Game.Player.Equipment.Equip(g.GearType, g);
                            break;
                        }
                        cnt++;
                    }
                }
            }

        }

        #endregion
    }
}

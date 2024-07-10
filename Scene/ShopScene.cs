using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class ShopScene : Scene
    {
        public override string Title { get; protected set; } = "상 점";

        private List<Item> sellItemList = new List<Item>();

        private List<ActionOption> buyItemActions = new List<ActionOption>();

        private GearType showGearType;
        private enum ShopStep
        {
            Main, // 기본 모드
            Buy,  // 구매 모드
        }
        private ShopStep step;

        #region Scene

        public override void EnterScene()
        {
            selectedOptionIndex = 0;

            // #2. 선택지 설정.
            Options.Clear();
            for (GearType i = GearType.Weapon; i != GearType.None; i++)
            {
                GearType currentGear = i;

                Options.Add(new ActionOption(i.String(), i.String(), () =>
                {
                    sellItemList = GameTable.Items.Where(e => e.Type == ItemType.Gear).Where(e =>
                    {
                        var g = e as Gear;
                        return g.GearType == currentGear;
                    }).ToList();
                    step = ShopStep.Buy;
                    showGearType = currentGear;
                    selectedOptionIndex = 0;

                }));
            }


            Options.Add(new ActionOption("소비아이템", "소비아이템", () =>
            {
                sellItemList = GameTable.Items.Where(e => e.Type == ItemType.ConsumeItem).ToList();
                step = ShopStep.Buy;
                showGearType = GearType.None;
                selectedOptionIndex = 0;
            }));


        }

        public override void NextScene()
        {
            do
            {
                DrawStep();
                GetInput();
            }
            while (lastCommand != Command.Exit);
        }

        protected override void OnCommandExit()
        {
            if (step == ShopStep.Buy)
            {
                step = ShopStep.Main;
                lastCommand = Command.Nothing;
                selectedOptionIndex = 0;
            }
            else
            {
                Managers.Scene.GetOption("Back").Execute();
            }
        }
        private void DrawStep()
        {
            var diffAtk = 0;
            var diffDef = 0;
            var diffSpeed = 0;

            if (step == ShopStep.Main)
            {
                Renderer.ConsoleClear();
                Renderer.DrawBorder(Title);
                Renderer.DrawOptionsCenter(8, Options, selectedOptionIndex, Game.lobbyOffsetX);
            }
            else if (step == ShopStep.Buy)
            {
                Renderer.ConsoleClear();
                Renderer.DrawBorder(Title);
                buyItemActions.Clear();

                var currentG = new Gear();

                if (showGearType < GearType.None)
                {
                    currentG = Game.Player.Equipment.Equipped[showGearType];
                    Renderer.DrawCenter(5, $"장착중인 아이템 : y{currentG.Name}w", Game.lobbyOffsetX);
                    
                }

                foreach (var e in sellItemList)
                {
                    var haveItem = Game.Player.Inventory.Items.Where(i => i.ID == e.ID).ToList();

                    var actionStr = "";
                    var action = new Action(() => { });

                    if (Game.Player.Inventory.Gold >= e.Price)
                    {
                        actionStr = $"{e.Name} : {e.Price} G";
                        action = new Action(() =>
                        {
                            Game.Player.ChangeGold(-e.Price);
                            if (e is Gear g)
                            {
                                Game.Player.Inventory.Add(new Gear(g));
                            }
                            if (e is HealingPotion h)
                            {
                                Game.Player.Inventory.Add(new HealingPotion(h));
                            }
                            if(e is ManaPotion m)
                            {
                                Game.Player.Inventory.Add(new ManaPotion(m));
                            }
                        });
                    }
                    else
                    {
                        actionStr = $"d{e.Name} : {e.Price} G";
                    }

                    if (haveItem.Count > 0)
                    {
                        actionStr += $"(현재 보유량 : {haveItem[0].StackCount})";
                    }
                    else
                    {
                        actionStr += $"(현재 보유량 : {0})";
                    }

                    buyItemActions.Add(new ActionOption(e.Name, actionStr, action));
                }
                Renderer.DrawOptionsCenter(8, buyItemActions, selectedOptionIndex, Game.lobbyOffsetX);

                if (showGearType < GearType.None)
                {
                    var g = sellItemList[selectedOptionIndex] as Gear;
                    diffAtk = g.Atk - currentG.Atk;
                    diffDef = g.Def - currentG.Def;
                    diffSpeed = g.Speed - currentG.Speed;
                }
            }
            Renderer.ShowCharacterInfo(diffAtk, diffDef, diffSpeed);
        }

        #endregion

        protected override void OnCommandMoveTop()
        {
            var optionCount = 0;

            if (step == ShopStep.Main)
            {
                optionCount = Options.Count;
            }
            else
            {
                optionCount = buyItemActions.Count;
            }

            selectedOptionIndex = (selectedOptionIndex - 1 + optionCount) % optionCount;
        }
        protected override void OnCommandMoveBottom()
        {
            var optionCount = 0;

            if (step == ShopStep.Main)
            {
                optionCount = Options.Count;
            }
            else
            {
                optionCount = buyItemActions.Count;
            }

            selectedOptionIndex = (selectedOptionIndex + 1 + optionCount) % optionCount;
        }

        protected override void OnCommandInteract()
        {
            if(step == ShopStep.Main)
            {
                Options[selectedOptionIndex].Action.Invoke();
            }
            else
            {
                buyItemActions[selectedOptionIndex].Action.Invoke();
            }
        }
    }
}

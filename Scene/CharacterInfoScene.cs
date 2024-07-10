using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class CharacterInfoScene : Scene
    {
        public override string Title { get; protected set; } = "상 태 보 기";
        public override void EnterScene()
        {
            selectedOptionIndex = 0;
            Options.Clear();
            Options.Add(Managers.Scene.GetOption("Back"));
            DrawScene();
        }

        public override void NextScene()
        {
            do
            {
                GetInput();
            } while (Managers.Scene.CurrentScene is CharacterInfoScene);
        }
        protected override void DrawScene()
        {
            Renderer.DrawBorder(Title);

            // ==== 캐릭터 정보 표시 ====

            const int startOffset = 8;

            
            Renderer.DrawCenter(startOffset + 2, $"y무기w - {Game.Player.Equipment.Equipped[GearType.Weapon].Name}", Game.lobbyOffsetX);
            Renderer.DrawCenter(startOffset, "y착용 장비w", Game.lobbyOffsetX);
            Renderer.DrawCenter(startOffset + 4, $"y모자w - {Game.Player.Equipment.Equipped[GearType.Hat].Name}", Game.lobbyOffsetX);
            Renderer.DrawCenter(startOffset + 6, $"y갑옷w - {Game.Player.Equipment.Equipped[GearType.Armor].Name}", Game.lobbyOffsetX);
            Renderer.DrawCenter(startOffset + 8, $"y신발w - {Game.Player.Equipment.Equipped[GearType.Shoes].Name}", Game.lobbyOffsetX);
            Renderer.DrawCenter(startOffset + 10, $"y장갑w - {Game.Player.Equipment.Equipped[GearType.Gloves].Name}", Game.lobbyOffsetX);

            Renderer.ShowCharacterInfo();

            //Renderer.PrintKeyGuide("[ESC : 뒤로가기]");
        }
    }
}

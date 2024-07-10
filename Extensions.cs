using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public static class Extensions
    {
        // 아이템의 이름이 빈 문자열인 경우 => true
        public static bool IsEmptyItem(this Gear gear) => string.IsNullOrEmpty(gear.Name);

        // 아이템 타입을 문자로 리턴
        public static string String(this ItemType type)
        {
            switch (type)
            {
                case ItemType.Gear:
                    return "장비";
                case ItemType.ConsumeItem:
                    return "소모품";
                default:
                    return "";
            }
        }

        // 장비 타입을 문자로 리턴
        public static string String(this GearType type)
        {
            switch (type)
            {
                case GearType.Weapon:
                    return "무기";
                case GearType.Armor:
                    return "갑옷";
                case GearType.Hat:
                    return "모자";
                case GearType.Gloves:
                    return "장갑";
                case GearType.Shoes:
                    return "신발";
                default:
                    return "";
            }
        }


        // 직업 타입을 문자로 리턴
        public static string String(this JobType type)
        {
            switch (type)
            {
                case JobType.Warrior:
                    return "전사";
                case JobType.Mage:
                    return "마법사";
                case JobType.Rogue:
                    return "도적";
                case JobType.Archer:
                    return "궁수";
                default:
                    return "";
            }
        }


        public static string StatToString(this Gear gear)
        {
            string stat = string.Empty;

            if (gear.Atk != 0)
            {
                stat += $"공격력 {gear.Atk} ";
            }

            if (gear.Def != 0)
            {
                stat += $"방어력 {gear.Def} ";
            }


            return stat;
        }
    }
}

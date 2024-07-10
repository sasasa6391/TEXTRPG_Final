using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class GameTable
    {

        public static Skill[] Skills =
        {
            new Skill(JobType.Warrior, new List<int>{1, 3}, new List<string>{"전사스킬1","전사스킬2"}, new List<int>{1,1}, new List<float>{1.0f, 1.5f}, new List<int>{20, 25}, new List<SkillType>{SkillType.Target, SkillType.AllTarget}),
            new Skill(JobType.Mage, new List<int>{1, 3, 4, 5}, new List<string>{"에너지볼트", "썬더볼트", "소울스트라이크", "명상"}, new List<int>{1,1,3,1},new List<float>{1.5f, 1.3f, 0.8f, 75}, new List<int>{0, 50, 50, 0}, new List<SkillType>{SkillType.Target, SkillType.AllTarget, SkillType.Target, SkillType.RecoveryMP}),
            new Skill(JobType.Rogue, new List<int>{1, 3}, new List<string>{"도적스킬1", "도적스킬2"}, new List<int>{1,1}, new List<float>{1.0f, 1.5f}, new List<int>{20, 30}, new List<SkillType>{SkillType.Target, SkillType.AllTarget}),
            new Skill(JobType.Archer, new List<int>{1, 3}, new List<string>{"궁수스킬1", "궁수스킬2"}, new List<int>{1,1}, new List<float>{1.0f, 1.5f}, new List<int>{20, 30}, new List<SkillType>{SkillType.Target, SkillType.AllTarget})
        };

        public static Character[] Characters =
        {
            new Character("", JobType.Warrior, 1, 10, 5, 3, 100, 50, Skills[0]),
            new Character("", JobType.Mage, 1, 10, 3, 3, 80, 100, Skills[1]),
            new Character("", JobType.Rogue, 1, 8, 4, 5, 90, 70, Skills[2]),
            new Character("", JobType.Archer, 1, 7, 3, 4, 80, 100, Skills[3])
        };

        public static Item[] Items =
        {
            new Gear(1, "나무 스태프", "초보 마법사들이 사용하는 마법 지팡이", 200, GearType.Weapon, 5, 0, 0),
            new Gear(2, "사파이어 스태프", "사파이어로 장식된 지팡이", 600, GearType.Weapon, 8, 0, 0),
            new Gear(3, "고목나무 스태프", "오래된 고목에서 만들어진 지팡이", 1200, GearType.Weapon, 15, 0, 0),
            new Gear(4, "위저드 스태프", "숙련된 마법사들이 사용하는 지팡이", 2500, GearType.Weapon, 20, 0, 0),
            new Gear(5, "아크 스태프", "고대의 힘이 담긴 지팡이", 5000, GearType.Weapon, 35, 0, 0),

            new Gear(6, "플레 로브", "부드러운 천으로 만들어진 로브", 200, GearType.Armor, 0, 3 ,0),
            new Gear(7, "도로스 로브", "방어력이 뛰어난 로브", 600, GearType.Armor, 0, 6 ,0),
            new Gear(8, "사제의 로브", "신성한 힘이 깃든 로브", 1200, GearType.Armor, 0, 12,0),
            new Gear(9, "데빌즈 로브", "악마의 기운이 담긴 로브", 2500, GearType.Armor, 0, 18, 0),
            new Gear(10, "스타라이트", "별빛의 힘을 담은 로브", 5000, GearType.Armor, 0, 25, 0),

            new Gear(11, "고깔 모자", "마법사들이 흔히 쓰는 전통적인 모자", 200, GearType.Hat, 0, 1, 0),
            new Gear(12, "사제의 모자", "신성한 힘을 담은 모자", 600, GearType.Hat, 0, 3, 0),
            new Gear(13, "다크 매티", "어둠의 힘이 깃든 모자", 1200, GearType.Hat, 0, 5, 0),
            new Gear(14, "티아라", "왕족들이 쓰는 화려한 장식의 티아라", 2500, GearType.Hat, 0, 7, 0),
            new Gear(15, "세라피스", "천사의 축복이 담긴 모자", 5000, GearType.Hat, 0, 9, 0),

            new Gear(16, "베이지 니티", "편안한 신발", 200, GearType.Shoes, 0, 0, 3),
            new Gear(17, "쥬얼리 슈즈", "보석이 박힌 신발", 600, GearType.Shoes, 0, 0, 5),
            new Gear(18, "윈드슈즈", "바람의 힘이 담긴 신발", 1200, GearType.Shoes, 0, 0, 7),
            new Gear(19, "매직슈즈", "마법의 힘이 담긴 신발", 2500, GearType.Shoes, 0, 0, 9),
            new Gear(20, "문슈즈", "달의 힘이 깃든 신발", 5000, GearType.Shoes, 0, 0, 11),

            new Gear(21, "레모나", "상큼한 힘을 담은 장갑", 200, GearType.Gloves, 2, 1, 0),
            new Gear(22, "퍼플 모리칸", "보라색 마법의 힘이 담긴 장갑", 600, GearType.Gloves, 4, 2, 0),
            new Gear(23, "블랙 루티아", "검은 기운이 깃든 장갑", 1200, GearType.Gloves, 7, 4, 0),
            new Gear(24, "블랙 아르텐", "방어력이 뛰어난 검은 장갑", 2500, GearType.Gloves, 12, 6, 0),
            new Gear(25, "다크 페넌스", "어둠의 힘이 담긴 장갑", 5000, GearType.Gloves, 20, 10, 0),

            new HealingPotion(26, "체력 포션", "체력을 회복하는 물약", 150, 1, 50),
            new ManaPotion(27, "마나 포션", "마나를 회복하는 물약", 150, 1, 50),
        };

        public static string[] BattleSceneNames =
        {
            "고대의 숲", "그림자 성채", "불타는 폐허", "서리 동굴", "어둠의 성역"
        };
        public static Monster[] Monsters =
        {
            new Monster(1, "나무 정령", 1),
            new Monster(2, "울프", 2),
            new Monster(3, "독거미", 3),
            new Monster(4, "나무 수호자", 4, true),
            new Monster(5, "섀도우 나이트", 5),
            new Monster(6, "고스트", 6),
            new Monster(7, "다크 엘프", 7),
            new Monster(8, "섀도우 엠퍼러", 8, true),
            new Monster(9, "파이어 임프", 9),
            new Monster(10, "화염 정령", 10),
            new Monster(11, "레드 와이번", 11),
            new Monster(12, "레드 드래곤", 12, true),
            new Monster(13, "얼음 정령", 13),
            new Monster(14, "아이스 골렘", 14),
            new Monster(15, "화이트 울프", 15),
            new Monster(16, "서리여왕", 16, true),
            new Monster(17, "다크 메이지", 17),
            new Monster(18, "다크 나이트", 18),
            new Monster(19, "뱀파이어", 19),
            new Monster(20, "다크 엠퍼러", 20, true),
        };


        public static List<List<int>> MonsterGroups = new List<List<int>>
        {
            new List<int>{1,1,1},
            new List<int>{1,2,1},
            new List<int>{2,2,2},
            new List<int>{2,3,2},
            new List<int>{3,4,3},

            new List<int>{5,5,5},
            new List<int>{5,6,5},
            new List<int>{6,6,6},
            new List<int>{6,7,6},
            new List<int>{7,8,7},

            new List<int>{9,9,9},
            new List<int>{9,10,9},
            new List<int>{10,10,10},
            new List<int>{10,11,10},
            new List<int>{11,12,11},

            new List<int>{13,13,13},
            new List<int>{13,14,13},
            new List<int>{14,14,14},
            new List<int>{14,15,14},
            new List<int>{15,16,15},

            new List<int>{17,17,17},
            new List<int>{17,18,17},
            new List<int>{18,18,18},
            new List<int>{18,19,18},
            new List<int>{19,20,19},
        };


        public static List<Quest> QuestTable = new List<Quest>()
        {
            new KillMonsterQuest(1, 1, 5, BattleSceneNames[0], 500, 200),
            new KillMonsterQuest(2, 2, 5, BattleSceneNames[0], 1000, 400),
            new KillMonsterQuest(3, 3, 3, BattleSceneNames[0], 1500, 600),
            new KillMonsterQuest(4, 4, 1, BattleSceneNames[0], 3000, 1200),
            new KillMonsterQuest(5, 5, 5, BattleSceneNames[1], 2000, 800),
            new KillMonsterQuest(6, 6, 5, BattleSceneNames[1], 2500, 1000),
            new KillMonsterQuest(7, 7, 3, BattleSceneNames[1], 3000, 1200),
            new KillMonsterQuest(8, 8, 1, BattleSceneNames[1], 6000, 2400),
            new KillMonsterQuest(9, 9, 5, BattleSceneNames[2], 3500, 1400),
            new KillMonsterQuest(10, 10, 5, BattleSceneNames[2], 4000, 1600),
            new KillMonsterQuest(11, 11, 3, BattleSceneNames[2], 4500, 1800),
            new KillMonsterQuest(12, 12, 1, BattleSceneNames[2], 9000, 3600),
            new KillMonsterQuest(13, 13, 5, BattleSceneNames[3], 5000, 2000),
            new KillMonsterQuest(14, 14, 5, BattleSceneNames[3], 5500, 2200),
            new KillMonsterQuest(15, 15, 3, BattleSceneNames[3], 6000, 2400),
            new KillMonsterQuest(16, 16, 1, BattleSceneNames[3], 12000, 4800),
            new KillMonsterQuest(17, 17, 5, BattleSceneNames[4], 6500, 2600),
            new KillMonsterQuest(18, 18, 5, BattleSceneNames[4], 7000, 2800),
            new KillMonsterQuest(19, 19, 3, BattleSceneNames[4], 7500, 3000),
            new KillMonsterQuest(20, 20, 1, BattleSceneNames[4], 1500, 6000),
        };

    }
}

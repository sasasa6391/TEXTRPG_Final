using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    [Serializable]
    public abstract class Quest
    {
        public int ProgressValue;
        public int AchieveValue;
        public int ID;
        public string Pos;
        public int RewardGold;
        public int RewardExp;

        public abstract bool CheckEnd();
        public (int, int) CurrentProgress()
        {
            return (ProgressValue, AchieveValue);
        }
        public Quest()
        {
            ProgressValue = 0;
        }

        public abstract string GetDesc();
    }

    public class KillMonsterQuest : Quest
    {
        public int TargetMonsterID;

        public KillMonsterQuest() : base()
        {
            Managers.Game.OnKillMonster += OnProgress;
        }
        public KillMonsterQuest(int questID, int targetMonsterID, int monsterCount, string pos, int rewardGold, int rewardExp) : base()
        {
            Managers.Game.OnKillMonster += OnProgress;
            ID = questID;
            TargetMonsterID = targetMonsterID;
            ProgressValue = 0;
            AchieveValue = monsterCount;
            Pos = pos;
            RewardGold = rewardGold;
            RewardExp = rewardExp;
        }
        public KillMonsterQuest(KillMonsterQuest reference) : base()
        {
            Managers.Game.OnKillMonster += OnProgress;
            ID = reference.ID;
            TargetMonsterID = reference.TargetMonsterID;
            ProgressValue = 0;
            AchieveValue = reference.AchieveValue;
            Pos = reference.Pos;
            RewardGold = reference.RewardGold;
            RewardExp = reference.RewardExp;
        }

        public override bool CheckEnd()
        {
            return AchieveValue <= ProgressValue;
        }

        public override string GetDesc()
        {
            if (GameTable.Monsters[TargetMonsterID - 1].IsBoss == true)
            {
                return $"{GameTable.Monsters[TargetMonsterID - 1].Name} {AchieveValue}마리 처치";
            }
            else
            {
                return $"{GameTable.Monsters[TargetMonsterID - 1].Name} {AchieveValue}마리 처치 ({ProgressValue} / {AchieveValue})";
            }
        }

        public void OnProgress(int id)
        {
            if (id == TargetMonsterID)
            {
                ProgressValue++;
            }
        }

    }
}

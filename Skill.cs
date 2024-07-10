using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public enum SkillType
    {
        Target,
        AllTarget,
        RecoveryHP,
        RecoveryMP,
    }
    public class Skill
    {
        public JobType Job;
        public List<int> ReqLevel;
        public List<string> Names;
        public List<int> AttackCount;
        public List<float> DamageRate;
        public List<int> MpCost;
        public List<SkillType> SkillTypes;
        public int Count;

        public Skill()
        {

        }
        public Skill(JobType job)
        {
            Job = job; 
            ReqLevel = new List<int>();
            Names = new List<string>();
            AttackCount = new List<int>();
            DamageRate = new List<float>();
            MpCost = new List<int>();
            SkillTypes = new List<SkillType>();
            Count = 0; 
        }

        public Skill(JobType job, List<int> reqLevel, List<string> names, List<int> attackCount, List<float> damageRate, List<int> mpCost, List<SkillType> skillType)
        {
            Job = job;
            ReqLevel = reqLevel;
            Names = names;
            AttackCount = attackCount;
            DamageRate = damageRate;
            Count = Names.Count;
            MpCost = mpCost;
            SkillTypes = skillType;
        }

        public List<string> CheckNewSkill(int level)
        {
            var result = new List<string>();

            var allSkillList = GameTable.Skills.FirstOrDefault(e => e.Job == Job);

            for (int i = Count; i < allSkillList.Count; i++)
            {
                if (level >= allSkillList.ReqLevel[i])
                {
                    ReqLevel.Add(allSkillList.ReqLevel[i]);
                    Names.Add(allSkillList.Names[i]);
                    AttackCount.Add(allSkillList.AttackCount[i]);
                    DamageRate.Add(allSkillList.DamageRate[i]);
                    MpCost.Add(allSkillList.MpCost[i]);
                    SkillTypes.Add(allSkillList.SkillTypes[i]);
                    result.Add(Names[i]);
                    Count++;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}

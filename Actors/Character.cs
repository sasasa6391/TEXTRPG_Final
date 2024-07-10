using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public enum JobType
    {
        Warrior,
        Mage,
        Rogue,
        Archer,
    }

    public class Character : Actor
    {

        public JobType Job { get; set; }
        public int HpMax => DefaultHpMax + hpMaxModifier;
        public int Damage => DefaultDamage + damageModifier;
        public int Defense => DefaultDefense + defenseModifier;
        public int MpMax => DefaultMpMax + mpMaxModifier;
        public int Speed => DefaultSpeed + speedModifier;

        public int MpPad = 0;

        public int nextLevelExp;
        public int totalExp;

        public Skill PlayerSkill;
        public override int Hp
        {
            get => hp;
            set
            {
                SetHpPad();
                if (value <= 0) hp = 0;
                else if (value >= HpMax) hp = HpMax;
                else hp = value;
            }
        }

        private string name;

        public override string Name
        {
            get
            {
                return $"g{name}w";
            }
            set
            {
                name = value;
            }
        }
        public void SetMpPad()
        {
            var tMp = mp;
            var tMpPad = 0;
            while (tMp > 0)
            {
                tMpPad++;
                tMp /= 10;
            }
            MpPad = Math.Max(tMpPad, MpPad);
        }
        protected int mp;
        public int Mp
        {
            get => mp;
            set
            {
                SetMpPad();

                if (value <= 0) mp = 0;
                else if (value >= MpMax) mp = MpMax;
                else mp = value;
            }
        }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }

        public int hpMaxModifier;
        public int damageModifier;
        public int defenseModifier;
        public int mpMaxModifier;
        public int speedModifier;
        public Character(string name, JobType job, int level, int damage, int defense, int speed, int hp, int mp, Skill playerSkill)
        {
            Name = name;
            Job = job;
            Level = level;
            DefaultDamage = damage;
            DefaultDefense = defense;
            DefaultHpMax = hp;
            DefaultMpMax = mp;
            DefaultSpeed = speed;

            Inventory = new Inventory(this);
            Equipment = new Equipment();
            nextLevelExp = 50;
            totalExp = 0;
            Hp = HpMax;
            Mp = MpMax;

            SetHpPad();

            var tMp = mp;
            var tMpPad = 0;
            while (tMp > 0)
            {
                tMpPad++;
                tMp /= 10;
            }
            MpPad = Math.Max(tMpPad, MpPad);

            PlayerSkill = new Skill(job);
            PlayerSkill.CheckNewSkill(level);
        }

        public override LerpObject Attack(Actor actor, int line, float skillRate)
        {
            var rand = new Random();

            double minValue = 0.8;
            double maxValue = 1.2;

            double randomValue = minValue + (rand.NextDouble() * (maxValue - minValue));

            int finalDamage = Math.Max((int)(((double)(Damage - actor.DefaultDefense) * skillRate) * randomValue), 1);

            string battleText = $"{Name}은 {actor.Name}에게 r{finalDamage}w의 데미지를 입혔다!";
            Renderer.ShowText(60, line, battleText);
            return actor.OnDamaged(finalDamage);
        }

        public void Skill(Actor actor, ref int line, float damage)
        {
            if (actor.IsDead())
                return;
        }

        public override LerpObject OnDamaged(int damage)
        {
            SFXPlayer.Instance.music = "[Effect]Attack1_default.mp3";
            SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
            int from = Hp;
            int dest = Math.Max(Hp - damage, 0);
            return new LerpObject(from, dest, 15, this, 0);
        }

        public void ChangeMana(int value)
        {
            Mp += value;
        }
        public override bool IsDead()
        {
            if (hp <= 0) return true;
            return false;
        }

        public void ChangeGold(int gold)
        {
            Inventory.Gold += gold;
        }

        public void ChangeExp(int expAmount)
        {
            int levelsToAdvance = 0;
            totalExp += expAmount;
            while (totalExp >= nextLevelExp)
            {
                totalExp -= nextLevelExp;
                levelsToAdvance++;
                nextLevelExp += 50;
            }
            LevelUp(levelsToAdvance);
        }

        public void LevelUp(int levelsToAdvance)
        {
            if (levelsToAdvance == 0)
                return;
            nextLevelExp += Level * 40;
            Level += levelsToAdvance;
            DefaultHpMax += 20;
            DefaultMpMax += 20;
            DefaultDamage += 3 * levelsToAdvance;
            DefaultDefense += 1 * levelsToAdvance;
            Hp = HpMax;
            Mp = MpMax;
            Thread.Sleep(Managers.Game.GetGameSleepTime(300));

            /*
            // 출력
            */
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public abstract class Actor
    {
        #region Properties
        public virtual string Name { get; set; } = "232";
        public int DefaultHpMax { get; set; }
        public int DefaultDamage { get; set; }
        public int DefaultDefense { get; set; }
        public int DefaultSpeed { get; set; }
        public int DefaultMpMax { get; set; }

        public int HpPad;

        public int Level { get; set; }

        protected int hp;
        public virtual int Hp
        {
            get => hp;
            set
            {
                SetHpPad();
                if (value <= 0) hp = 0;
                else if (value >= DefaultHpMax) hp = DefaultHpMax;
                else hp = value;
            }
        }

        public void SetHpPad()
        {
            var tHp = hp;
            var tHpPad = 0;
            while (tHp > 0)
            {
                tHpPad++;
                tHp /= 10;
            }
            HpPad = Math.Max(tHpPad, HpPad);
        }

        #endregion


        public abstract LerpObject Attack(Actor actor, int line, float skillRate);

        public abstract LerpObject OnDamaged(int damage);

        public virtual bool IsDead()
        {
            return false;
        }

    }
}

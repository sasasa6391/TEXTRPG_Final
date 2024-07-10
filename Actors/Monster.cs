using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace 김도명_TEXTRPG
{
    public class Monster : Actor
    {
        public override int Hp
        {
            get => hp;
            set
            {
                SetHpPad();
                if (value <= 0) hp = 0;
                else hp = value;
            }
        }

        public int ID;
        public int Exp;
        public int Gold;
        public bool IsBoss;

        private string name;
        public override string Name
        {
            get
            {
                if(IsBoss == true)
                {
                    return $"y[rBossy]{name}w";
                }
                else
                {
                    return $"y{name}w";
                }
            }
            set
            {
                name = value;
            }
        }
        public Monster(int id, string name, int level, bool IsBoss = false)
        {
            ID = id;
            Name = name;
            Level = level;
            this.IsBoss = IsBoss;
            if (IsBoss == true)
            {
                Hp = level * 30;
                DefaultHpMax = Hp;
                DefaultDamage = level * 15;
                DefaultDefense = level;
                DefaultSpeed = (int)((float)level * 1.5f);
                DefaultMpMax = level * 5;
                Exp = level * 50;
                Gold = level * 200;
            }
            else
            {
                Hp = level * 15;
                DefaultHpMax = Hp;
                DefaultDamage = level * 7;
                DefaultDefense = level;
                DefaultSpeed = level;
                DefaultMpMax = level * 5;
                Exp = level * 20;
                Gold = level * 50;
            }
            SetHpPad();
        }
        public Monster(Monster other)
        {

            ID = other.ID;
            Name = other.Name;
            Level = other.Level;
            Hp = other.Hp;
            DefaultHpMax = other.DefaultHpMax;
            DefaultDamage = other.DefaultDamage;
            DefaultDefense = other.DefaultDefense;
            DefaultSpeed = other.DefaultSpeed;
            DefaultMpMax = other.DefaultMpMax;
            Exp = other.Exp;
            Gold = other.Gold;
            SetHpPad();

        }


        public override LerpObject Attack(Actor actor, int line, float skillRate)
        {
            var rand = new Random();

            double minValue = 0.8;
            double maxValue = 1.2;

            double randomValue = minValue + (rand.NextDouble() * (maxValue - minValue));

            var c = actor as Character;

            int finalDamage = Math.Max((int)((double)(DefaultDamage - c.Defense) * randomValue), 1);
            string battleText = $"{Name}은 {actor.Name}에게 r{finalDamage}w의 데미지를 입혔다!";
            Renderer.ShowText(60, line, battleText);
            return actor.OnDamaged(finalDamage);
        }

        public override LerpObject OnDamaged(int damage)
        {
            SFXPlayer.Instance.music = "[Effect]Attack1_default.mp3";
            SFXPlayer.Instance.PlayAsync(1.0f); // 음악파일명, 볼륨
            int from = Hp;
            int dest = Math.Max(Hp - damage, 0);
            return new LerpObject(from, dest, 15, this, 0);
        }

        public override bool IsDead()
        {
            if (hp <= 0) return true;
            return false;
        }
    }
}

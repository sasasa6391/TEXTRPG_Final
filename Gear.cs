using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public enum GearType
    {
        Weapon,
        Hat,
        Armor,
        Shoes,
        Gloves,
        None, 
    }

    public class Gear : Item
    {
        public GearType GearType { get; set; }
        public int Atk { get; set; }    // 공격력
        public int Def { get; set; }    // 방어력
        public int Speed { get; set; } // 스피드
        public bool IsEquip { get; set; }

        public Gear()
        {
            OnRemoved += (owner) => { if (owner.Equipment.Equipped[(GearType)GearType] == this) owner.Equipment.Unequip((GearType)GearType); };
        }

        public Gear(int id, string name, string description, int price, GearType gearType, int atk, int def, int speed, ItemType itemType = ItemType.Gear, bool isEquip = false) : base(id, name, description, price, itemType)
        {
            GearType = gearType;
            Atk = atk;
            Def = def;
            Speed = speed;
            IsEquip = isEquip;
            OnRemoved += (owner) => { if (owner.Equipment.Equipped[(GearType)GearType] == this) owner.Equipment.Unequip((GearType)GearType); };
        }

        public Gear(Gear reference) : base(reference)
        {
            GearType = reference.GearType;
            Atk = reference.Atk;
            Def = reference.Def;
            Speed = reference.Speed;
            IsEquip = reference.IsEquip;
            OnRemoved += (owner) => { if (owner.Equipment.Equipped[(GearType)GearType] == this) owner.Equipment.Unequip((GearType)GearType); };
        }


        public static Gear Empty = new Gear(-1, "없음", string.Empty, 0, GearType.None, 0, 0, 0);
        public override Item DeepCopy() => new Gear(this);
    }

}

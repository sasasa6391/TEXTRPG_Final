using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    public class ConsumeItem : Item
    {
        public string EffectDesc { get; set; }

        public ConsumeItem()
        {
            EffectDesc = string.Empty;
            OnUsed += UseEffect;
        }

        public ConsumeItem(int id, string name, string description, int price, int stackCount, ItemType itemType = ItemType.ConsumeItem, string effectDesc = null) : base(id, name, description, price, itemType)
        {
            EffectDesc = effectDesc ?? "";
            OnUsed += UseEffect;
        }

        public ConsumeItem(ConsumeItem reference) : base(reference)
        {
            EffectDesc = reference.EffectDesc;
            OnUsed += UseEffect;
        }


        public virtual LerpObject UseEffect(Character owner)
        {
            if (StackCount > 1)
                StackCount--;
            else
                owner.Inventory.Remove(this);

            return null;
        }
        public override Item DeepCopy() => new ConsumeItem(this);
    }

    public class HealingPotion : ConsumeItem
    {
        private int healValue;
        public int HealValue { get => healValue; set { healValue = value; EffectDesc = $"체력 {healValue} 회복"; } }

        public HealingPotion() : base()
        {
        }

        public HealingPotion(int id, string name, string description, int price, int stackCount, int healValue, ItemType itemType = ItemType.ConsumeItem, string effectDesc = null) : base(id, name, description, price, stackCount, itemType, effectDesc)
        {
            HealValue = healValue;
        }

        public HealingPotion(HealingPotion reference) : base(reference)
        {
            HealValue = reference.HealValue;
        }

        public override LerpObject UseEffect(Character owner)
        {
            base.UseEffect(owner);
            return new LerpObject(owner.Hp, Math.Min(owner.Hp + healValue, owner.HpMax), 15, owner, 0);
        }

        public override Item DeepCopy() => new HealingPotion(this);
    }

    public class ManaPotion : ConsumeItem
    {
        private int healValue;
        public int HealValue { get => healValue; set { healValue = value; EffectDesc = $"마나 {healValue} 회복"; } }

        public ManaPotion() : base()
        {
        }

        public ManaPotion(int id, string name, string description, int price, int stackCount, int healValue, ItemType itemType = ItemType.ConsumeItem, string effectDesc = null) : base(id, name, description, price, stackCount, itemType, effectDesc)
        {
            HealValue = healValue;
        }

        public ManaPotion(ManaPotion reference) : base(reference)
        {
            HealValue = reference.HealValue;
        }

        public override LerpObject UseEffect(Character owner)
        {
            base.UseEffect(owner);
            return new LerpObject(owner.Mp, Math.Min(owner.Mp + healValue, owner.MpMax), 15, owner, 1);
        }

        public override Item DeepCopy() => new ManaPotion(this);
    }
}

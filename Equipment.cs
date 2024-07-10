using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{

    public class Equipment
    {
        private Dictionary<GearType, Gear> equipped = new Dictionary<GearType, Gear>();
        public IReadOnlyDictionary<GearType, Gear> Equipped => equipped;

        public Equipment()
        {
            var slots = Enum.GetValues(typeof(GearType));

            foreach (GearType slot in slots)
            {
                Gear value;
                if (equipped.TryGetValue(slot, out value) && value != null) continue;
                equipped[slot] = Gear.Empty;
            }
        }

        public void Equip(GearType slot, Gear gear)
        {
            equipped.TryGetValue(slot, out var item);

            // 같은 장비를 착용중 인가?
            if (item == gear)
            {
                Unequip(slot);
                return;
            }

            // 해당 장비창이 비어있지 않은가?
            if (!equipped[slot].IsEmptyItem())
            {
                Unequip(slot);
            }

            equipped[slot] = gear;
            StatAdd(equipped[slot]);
            equipped[slot].IsEquip = true;
        }

        public void Unequip(GearType slot)
        {
            StatSubtract(equipped[slot]);
            equipped[slot].IsEquip = false;
            equipped[slot] = Gear.Empty;
        }

        public void StatAdd(Gear gear)
        {
            Game.Player.DefaultDamage += gear.Atk;
            Game.Player.DefaultDefense += gear.Def;
            Game.Player.DefaultSpeed += gear.Speed;
        }

        public void StatSubtract(Gear gear)
        {
            Game.Player.DefaultDamage -= gear.Atk;
            Game.Player.DefaultDefense -= gear.Def;
            Game.Player.DefaultSpeed -= gear.Speed;
        }
    }


}

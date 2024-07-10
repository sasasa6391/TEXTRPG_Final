using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{
    // 아이템 타입
    public enum ItemType
    {
        Gear,
        ConsumeItem,
    }

    public class Item
    {
        /// <summary>
        /// 아이템을 식별할 고유 번호 <br/>
        /// </summary>
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public ItemType Type { get; set; }

        public int StackCount { get; set; }

        public Item() {
            OnAdded += MergeItem;
        }
        public Item(int id, string name, string description, int price, ItemType type)
        {
            ID = id;
            Name = name;
            Description = description;
            Price = price;
            Type = type;
            StackCount = 1;
            OnAdded += MergeItem;
        }

        public Item(Item reference)
        {
            ID = reference.ID;
            Name = reference.Name;
            Description = reference.Description;
            Price = reference.Price;
            Type = reference.Type;
            StackCount = reference.StackCount;
            OnAdded += MergeItem;
        }

        //아이템이 사용될 때 호출될 이벤트
        protected event Func<Character, LerpObject> OnUsed;

        //아이템이 인벤토리에 추가될 때 호출될 이벤트
        protected event Action<Character, Item> OnAdded;

        //아이템이 인벤토리에서 삭제될 떄 호출될 이벤트
        //만약 장착중인 장비아이템이 삭제된다면, 장착해제도 같이 진행해야합니다.
        protected event Action<Character> OnRemoved;

        public void Use(Character owner) => OnUsed?.Invoke(owner);
        public void OnAdd(Character owner, Item duplicatedItem = null) => OnAdded?.Invoke(owner, duplicatedItem);
        public void OnRemove(Character owner) => OnRemoved?.Invoke(owner);
        public virtual Item DeepCopy() => new Item(this);


        public void MergeItem(Character onwer, Item duplicatedItem)
        {
            // 중복된 아이템이 있을 경우 duplicatedItem로 받아옵니다.
            // duplicatedItem이 null이 아니라면 두 아이템의 개수를 합칩니다.
            StackCount += duplicatedItem != null ? duplicatedItem.StackCount : 0;
        }

    }
}

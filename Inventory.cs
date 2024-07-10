using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 김도명_TEXTRPG
{

    public enum ItemOrderMode
    {
        Default,
        NameAsc,
        NameDesc,
        AtkDesc,
        DefDesc,
        PriceAsc,
        PriceDesc,
    }

    public class Inventory
    {
        private List<Item> _items;
        public List<Item> Items => _items;
        /// <summary>
        /// 이 인벤토리의 주인인 Character 클래스를 가리킵니다.
        /// </summary>
        public Character Parent { get; } //추후에 상점이나 몬스터도 인벤토리를 가질 수 있음.
        public int Gold { get; set; }

        /// <summary>
        /// 인벤토리 내부의 아이템 개수를 반환합니다.<br/>
        /// return items.Count
        /// </summary>
        public int Count { get => _items.Count; }

        private Action<Item> _onAdded = null;
        private Action<Item> _onRemoved = null;


        /// <summary>
        /// 현재 인벤토리에서 이름이 가장 긴 아이템의 byte를 저장합니다.<br/>
        /// 콘솔에서 테이블 크기를 결정하기 위해 사용합니다.
        /// </summary>
        public int MaxPad { get; private set; }
        public Inventory(Character _parent)
        {
            _items = new List<Item>();
            Gold = 0;
            Parent = _parent;
            MaxPad = 4;
        }

        public void Add(Item item)
        {
            if (!HasSameItem(item, out Item res))
            {
                // 중복되는 아이템이 없는 경우만 add
                _items.Add(item);
                item.OnAdd(Parent);
            }
            else
            {
                res.OnAdd(Parent, item);
            }
            _onAdded?.Invoke(item);
        }

        public void Remove(Item item)
        {
            _items.Remove(item);
            item.OnRemove(Parent);
            _onRemoved?.Invoke(item);
        }


        /// <summary>
        /// 인벤토리에 같은 아이템이 있는지 찾습니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool HasSameItem(Item item) => HasSameItem(item, out _);


        /// <summary>
        /// 인벤토리에 같은 아이템이 있는지 찾습니다. <br/>
        /// 중복되는 아이템이 있다면 out 매개변수로 반환합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="res"> item과 같은 아이템의 객체입니다.</param>
        /// <returns></returns>
        public bool HasSameItem(Item item, out Item res)
        {
            for (int i = 0; i < _items.Count; i++)
            {

                if (_items[i].ID == item.ID)
                {
                    res = _items[i];
                    return true;
                }
            }
            res = null;
            return false;
        }

        public Item this[int idx]
        {
            get
            {
                if (idx < 0 || idx >= _items.Count)
                    throw new ArgumentOutOfRangeException(nameof(idx));
                return _items[idx];
            }
        }
    }
}

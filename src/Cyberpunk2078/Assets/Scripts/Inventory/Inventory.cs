using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable] public struct Item
{
    public int id;
    public uint num;

    public Item(KeyValuePair<int, uint> pair)
    {
        id = pair.Key;
        num = pair.Value;
    }
}


[Serializable] public class Inventory : IAttributeCollection, ISerializationCallbackReceiver
{
    [SerializeField] private Item[] serializedItems;
    private Dictionary<int, uint> items;
    private AttributeSet sumAttributes;


    public uint this[int id]
    {
        get
        {
            return items.ContainsKey(id) ? items[id] : 0;
        }
    }

    public float this[AttributeType type] => sumAttributes[type];

    public EventOnAttributeChange OnAttributeChange => sumAttributes.OnAttributeChange;


    public Inventory()
    {
        items = new Dictionary<int, uint>();
        sumAttributes = new AttributeSet();
    }


    /// <summary>
    /// Add items to the inventory
    /// </summary>
    /// <param name="id"> The ID of the item to be added </param>
    /// <param name="num"> The number of items to be added </param>
    /// <returns> The number of items which were actually added </returns>
    public uint AddItem(int id, uint num = 1)
    {
        if (items.ContainsKey(id))
            items[id] += num;
        else
            items.Add(id, num);

        sumAttributes.Modify(DataTableManager.singleton.GetItemData(id).Attributes, (int)num);

        return num;
    }

    /// <summary>
    /// Remove items from the inventory
    /// </summary>
    /// <param name="id"> The ID of the item to be removed </param>
    /// <param name="num"> The number of items to be removed </param>
    /// <returns> The number of items which were actually removed </returns>
    public uint RemoveItem(int id, uint num = 1)
    {
        if (items.ContainsKey(id))
        {
            if (num < items[id])
            {
                items[id] -= num;

                sumAttributes.Modify(DataTableManager.singleton.GetItemData(id).Attributes, -(int)num);

                return num;
            }
            else
            {
                uint numRemoved = items[id];

                items.Remove(id);

                sumAttributes.Modify(DataTableManager.singleton.GetItemData(id).Attributes, -(int)numRemoved);

                return numRemoved;
            }
        }

        return 0;
    }


    public IEnumerator<KeyValuePair<AttributeType, float>> GetEnumerator()
    {
        return sumAttributes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return sumAttributes.GetEnumerator();
    }


    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if (serializedItems == null || serializedItems.Length < items.Count)
            serializedItems = new Item[items.Count];
#else
        serializedItems = new Item[items.Count];
#endif

        int i = 0;
        foreach (KeyValuePair<int, uint> item in items)
            serializedItems[i++] = new Item(item);
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (serializedItems != null)
        {
            items.Clear();

            foreach (Item item in serializedItems)
                if (!items.ContainsKey(item.id))
                    items.Add(item.id, item.num);

#if UNITY_EDITOR
#else
            serializedItems = null;
#endif
        }
    }


    public override string ToString()
    {
        string s = "";

        foreach (KeyValuePair<int, uint> item in items)
            s += ";" + item.Key + ":" + item.Value;

        return s.Substring(1);
    }
}

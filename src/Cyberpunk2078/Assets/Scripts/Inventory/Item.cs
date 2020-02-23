using UnityEngine;


public abstract class Item : MonoBehaviour
{
    [SerializeField] protected int itemID;


    public int ItemID
    {
        get
        {
            return itemID;
        }
    }
}

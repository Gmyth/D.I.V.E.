using System.Collections.Generic;
using UnityEngine;


public class ObjectRecycler : MonoBehaviour
{
    public static ObjectRecycler Singleton { get; private set; }

    private static Vector3 recyclePosition = Vector3.one * -10000;
    
    
    [SerializeField] private Recyclable[] prefabs;

    private Stack<Recyclable>[] recycledObjects;


    private ObjectRecycler() { }


    public Recyclable GetObject(int id)
    {
        if (recycledObjects[id].Count > 0)
            return recycledObjects[id].Pop();


        Recyclable recyclable = Instantiate(prefabs[id], transform);
        recyclable.id = id;


        return recyclable;
    }

    public Recyclable GetObject(int id, Vector3 position)
    {
        if (recycledObjects[id].Count > 0)
            return recycledObjects[id].Pop();


        Recyclable recyclable = Instantiate(prefabs[id], position, Quaternion.identity, transform.GetChild(id));
        recyclable.id = id;


        return recyclable;
    }

    public T GetObject<T>(int id) where T : MonoBehaviour
    {
        if (recycledObjects[id].Count > 0)
            return recycledObjects[id].Pop().GetComponent<T>();


        T obj = Instantiate(prefabs[id].GetComponent<T>(), recyclePosition, Quaternion.identity, transform.GetChild(id));

        if (obj)
        {
            obj.GetComponent<Recyclable>().id = id;
            obj.gameObject.SetActive(false);
        }


        return obj;
    }

    public SingleEffect GetSingleEffect(int id, Vector3 position)
    {
        SingleEffect effect = GetObject<SingleEffect>(id);
        effect.transform.position = position;
        effect.transform.right = transform.right;
        effect.transform.localScale = Vector3.one;
        effect.setTarget(null);
        effect.gameObject.SetActive(true);


        return effect;
    }

    public SingleEffect GetSingleEffect(int id, Transform target)
    {
        SingleEffect effect = GetObject<SingleEffect>(id);
        effect.transform.position = target.position;
        effect.transform.right = transform.right;
        effect.transform.localScale = Vector3.one;
        effect.setTarget(target);
        effect.gameObject.SetActive(true);


        return effect;
    }

    public SingleEffect GetSingleEffect(int id, Transform target, Vector3 positionOffset)
    {
        SingleEffect effect = GetObject<SingleEffect>(id);
        effect.transform.position = target.position + positionOffset;
        effect.transform.right = transform.right;
        effect.transform.localScale = Vector3.one;
        effect.setTarget(target);
        effect.gameObject.SetActive(true);


        return effect;
    }


    public void Recycle(Recyclable recyclable)
    {
        if (recyclable.id >= 0)
        {
            recyclable.gameObject.SetActive(false);
            recyclable.transform.localPosition = recyclePosition;

            recycledObjects[recyclable.id].Push(recyclable);
        }
        else
            Destroy(recyclable.gameObject);
    }

    public void RecycleAll()
    {
        foreach (Recyclable recyclable in FindObjectsOfType<Recyclable>())
            Recycle(recyclable);
    }


    private void Awake()
    {
        if (!Singleton)
        {
            Singleton = this;
            //TEMP: Reload the scene will have issue
            //DontDestroyOnLoad(gameObject);

            recycledObjects = new Stack<Recyclable>[prefabs.Length];

            GameObject child;
            for (int id = 0; id < recycledObjects.Length; id++)
            {
                recycledObjects[id] = new Stack<Recyclable>(256);

                child = new GameObject(id.ToString() + " - " + prefabs[id].name);
                child.transform.parent = transform;
                child.transform.localPosition = Vector3.zero;
            }
        }
        else if (this != Singleton)
            Destroy(gameObject);
    }
}

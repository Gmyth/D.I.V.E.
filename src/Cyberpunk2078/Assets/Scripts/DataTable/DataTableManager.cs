using System.Collections.Generic;
using UnityEngine;


public class DataTableManager
{
    public static readonly DataTableManager singleton = new DataTableManager();


    private Dictionary<string, DataTable> dataTables = new Dictionary<string, DataTable>();


    private DataTableManager()
    {
        foreach (DataTable dataPage in Resources.LoadAll<DataTable>("DataTables"))
            dataTables.Add(dataPage.name, dataPage);
    }


    public int GetEntryCount(string tableName)
    {
        return dataTables[tableName].Count;
    }

    /// <summary>
    /// Get data at a specific index in the given data page
    /// </summary>
    /// <typeparam name="T"> The type of the data </typeparam>
    /// <param name="tableName"> The name of the data page containing the desired data entry </param>
    /// <param name="index"> The index of the data entry to get </param>
    /// <returns></returns>
    public T GetData<T>(string tableName, int index) where T : DataTableEntry
    {
        return ((DataTable<T>)dataTables[tableName])[index];
    }

    public MotionData GetMotionData(int id)
    {
        return GetData<MotionData>("Motion", id);
    }

    public ItemData GetItemData(int id)
    {
        return GetData<ItemData>("Item", id);
    }

    public EnemyData GetEnemyData(int id)
    {
        return GetData<EnemyData>("Enemy", id);
    }
}

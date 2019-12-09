using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class DataTableManager
{
    public static readonly DataTableManager singleton = new DataTableManager();


    private Dictionary<string, DataTable> dataTables = new Dictionary<string, DataTable>();
    private Dictionary<int, string[]> textTable = new Dictionary<int, string[]>();


    private DataTableManager()
    {
        foreach (DataTable dataPage in Resources.LoadAll<DataTable>("DataTables"))
            dataTables.Add(dataPage.name, dataPage);

        ReadTextTable();
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

    public string GetText(int id)
    {
        return GetText(id, Global.currentLanguage);
    }

    public string GetText(int id, Language language)
    {
        return textTable[id][(int)language];
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

    public DialogueData GetDialogueData(int id)
    {
        return GetData<DialogueData>("Dialogue", id);
    }


    private void ReadTextTable()
    {
        string jsonString = File.ReadAllText(Application.dataPath + "/StreamingAssets" + "/Text.json");
        string fixedJsonString = JsonHelper.fixJson(jsonString);


        foreach(TextData data in JsonHelper.FromJson<TextData>(fixedJsonString))
            textTable.Add(data.Id, new string[] { data.English });
    }
}

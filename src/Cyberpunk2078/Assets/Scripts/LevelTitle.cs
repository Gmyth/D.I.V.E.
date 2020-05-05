using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "fileName.asset", menuName = "DataTable/Level Title")]
public class LevelTitle : ScriptableObject
{
    [System.Serializable]
    public class LevelEntry
    {
        public int id;
        public string name;
    }

    public List<LevelEntry> SerializedEntries;

}

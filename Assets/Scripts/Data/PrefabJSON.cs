using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://forum.unity.com/threads/how-to-read-json-file.401306/
namespace JSON
{
    [System.Serializable]
    public class PrefabJSON
    {
        public LevelPrefabJSON[] mapData;

    }

    [System.Serializable]
    public class LevelPrefabJSON
    {
        public RoomPrefabJSON[] levelData;
    }

    [System.Serializable]
    public class RoomPrefabJSON
    {
        public float[] percents;
        public string[] prefabs;
    }
}

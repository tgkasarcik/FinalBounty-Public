using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JSON
{
    public class JSONReader
    {
        // https://forum.unity.com/threads/how-to-read-json-file.401306/
        readonly private string fileName = "Text/MapPrefabs";
        private TextAsset jsonFile;
        private LevelPrefabData[][] Data;
        public JSONReader()
        {
            jsonFile = Resources.Load<TextAsset>(fileName);
            populateData();
        }
        private void populateData()
        {
            PrefabJSON MapData = JsonUtility.FromJson<PrefabJSON>(jsonFile.text);
            Data = new LevelPrefabData[MapData.mapData.Length][];
            for (int i = 0; i < Data.Length; i++)
            {

                RoomPrefabJSON[] current = MapData.mapData[i].levelData;
                LevelPrefabData[] newArray = new LevelPrefabData[current.Length];
                for (int j = 0;j < current.Length; j++)
                {
                    newArray[j] = new LevelPrefabData(current[j].percents, current[j].prefabs);
                }
                Data[i] = newArray;
    
            }
        }
        public LevelPrefabData[][] getData()
        {
            return Data;
        }

        

        
    
    }
    public struct LevelPrefabData
    {
        public float[] percents;
        public string[] prefabs;

        public LevelPrefabData(float[] percents, string[] prefabs)
        {
            this.percents = percents;
            this.prefabs = prefabs;
        }
    }
}
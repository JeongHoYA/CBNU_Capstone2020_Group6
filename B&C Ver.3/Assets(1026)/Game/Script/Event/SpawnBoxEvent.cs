using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnBoxEvent : MonoBehaviour
{

    private static SpawnBoxEvent _instance;
    public static SpawnBoxEvent Instance { get { return _instance; } }


    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            World.Instance.GetChunkFromVector3(transform.position).EditVoxelMap(transform.position, 0);
        }
        else
        {
            _instance = this;

            if (World.Instance.isBuildMode == true)
            {
                Vector3Int pos = Vector3Int.FloorToInt(transform.position);
                World.Instance.settings.cpX = pos.x;
                World.Instance.settings.cpY = pos.y;
                World.Instance.settings.cpZ = pos.z;

                World.Instance.settings.isCanBeCleared = true;

                string jsonExport = JsonUtility.ToJson(World.Instance.settings);
                Debug.Log("Setting Change to : " + jsonExport);
                File.WriteAllText(World.Instance.mapFileLocation + World.Instance.mapName + " settings.cfg", jsonExport);

                World.Instance.isSettingsChanged = true;
                World.Instance.SaveWorld();

                World.Instance.hasSpawnBox = true;
            }
        }          
    }

    private void OnDestroy()
    {
        World.Instance.hasSpawnBox = false;
        World.Instance.settings.isCanBeCleared = false;
    }
}

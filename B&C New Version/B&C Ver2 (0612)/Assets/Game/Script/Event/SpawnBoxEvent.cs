using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnBoxEvent : MonoBehaviour
{
    void Start()
    {
        Vector3Int pos = Vector3Int.FloorToInt(transform.position);
        World.Instance.settings.cpX = pos.x;
        World.Instance.settings.cpY = pos.y;
        World.Instance.settings.cpZ = pos.z;

        string jsonExport = JsonUtility.ToJson(World.Instance.settings);
        Debug.Log("Setting Change to : " + jsonExport);
        File.WriteAllText(World.Instance.mapFileLocation + World.Instance.mapName + " settings.cfg", jsonExport);

        World.Instance.isSettingsChanged = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ClearBoxEvent : MonoBehaviour
{
    private static ClearBoxEvent _instance;
    public static ClearBoxEvent Instance { get { return _instance; } }

    // Start is called before the first frame update
    void Start()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            World.Instance.GetChunkFromVector3(transform.position).EditVoxelMap(transform.position, 0);
        }
        else
        {
            _instance = this;

            if (World.Instance.isBuildMode == true)
            {
                World.Instance.SaveWorld();

                World.Instance.settings.isCanBeCleared = true;
                World.Instance.hasClearBox = true;
            }
        }
    }

    private void OnDestroy()
    {
        World.Instance.hasClearBox = false;
        World.Instance.settings.isCanBeCleared = false;
    }
}

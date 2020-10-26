using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBox : MonoBehaviour
{
    public MeshRenderer mesh;
    public BoxCollider col;

    Color c;

    public float timer;
    float time;
    public bool enableAtStart;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;

        if (!World.Instance.isBuildMode)
        {
            col.enabled = enableAtStart;

            c = mesh.material.color;

            if (enableAtStart)
                mesh.material.color = changeAlpha(c, 1f);
            else
                mesh.material.color = changeAlpha(c, 0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (World.Instance.isBuildMode)
            return;
        
        time += Time.deltaTime;

        if (time > timer)
        {
            enableAtStart = !enableAtStart;

            col.enabled = enableAtStart;

            if (enableAtStart)
                mesh.material.color = changeAlpha(c, 1f);
            else
                mesh.material.color = changeAlpha(c, 0.2f);

            time = 0;
        }
    }

    Color changeAlpha(Color color, float newAlpha)
    {
        color.a = newAlpha;
        return color;
    }
}

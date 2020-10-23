using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloudblock : MonoBehaviour
{
    public MeshRenderer rend;
    Color c;

    public float duration;
    float t;

    bool playertouch;
    
    // Start is called before the first frame update
    void Start()
    {
        if(!World.Instance.isBuildMode)
        {
            c = rend.material.color;
            t = 0;
            playertouch = false;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        if (World.Instance.isBuildMode)
            return;

        if (playertouch)
        {
            t = t + Time.deltaTime;
            float lerp = 1 - Mathf.Clamp(t, 0, duration) / duration;

            rend.material.color = changeAlpha(c, lerp);

            if (t > duration)
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }

    Color changeAlpha(Color color, float newAlpha)
    {
        color.a = newAlpha;
        return color;
    }

    private void OnTriggerEnter(Collider collision)
    {
        CharacterStat n = collision.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n != null)
        {
            playertouch = true;
        }
    }
}

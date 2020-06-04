using UnityEngine;

public class DotDamageBoxEvent : MonoBehaviour
{
    public int damage = 1;
    public float time = 1f;
    

    private void OnTriggerStay(Collider other)
    {
        CharacterStat n = other.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n != null)
        {
            n.isEnterDotDamageZone = true;
            n.dotdamage = damage;
            n.dotTick = time;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterStat n = other.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n != null)
        { 
            n.isEnterDotDamageZone = false;
            n.dotdamage = 0;
            n.dotTick = 0;
        }
    }
}

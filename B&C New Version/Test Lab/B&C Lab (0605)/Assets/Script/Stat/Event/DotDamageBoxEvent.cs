using UnityEngine;

public class DotDamageBoxEvent : MonoBehaviour
{
    public int damage = 0;
    public float time = 1f;
    

    private void OnTriggerStay(Collider other)
    {
        CharacterStat n = other.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n != null)
        {
            if (damage == 0)
                return;
            else if (damage > 0)
            {
                n.isEnterDotDamageZone = true;
                n.dotdamage = damage;
                n.dotTick = time;
            }
            else
            {
                n.isEnterDotHealZone = true;
                n.dotdamage = damage;
                n.dotTick = time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterStat n = other.transform.root.gameObject.GetComponent<CharacterStat>();
        if (n != null)
        {
            if (damage == 0)
                return;
            else if (damage > 0)
            {
                n.isEnterDotDamageZone = false;
                n.dotdamage = 0;
                n.dotTick = 0;
            }
            else
            {
                n.isEnterDotHealZone = false;
                n.dotdamage = 0;
                n.dotTick = 0;
            }
        }
    }
}

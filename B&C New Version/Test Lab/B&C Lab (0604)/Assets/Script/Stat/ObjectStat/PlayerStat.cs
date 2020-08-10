using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : CharacterStat
{
    public CharController charController;
    public GameObject restartPanel;


    public override void Die()
    {
        base.Die();
        Vector3 pos = transform.position;
        transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, 1f, 0), 0.5f);
        Invoke("RestartPanel", 3f);
    }

    private void RestartPanel()
    {
        restartPanel.SetActive(true);
    }
}

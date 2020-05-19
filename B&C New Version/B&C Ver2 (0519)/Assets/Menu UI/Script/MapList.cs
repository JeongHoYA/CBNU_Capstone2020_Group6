using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapList : MonoBehaviour
{
    public GameObject buttonPrefab;
    public TextMeshProUGUI text;


    void Start()
    {
        text.text = buttonPrefab.name.ToString();
        buttonPrefab.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(buttonPrefab.name));
    }

    public void OnButtonClick(string name)
    {
        Debug.Log(name);
    }
}

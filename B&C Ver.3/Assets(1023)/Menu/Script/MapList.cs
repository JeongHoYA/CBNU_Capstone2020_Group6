using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapList : MonoBehaviour
{
    public Color color;
    public Color clickedColor;
    public GameObject buttonPrefab;
    public TextMeshProUGUI text;


    void Start()
    {
        text.text = name.ToString();
        buttonPrefab.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(buttonPrefab.name)); 
    }

    public void OnButtonClick(string name)
    {
        Debug.Log(name + " Map Selected");
        MainMenu.nameFromMainScene = name;

        GetComponent<Image>().color = clickedColor;
        Invoke("changeColor", 0.5f);
    }

    public void changeColor()
    {
        GetComponent<Image>().color = color;
    }
}

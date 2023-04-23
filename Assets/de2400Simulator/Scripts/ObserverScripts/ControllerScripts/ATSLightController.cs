using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATSLightController : MonoBehaviour
{
    public List<IButtonObserver> buttons;
    public List<GameObject> atsLightsList;
    void Start()
    {
        foreach (var item in atsLightsList)
        {
            item.SetActive(false);
        }

        foreach (var item in buttons)
        {
            item.OnOpen = OnOpen;
            item.OnClose = OnClose;
        }
    }
    private void OnOpen(string buttonName)
    {
        //First light
        if (buttonName == "MainLightSwitch")
        {
            for (int i = 0; i < atsLightsList.Count; i++)
            {
                atsLightsList[i].SetActive(true);
            }
        }

    }

    private void OnClose(string buttonName)
    {
        //First light
        if (buttonName == "MainLightSwitch")
        {
            for (int i = 0; i < atsLightsList.Count; i++)
            {
                atsLightsList[i].SetActive(false);
            }
        }
    }
}

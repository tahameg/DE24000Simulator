using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsolLightsController : MonoBehaviour
{
    public List<IButtonObserver> buttons;
    public List<GameObject> consolLightsList;
    void Start()
    {

        foreach (var item in consolLightsList)
        {
            item.SetActive(false);
        }
        foreach (var item in buttons)
        {
            item.OnOpen = OnOpen;
            item.OnClose = OnClose;
            item.OnKrankOneOn = OnKrankOneOn;
            item.OnKrankSecondOn = OnKrankSecondOn;
            item.OnKrankOneOff = OnKrankOneOff;
            item.OnKrankSecondOff = OnKrankSecondOff;
        }
    }

    private void OnKrankOneOn(string obj)
    {
        consolLightsList[0].SetActive(false);
    }
    private void OnKrankSecondOn(string obj)
    {
        consolLightsList[1].SetActive(false);
    }
    private void OnKrankOneOff(string obj)
    {
        consolLightsList[0].SetActive(true);
    }

    private void OnKrankSecondOff(string obj)
    {
        consolLightsList[1].SetActive(true);
    }

    private void OnOpen(string buttonName)
    {
        //First light
        if(buttonName == "MainLightSwitch")
        {
            for(int i =0; i<4;  i++)
            {
                consolLightsList[i].SetActive(true);
            }
        } 
        //Engine lightOpen
        if(buttonName == "EngineStartButton")
        {
            for (int i = 4; i < 6; i++)
            {
                consolLightsList[i].SetActive(true);
            }
        }


    }

    private void OnClose(string buttonName)
    {
        //Engine Opened Full and off light
        if (buttonName == "EngineStartButton")
        {
            consolLightsList[2].SetActive(false);
            for (int i = 4; i < 6; i++)
            {
                consolLightsList[i].SetActive(false);
            }
        }
    }
}

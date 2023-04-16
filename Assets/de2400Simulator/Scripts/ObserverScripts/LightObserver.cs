using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightObserver : MonoBehaviour
{
    public List<IButtonObserver> buttons;
    private void Start()
    {
        foreach (var item in buttons)
        {
            item.OnOpen = OnOpen;
            item.OnClose = OnClose;
        }
    }

    private void OnOpen(string buttonName)
    {
        
    }

    private void OnClose(string buttonName)
    {
        
    }
}

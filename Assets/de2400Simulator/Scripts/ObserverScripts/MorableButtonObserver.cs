using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorableButtonObserver : IButtonObserver
{
    public void Start()
    {
        base.Start();
    }
    public override void OnValueChange(float buttonValue)
    {
        if (buttonValue >= 30)
        {
            OnOpen(buttonName);
        }
        if (buttonValue <= 5)
        {
            OnClose(buttonName);
        }
    }
}


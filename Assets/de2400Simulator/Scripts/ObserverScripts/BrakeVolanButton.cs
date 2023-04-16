using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeVolanButton :  IButtonObserver
{
    private void Start()
    {
        base.Start();
    }
    public override void OnValueChange(float buttonValue)
    {

        if (buttonValue >= 90)
        {
            OnOpen(buttonName);
        }
        if (buttonValue <= 5)
        {
            OnClose(buttonName);
        }
    }
}

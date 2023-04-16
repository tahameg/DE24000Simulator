using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IButtonObserver: MonoBehaviour
{
    //public GameObject subscribedButton;
    public string buttonName;
    public Action<string> OnOpen;
    public Action<string> OnClose;
    
    public void Start()
    {
        //if (subscribedButton != null)
        gameObject.GetComponent<RotationProcessorBase>().ValueChanged.AddListener(OnValueChange);
    }
    public abstract void OnValueChange(float buttonValue);
}

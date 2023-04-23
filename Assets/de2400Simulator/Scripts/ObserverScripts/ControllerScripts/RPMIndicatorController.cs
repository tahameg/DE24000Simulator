using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPMIndicatorController : MonoBehaviour
{
    public List<IButtonObserver> buttons;
    private Quaternion wantedRotation;
    public float rotateSpeed = 10;
    bool eventCalled = false;

    private void Start()
    {
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
        wantedRotation = Quaternion.Euler(0, 30, 0);
        eventCalled = true;
    }
    private void OnKrankSecondOn(string obj)
    {
        wantedRotation = Quaternion.Euler(0, 60, 0);
        eventCalled = true;
    }
    private void OnKrankOneOff(string obj)
    {
        wantedRotation = Quaternion.Euler(0, -30, 0);
        eventCalled = true;
    }

    private void OnKrankSecondOff(string obj)
    {
        wantedRotation = Quaternion.Euler(0, -60, 0);
        eventCalled = true;
    }

    private void Update()
    {
        if (eventCalled)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, wantedRotation, Time.deltaTime * rotateSpeed);
            if (transform.rotation.y == wantedRotation.y)
                eventCalled = false;
        }
    }

    private void OnOpen(string buttonName)
    {   
        eventCalled = true;
    }

    private void OnClose(string buttonName)
    {
        wantedRotation = Quaternion.Euler(0, -60, 0);
            eventCalled = true;
    }


}

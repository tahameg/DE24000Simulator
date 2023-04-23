using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakePressIndicatorController : MonoBehaviour
{
    public List<IButtonObserver> buttons;
    private Quaternion currentRotation;
    private Quaternion wantedRotation;
    public float rotateSpeed = 10;
    bool isOpen= false, isClosed = false;

    private void Start()
    {
        foreach (var item in buttons)
        {
            item.OnOpen = OnOpen;
            item.OnClose = OnClose;
        }
    }

    private void Update()
    {
       if(isOpen || isClosed)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, wantedRotation, Time.deltaTime * rotateSpeed);
            if(transform.rotation.y == wantedRotation.y)
            {
                isOpen = false;
                isClosed = false;
            }
        }

    }

    private void OnOpen(string buttonName)
    {
        wantedRotation = Quaternion.Euler(0, 60, 0);
        isOpen = true;
    }

    private void OnClose(string buttonName)
    {
        wantedRotation = Quaternion.Euler(0, -60, 0);
        isClosed = true;
    }
}

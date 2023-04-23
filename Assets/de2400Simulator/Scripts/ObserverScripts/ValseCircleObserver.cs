using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValseCircleObserver : IButtonObserver
{
    public GameObject subscribedButtons;
    bool IsGasOpen = false;
    bool IsFirstKrank = false;
    bool IssecondKrank = false;
    float pivotAngle = 0f;
    float firstAngle = 0f;
    bool firstAngleTaken = true;
    public string buttonName;

    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnValueChange(float buttonValue)
    {
        if(firstAngleTaken)
        {
            firstAngle = buttonValue;
            firstAngleTaken = false;
        }
        if ((buttonValue- firstAngle) >= 90)
        {
            IsGasOpen = true;
            pivotAngle = 90f;  
        }

        if ((buttonValue- firstAngle) >= 105 && IsGasOpen)
        {
            IsFirstKrank = true;
            OnKrankOneOn(buttonName);
        }

        if ((buttonValue - firstAngle) >= 105 && IsGasOpen && IsFirstKrank)
        {
            IssecondKrank = true;
            OnKrankSecondOn(buttonName);
        }

    }

}

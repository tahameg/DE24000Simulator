using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TakeControllerData : MonoBehaviour
{
    public ActionBasedController actionBasedControllerLeft;
    public ActionBasedController actionBasedControllerRight;
    public static bool triggerPressedLeft, triggerPressedRight, triggerReleasedLeft, triggerReleasedRight;
    void Start()
    {
        actionBasedControllerLeft.activateAction.action.performed += TriggerPressedLeft;
        actionBasedControllerRight.activateAction.action.performed += TriggerPressedRight;
    }

    private void TriggerPressedLeft(InputAction.CallbackContext obj)
    {
        triggerPressedLeft = true;
    }
    private void TriggerPressedRight(InputAction.CallbackContext obj)
    {
        triggerPressedRight = true;
    }
    // Update is called once per frame
    void Update()
    {
       if(triggerPressedLeft || triggerPressedRight)
       {
            if (actionBasedControllerRight.activateAction.action.WasReleasedThisFrame())
                triggerPressedRight = false;

            if (actionBasedControllerLeft.activateAction.action.WasReleasedThisFrame())
                triggerPressedLeft = false;
       }

    }
}

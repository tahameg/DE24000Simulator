using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatkapTurn : MonoBehaviour
{
    public float turnSpeed = 700;
    public Transform matkapButton;
    Vector3 matkapButtonFirstPos;
    Vector3 matkapButtonSecondPos;
    bool released;
    // Update is called once per frame
    private void Start()
    {
        matkapButtonFirstPos = matkapButton.localPosition;
        matkapButtonSecondPos = new Vector3(matkapButton.localPosition.x + 0.007f, matkapButton.localPosition.y, matkapButton.localPosition.z);
    }
    void Update()
    {
        if (TakeControllerData.triggerPressedLeft || TakeControllerData.triggerPressedRight)
        {
            //Matkap Turn
            gameObject.transform.Rotate(Vector3.right * turnSpeed * Time.deltaTime);
            //Matkap Button Visual
            matkapButton.localPosition = matkapButtonSecondPos;
            released = true;
        }
        else if(released)
        {
            released = false;
            matkapButton.localPosition = matkapButtonFirstPos;

        }
    }
}

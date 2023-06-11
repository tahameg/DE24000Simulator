using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScrewController : MonoBehaviour
{
    Vector3 screwFirstPos;
    Vector3 screwSecondPos;
    public bool isOut;
    public UnityEvent OnScrewIsOut;

    private void Start()
    {
        screwFirstPos = gameObject.transform.localPosition;
        screwSecondPos = new Vector3(gameObject.transform.localPosition.x + 0.03f, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "TCDD_24000_Matkap_Cevirme" && (TakeControllerData.triggerPressedLeft || TakeControllerData.triggerPressedRight))
        {
            if(gameObject.transform.localPosition == screwFirstPos)
            {
                gameObject.transform.localPosition = screwSecondPos;
                isOut = true;
                OnScrewIsOut.Invoke();
            }
                

            //if (gameObject.transform.localPosition == screwSecondPos)
            //{
            //    isOut = false;
            //    gameObject.transform.localPosition = screwFirstPos;
            //}
                
        }
    }
}

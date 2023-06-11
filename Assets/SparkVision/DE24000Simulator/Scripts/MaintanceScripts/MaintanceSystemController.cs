using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class MaintanceSystemController : MonoBehaviour
{
    public List<ScrewController> screwControllerList;
    public GameObject screwCoverGO;
    XRGrabInteractable screwCoverXRGrabInteractiable;
    public int outScrewCount = 0;
    bool ScrewCoverGrabEnable;


    void Start()
    {
        screwCoverXRGrabInteractiable = screwCoverGO.GetComponent<XRGrabInteractable>();
        screwCoverXRGrabInteractiable.enabled = false;

        foreach (var item in screwControllerList)
            item.OnScrewIsOut.AddListener(OnScrewIsOut);
    }
    public void ScrewCoverSelected()
    {
        foreach (var item in screwControllerList)
            item.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(ScrewCoverGrabEnable)
        {
            screwCoverGO.GetComponent<Rigidbody>().isKinematic = false;
            screwCoverXRGrabInteractiable.enabled = true;
            ScrewCoverGrabEnable = false;
        }
    }

    void OnScrewIsOut()
    {
        outScrewCount++;
        if (outScrewCount >= 4)
            ScrewCoverGrabEnable = true;
    }
}

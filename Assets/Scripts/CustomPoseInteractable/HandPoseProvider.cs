using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Katana.XR.Interactables.HandPoseSystem.Utils;

namespace Katana.XR.Interactables.HandPoseSystem
{
    public class HandPoseProvider : MonoBehaviour
    {   
        [Header("Temporary Settings")]
        [SerializeField]
        XRGrabInteractable m_grabInteractable;

        [Header("Mesh Settings")]
        public GameObject RightHandPrefab;
        public GameObject LeftHandPrefab;

        [SerializeField]
        HandStructuralInfo m_leftHandInfo;
        [SerializeField]
        HandStructuralInfo m_rightHandInfo;

        public void AutoGenerateHandConfigurations()
        {
            string[] fingerPrefixes = new string[] { "index", "middle", "ring", "pinky", "thumb" };
            if(LeftHandPrefab != null)
            {
                m_leftHandInfo = HandUtils.AutoGenerateConfiguration(LeftHandPrefab, fingerPrefixes);
            }
            
            if(RightHandPrefab != null)
            {
                m_rightHandInfo = HandUtils.AutoGenerateConfiguration(RightHandPrefab, fingerPrefixes);
            }
        }

    }
}

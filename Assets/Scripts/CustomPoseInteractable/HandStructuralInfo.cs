using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Katana.XR.Interactables.HandPoseSystem
{
    [Serializable]
    public class HandStructuralInfo : MonoBehaviour
    {
        public GameObject HandObject => m_handObject; 

        [SerializeField]
        GameObject m_handObject;
        public bool IsInitialized => m_isAwake ? m_isInitialized : GetIsInitialized();
        bool m_isInitialized;
        bool m_isAwake;

        [Header("Hand Transforms", order = 0)]
        public List<Transform> IndexFingerTransforms;
        public List<Transform> MiddleFingerTransforms;
        public List<Transform> RingFingerTransforms;
        public List<Transform> PinkyFingerTransforms;
        public List<Transform> ThumbTransforms;

        private void Awake()
        {
            m_isInitialized = GetIsInitialized();
            m_isAwake = true;
            Debug.Log(" awaken!! ");
            if (!m_isInitialized)
            {
                Debug.LogWarning("All fingers should be assigned");
            }
        }

        bool GetIsInitialized()
        {
            return (IndexFingerTransforms != null)
                && (MiddleFingerTransforms != null)
                && (RingFingerTransforms != null)
                && (PinkyFingerTransforms != null)
                && (ThumbTransforms != null);
        }
    }
}


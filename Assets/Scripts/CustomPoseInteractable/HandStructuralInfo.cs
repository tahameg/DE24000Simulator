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
        public bool IsInitialized { get { return m_isInitialized; } }
        bool m_isInitialized;

        [Header("Hand Transforms", order = 0)]
        public List<Transform> IndexFingerTransforms;
        public List<Transform> MiddleFingerTransforms;
        public List<Transform> RingFingerTransforms;
        public List<Transform> PinkyFingerTransforms;
        public List<Transform> ThumbTransforms;

        private void Awake()
        {
            m_isInitialized = (IndexFingerTransforms != null)
                && (MiddleFingerTransforms != null)
                && (RingFingerTransforms != null)
                && (PinkyFingerTransforms != null)
                && (ThumbTransforms != null);

            if (!m_isInitialized)
            {
                Debug.LogWarning("All fingers should be assigned");
            }
        }
    }
}


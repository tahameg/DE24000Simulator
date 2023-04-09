using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Katana.XR.Interactables.HandPoseSystem.Data
{
    [CreateAssetMenu(fileName = "NewHandPoseData", menuName = "Katana/Data/HandRecord")]
    [Serializable]
    public class HandRecord : ScriptableObject
    {
        public string structuralHash;
        public List<Quaternion> IndexFingerRecords;
        public List<Quaternion> MiddleFingerRecords;
        public List<Quaternion> RingFingerRecords;
        public List<Quaternion> PinkyFingerRecords;
        public List<Quaternion> ThumbRecords;
    }
}

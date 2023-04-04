using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Katana.XR.Interactables.HandPoseSystem.Data;

namespace Katana.XR.Interactables.HandPoseSystem
{
    [RequireComponent(typeof(HandPoseOperator))]
    public class HandPoseRecorder : MonoBehaviour
    {
        [Tooltip("This is folder of the path that the hand poses will be saved on. This is a relative path")]
        public string DataSaveDirectory = "Katana/Interaction/Data/HandPose";
        public string DataSavePrefix = "HandPose_";
        HandPoseOperator m_handPoseOperator;
        public HandPoseOperator HandPoseOperator => 
            m_handPoseOperator == null ? GetComponent<HandPoseOperator>() : m_handPoseOperator;

        private void Awake()
        {
            m_handPoseOperator = GetComponent<HandPoseOperator>();
        }
        public HandRecord GetHandRecord()
        {
            if (!HandPoseOperator.IsInitialized) return null;
            HandRecord record = new HandRecord();
            record.IndexFingerRecords = GetFingerRotations(HandPoseOperator.StructuralInfo.IndexFingerTransforms);
            record.MiddleFingerRecords = GetFingerRotations(HandPoseOperator.StructuralInfo.MiddleFingerTransforms);
            record.RingFingerRecords = GetFingerRotations(HandPoseOperator.StructuralInfo.RingFingerTransforms);
            record.PinkyFingerRecords = GetFingerRotations(HandPoseOperator.StructuralInfo.PinkyFingerTransforms);
            record.ThumbRecords = GetFingerRotations(HandPoseOperator.StructuralInfo.ThumbTransforms);
            return record;
        }

        List<Quaternion> GetFingerRotations(List<Transform> transforms)
        {
            List<Quaternion> returnList = new List<Quaternion>();
            foreach(var t in transforms)
            {
                returnList.Add(t.localRotation);
            }
            return returnList;
        }
    }
}

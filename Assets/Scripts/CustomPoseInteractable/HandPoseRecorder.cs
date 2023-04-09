using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Katana.XR.Interactables.HandPoseSystem.Data;

namespace Katana.XR.Interactables.HandPoseSystem
{
    [RequireComponent(typeof(HandStructuralInfo))]
    public class HandPoseRecorder : MonoBehaviour
    {
        [Tooltip("This is folder of the path that the hand poses will be saved on. This is a relative path")]
        public string DataSaveDirectory = "Katana/Interaction/Data/HandPose";
        public string DataSaveName = "HandPose_";

        public List<HandRecord> TestHandRecords = new List<HandRecord>();

        HandStructuralInfo m_handStructuralInfo;
        HandPoseOperator m_handPoseOperator;
        public float transitionTime = 0.4f;
        public HandStructuralInfo HandStructuralInfo =>
            m_handStructuralInfo == null ? GetComponent<HandStructuralInfo>() : m_handStructuralInfo;

        public HandPoseOperator HandPoseOperator =>
            m_handPoseOperator == null ? GetComponent<HandPoseOperator>() : m_handPoseOperator;
        private void Awake()
        {
            m_handStructuralInfo = GetComponent<HandStructuralInfo>();
            m_handPoseOperator = GetComponent<HandPoseOperator>();
        }

        public HandRecord GetHandRecord()
        {
            if (!HandStructuralInfo.IsInitialized) return null;
            HandRecord record = ScriptableObject.CreateInstance<HandRecord>();
            record.structuralHash = HandStructuralInfo.GetStructureHash();
            record.IndexFingerRecords = GetFingerRotations(HandStructuralInfo.IndexFingerTransforms);
            record.MiddleFingerRecords = GetFingerRotations(HandStructuralInfo.MiddleFingerTransforms);
            record.RingFingerRecords = GetFingerRotations(HandStructuralInfo.RingFingerTransforms);
            record.PinkyFingerRecords = GetFingerRotations(HandStructuralInfo.PinkyFingerTransforms);
            record.ThumbRecords = GetFingerRotations(HandStructuralInfo.ThumbTransforms);
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

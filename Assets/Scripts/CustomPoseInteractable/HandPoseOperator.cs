using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Katana.XR.Interactables.HandPoseSystem.Data;

namespace Katana.XR.Interactables.HandPoseSystem
{
    [RequireComponent(typeof(HandStructuralInfo))]
    public class HandPoseOperator : MonoBehaviour
    {
        public HandStructuralInfo StructuralInfo => m_handStructuralInfo == null ?
            GetComponent<HandStructuralInfo>() : m_handStructuralInfo;
        [SerializeField]
        float m_frameDuration = 0.01f;
        HandStructuralInfo m_handStructuralInfo;

        public bool IsInitialized => StructuralInfo.IsInitialized;
        IEnumerator animationRoutine;

        private void Awake()
        {
            m_handStructuralInfo = GetComponent<HandStructuralInfo>();
        }

        public void ApplyRecord(HandRecord record)
        {
            if (!IsInitialized) return;
            if(StructuralInfo.GetStructureHash() != record.structuralHash)
            {
                Debug.Log(StructuralInfo.GetStructureHash());
                Debug.LogError("Hand record structure is not applicable to the given structural info.");
                return;
            }

            ApplyFingerRecord(StructuralInfo.IndexFingerTransforms, record.IndexFingerRecords, 1f);
            ApplyFingerRecord(StructuralInfo.MiddleFingerTransforms, record.MiddleFingerRecords, 1f);
            ApplyFingerRecord(StructuralInfo.RingFingerTransforms, record.RingFingerRecords, 1f);
            ApplyFingerRecord(StructuralInfo.PinkyFingerTransforms, record.PinkyFingerRecords, 1f);
            ApplyFingerRecord(StructuralInfo.ThumbTransforms, record.ThumbRecords, 1f);
        }


        public void ApplyRecord(HandRecord record, float animationDuration)
        {
            if (!IsInitialized) return;
            if (StructuralInfo.GetStructureHash() != record.structuralHash)
            {
                Debug.Log(StructuralInfo.GetStructureHash());
                Debug.LogError("Hand record structure is not applicable to the given structural info.");
                return;
            }
            float duration = Mathf.Clamp(animationDuration, 0, Mathf.Abs(animationDuration));
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }

            if(Application.IsPlaying(this))
            {
                animationRoutine = ApplyRecordRoutine(record, duration);
                Debug.Log("Duration is " + duration);
                StartCoroutine(animationRoutine);
            }
            else
            {
                ApplyFingerRecord(StructuralInfo.IndexFingerTransforms, record.IndexFingerRecords);
                ApplyFingerRecord(StructuralInfo.MiddleFingerTransforms, record.MiddleFingerRecords);
                ApplyFingerRecord(StructuralInfo.RingFingerTransforms, record.RingFingerRecords);
                ApplyFingerRecord(StructuralInfo.PinkyFingerTransforms, record.PinkyFingerRecords);
                ApplyFingerRecord(StructuralInfo.ThumbTransforms, record.ThumbRecords);
            }
        }

        bool ApplyFingerRecord(List<Transform> targetTransforms, List<Quaternion> sourceData, float lerpRatio = 1f)
        {
            float t = Mathf.Clamp01(lerpRatio);
            if (targetTransforms == null || sourceData == null || t == 0) return false;
            if(targetTransforms.Count != sourceData.Count)
            {
                Debug.LogError("The targetTransforms and sourceData must be of same size.");
                return false;
            }

            for(int i = 0; i < targetTransforms.Count; i++)
            {
                targetTransforms[i].localRotation = Quaternion.Lerp(targetTransforms[i].localRotation, sourceData[i], t);
            }
            return true;
        }

        IEnumerator ApplyRecordRoutine(HandRecord record, float duration)
        {
            float stepsLeft = Mathf.Ceil(duration / m_frameDuration);
            float waitTime = stepsLeft == 1f ? 0f : m_frameDuration;
            float totalWait = 0.0f;
            while(stepsLeft > .0f)
            {
                ApplyFingerRecord(m_handStructuralInfo.IndexFingerTransforms, record.IndexFingerRecords, 1f / stepsLeft);
                ApplyFingerRecord(m_handStructuralInfo.MiddleFingerTransforms, record.MiddleFingerRecords, 1f / stepsLeft);
                ApplyFingerRecord(m_handStructuralInfo.RingFingerTransforms, record.RingFingerRecords, 1f / stepsLeft);
                ApplyFingerRecord(m_handStructuralInfo.PinkyFingerTransforms, record.PinkyFingerRecords, 1f / stepsLeft);
                ApplyFingerRecord(m_handStructuralInfo.ThumbTransforms, record.ThumbRecords, 1f / stepsLeft);
                stepsLeft -= 1f;
                waitTime = stepsLeft == 1f ? 0f : m_frameDuration;
                totalWait += m_frameDuration;
                yield return new WaitForSeconds(waitTime);
            }
            Debug.Log("[Log] " + totalWait);
            animationRoutine = null;
        }
        
    }
}
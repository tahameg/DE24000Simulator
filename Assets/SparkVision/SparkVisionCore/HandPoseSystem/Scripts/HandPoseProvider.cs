using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SparkVision.HandPoseSystem
{
    public enum PoseActivationType
    {
        OnSelect,
        OnHover
    }
    public class HandPoseProvider : MonoBehaviour, IHandPoseHoverable, IHandPoseSelectable
    {
        [SerializeField]
        PoseActivationType m_activationType;

        public XRBaseInteractor CurrentlyHoveringInteractor { get; private set; }
        public XRBaseInteractor CurrentlySelectingInteractor { get; private set; }

        bool m_poseInteractionActive;
        ObjectHandPoseInfoBase m_selectedInfo;
        HandPoseProviderArgs m_currentlyOperatedArgs;
        Transform m_currentlyOperatedControllerTransform;
        Transform m_currentMockHandTransform;
        Vector3 m_referenceZeroVector;
        Quaternion m_lastRotation;


        HandRecord m_lastRecordBeforeEnter;

        [SerializeField]
        HandRecord m_leftProximityPose;

        [SerializeField]
        HandRecord m_rightProximityPose;

        public List<ObjectHandPoseInfoBase> InteractionPoses;
        
        public void HandleHoverEnd(HandPoseProviderArgs args)
        {
            CurrentlyHoveringInteractor = null;
            if (m_activationType == PoseActivationType.OnHover)
            {
                
            }
        }

        public void HandleHoverStart(HandPoseProviderArgs args)
        {
            CurrentlyHoveringInteractor = args.DirectInteractor;
            if (m_activationType == PoseActivationType.OnHover)
            {
                //Operate Start
            }
            else
            {
                var proximityRecord = GetProximityRecord(args);
                if (proximityRecord != null)
                    args.HandPoseOperator.ApplyRecord(proximityRecord, 0.1f);
            }
        }

        HandRecord GetProximityRecord(HandPoseProviderArgs args)
        {
            return args.Handedness == Handedness.Left 
                ? m_leftProximityPose : m_rightProximityPose;
        }

        ObjectHandPoseInfoBase GetInteractionPose(HandPoseProviderArgs args)
        {
            Transform handTransform = args.HandPoseOperator.HandObject.transform.GetChild(0);
            ObjectHandPoseInfoBase selected = null;
            float evaluation = float.PositiveInfinity;
            foreach (var info in InteractionPoses)
            {
                if (info.Handedness != args.Handedness) continue;
                float newEvaluation = info.EvaluateDistance(transform, handTransform);
                if (newEvaluation < evaluation)
                {
                    selected = info;
                    evaluation = newEvaluation;
                }
            }

            return selected;
        }
        public void HandleSelectEnd(HandPoseProviderArgs args)
        {
            CurrentlySelectingInteractor = null;
            if (m_activationType == PoseActivationType.OnHover) return;
            Release(args);
        }

        public void HandleSelectStart(HandPoseProviderArgs args)
        {
            CurrentlySelectingInteractor = args.DirectInteractor;
            if (m_activationType == PoseActivationType.OnHover) return;
            Operate(args);

        }

        public HandRecord GetHandRecord(HandStructuralInfo HandStructuralInfo)
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
            foreach (var t in transforms)
            {
                returnList.Add(t.localRotation);
            }
            return returnList;
        }
        void Operate(HandPoseProviderArgs args)
        {
            if (HandRecordModeManager.IsRecordModeActive) return;
            args.HandPoseOperator.HandObject.SetActive(false);
            m_selectedInfo = GetInteractionPose(args);
            if (m_selectedInfo == null) return;

            if(m_selectedInfo is ConstantPointHandPoseInfo)
            {
                ConstantPointHandPoseInfo info = m_selectedInfo as ConstantPointHandPoseInfo;
                if (!info.CanRotateAroundAxis) return;

                Vector3 perpVector = new Vector3();
                if ((info.RotationAxis.normalized != Vector3.up) || (info.RotationAxis.normalized != (-Vector3.up)))
                    perpVector = Vector3.up;
                else perpVector = Vector3.right;
                m_referenceZeroVector = Vector3.Cross(perpVector, info.RotationAxis).normalized;
            }

            m_currentlyOperatedArgs = args;
            m_currentlyOperatedControllerTransform = args.HandPoseOperator.HandObject.transform.GetChild(0);
            m_lastRotation = m_currentlyOperatedControllerTransform.rotation;
            var op = MockHandsReferenceHolder.Instance.BringHand(args.Handedness, transform);

            m_currentMockHandTransform = op.transform.GetChild(0);
            op.ApplyRecord(m_selectedInfo.HandRecord, 0.1f);
            m_currentMockHandTransform.localPosition = m_selectedInfo.GetAnchorPosition(transform, args.HandPoseOperator.transform);
            m_currentMockHandTransform.localRotation = m_selectedInfo.GetAnchorRotation(transform, args.HandPoseOperator.transform);
            m_poseInteractionActive = true;
        }

        void Release(HandPoseProviderArgs args)
        {
            if (HandRecordModeManager.IsRecordModeActive) return;
            args.HandPoseOperator.HandObject.SetActive(true);
            MockHandsReferenceHolder.Instance.ClearHand(args.Handedness);
            m_selectedInfo = null;
            m_currentlyOperatedArgs = null;
            m_poseInteractionActive = false;
            m_currentlyOperatedControllerTransform = null;
        }

        Vector3 GetWorldAxis(Transform transform, Vector3 rotationAxisLocal)
        {
            return transform.TransformVector(rotationAxisLocal);
        }

        void HandleConstantRotationFreedom(ConstantPointHandPoseInfo info)
        {
            if (m_currentlyOperatedArgs == null) return;
            if (m_currentlyOperatedControllerTransform == null) return;
            if (m_currentMockHandTransform == null) return;

            Quaternion rotationDiff = m_currentlyOperatedControllerTransform.rotation * Quaternion.Inverse(m_lastRotation);
            Vector3 rotationAxis = GetWorldAxis(transform, info.RotationAxis);
            Vector3 referenceVector = GetWorldAxis(transform, m_referenceZeroVector);
            Vector3 rotatedReference = (rotationDiff * referenceVector).normalized;
            Vector3 projectedRotatedReference = Vector3.ProjectOnPlane(rotatedReference, rotationAxis).normalized;
            Quaternion rotation = Quaternion.FromToRotation(referenceVector, projectedRotatedReference);
            RotateAround(m_currentMockHandTransform, m_currentMockHandTransform.TransformPoint(info.HandPivotPosition), rotation);
            m_lastRotation = m_currentlyOperatedControllerTransform.rotation;
        }

        void RotateAround(Transform handTransform, Vector3 pivotPoint, Quaternion rot)
        {
            handTransform.position = rot * (handTransform.position - pivotPoint) + pivotPoint;
            handTransform.rotation = rot * handTransform.rotation;
        }
        private void Update()
        {
            if (m_selectedInfo == null) return;

            if(m_selectedInfo is ConstantPointHandPoseInfo info)
            {
                if (!info.CanRotateAroundAxis) return;
                HandleConstantRotationFreedom(info);
            }

            if(m_selectedInfo is FloatingCircleHandPoseInfo)
            {

            }
        }
    }
}

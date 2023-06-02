using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SparkVision.HandPoseSystem
{
    [System.Serializable]
    public class ObjectHandPoseInfoBase : ScriptableObject
    {
        public Handedness Handedness;
        /// <summary>
        /// Holds up transforms of the fingers.
        /// </summary>
        public HandRecord HandRecord;

        /// <summary>
        /// Hand's Initial Pose (In interactable's local space.)
        /// </summary>
        public Pose InitialHandPose;


        /// <summary>
        /// This is the position where the pose is activated.
        /// (Respect to interactable's local space.)
        /// </summary>
        public virtual Vector3 GetAnchorPosition(Transform transform, Transform handTransform) => Vector3.zero;

        public virtual Quaternion GetAnchorRotation(Transform transform, Transform handTransform) => Quaternion.identity;

        /// <summary>
        /// This point is considered to be holding point of the transform.
        /// (Respect to the hand object)
        /// </summary>
        public Vector3 HandPivotPosition;

        public virtual float EvaluateDistance(Transform transform, Transform handTransform) => 0f;
    }
}

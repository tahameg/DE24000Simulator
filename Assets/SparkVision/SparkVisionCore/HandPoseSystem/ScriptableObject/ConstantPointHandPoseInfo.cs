using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SparkVision.HandPoseSystem
{
    public class ConstantPointHandPoseInfo : ObjectHandPoseInfoBase
    {
        [Header("Movement Freedom Parameters")]
        
        [Header("Angular Freedom")]
        public bool CanRotateAroundAxis;

        public Vector3 RotationAxis;
        public float MaxAngle;
        public float MinAngle;


        public override Vector3 GetAnchorPosition(Transform transform, Transform handTransform)
        {
            return InitialHandPose.position;
        }

        public override Quaternion GetAnchorRotation(Transform transform, Transform handTransform)
        {
            return InitialHandPose.rotation;
        }
        public override float EvaluateDistance(Transform transform, Transform handTransform)
        {
            float distance = Vector3.Distance(handTransform.position, InitialHandPose.position);
            Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * handTransform.rotation;
            float angle = Quaternion.Angle(relativeRotation, InitialHandPose.rotation);
            Debug.Log(angle);
            return angle * angle + distance;
        }
    }
}


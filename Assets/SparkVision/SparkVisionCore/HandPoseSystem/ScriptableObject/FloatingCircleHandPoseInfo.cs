using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SparkVision.HandPoseSystem
{
    public class FloatingCircleHandPoseInfo : ObjectHandPoseInfoBase
    {
        public float Radius;

        public Vector3 CenterOffset;
        public Vector3 UpVector;

        public override Vector3 GetAnchorPosition(Transform transform, Transform handTransform)
        {
            Vector3 newCenter = transform.position + CenterOffset;
            Vector3 realUp = transform.TransformVector(UpVector);
            Vector3 projectedNormalized = Vector3.ProjectOnPlane(handTransform.position - newCenter, realUp).normalized;
            return newCenter + projectedNormalized * Radius;
            
        }

        public override float EvaluateDistance(Transform transform, Transform handTransform)
        {
            Vector3 newCenter = transform.position + CenterOffset;
            Vector3 projected = Vector3.ProjectOnPlane(handTransform.position - newCenter, UpVector);
            float projectedLength = projected.magnitude;
            return Mathf.Abs(projectedLength - Radius);
        }
    }
}

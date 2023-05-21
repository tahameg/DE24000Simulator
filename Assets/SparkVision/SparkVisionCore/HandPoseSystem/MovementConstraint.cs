using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SparkVision.HandPoseSystem
{
    [Serializable]
    public class MovementConstraint
    {
        Vector3 ConstrainedAxis;

        public Quaternion FreezeRotation(Quaternion source)
        {
            Vector3 sampleVector = Vector3.forward;
            Vector3 rotatedVector = source * sampleVector;
            Vector3 projectedVector = Vector3.ProjectOnPlane(rotatedVector, ConstrainedAxis.normalized).normalized;
            Quaternion constrainedRotation = Quaternion.FromToRotation(sampleVector, projectedVector);
            return source * Quaternion.Inverse(constrainedRotation); 
        }

        public Vector3 FreezePosition(Vector3 source)
        {
            Vector3 toSubstract = Vector3.Project(source, ConstrainedAxis);
            return source - toSubstract;
        }
    }
}

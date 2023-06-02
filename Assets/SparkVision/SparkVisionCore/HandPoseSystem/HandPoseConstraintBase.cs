using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SparkVision.HandPoseSystem
{
    [System.Serializable]
    public abstract class HandPoseConstraintBase
    {

        public abstract Transform ApplyConstraint(Transform objectTransform, Transform sourceTransform, Transform targetTransform);
    }
}


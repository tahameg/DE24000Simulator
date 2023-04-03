using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Katana.XR.Interactables.HandPoseSystem
{
    [Serializable]
    public class HandStructuralInfo 
    {
        public FingerStructuralInfo[] FingerConfiguration;

        public HandStructuralInfo(FingerStructuralInfo[] fingerConfiguration)
            => FingerConfiguration = fingerConfiguration;

        public HandStructuralInfo() { }
    }

    [Serializable]
    public class FingerStructuralInfo
    {
        public InitializedTransform[] JointTransforms;

        public FingerStructuralInfo(InitializedTransform[] jointTransforms)
            => JointTransforms = jointTransforms;

        public FingerStructuralInfo() { }
    }

    [Serializable]
    public struct InitializedTransform
    {
        public Transform TargetTransform;
        public Pose InitialPose;

        public InitializedTransform(Transform transform, Pose initialPose)
        {
            TargetTransform = transform;
            InitialPose = initialPose;
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Katana.XR.Interactables.HandPoseSystem.Data;

namespace Katana.XR.Interactables.HandPoseSystem
{
    [Serializable]
    public class CustomHandPoseConfiguration
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 AttachPoint;
        public HandRecord HandRecord;

        public bool ApplyRotationConstraints;
        /// <summary>
        /// Defines the rotation constraints that will be applied to the hand prefab. 
        /// Rotation around the given axes will be frozen.
        /// The given values are respect to the interactable object.
        /// </summary>
        public List<MovementConstraint> RotationConstraints;

        public bool ApplyPositionConstraints;
        /// <summary>
        /// Defines the position constraints that will be applied to the hand prefab. 
        /// Rotation around the given axes will be frozen.
        /// The given values are respect to the interactable object.
        /// </summary>
        public List<MovementConstraint> PositionConstraints;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using SparkVision.HandPoseSystem.Utils;

namespace SparkVision.HandPoseSystem
{
    public class HandPoseProviderArgs
    {
        public Transform ControllerTransform;
        public Handedness Handedness;
        public HandPoseOperator HandPoseOperator;
        public XRDirectInteractor DirectInteractor;

        public HandPoseProviderArgs(Transform controllerTransform,
            Handedness handedness, 
            HandPoseOperator handPoseOperator,
            XRDirectInteractor directInteractor
            )
        {
            ControllerTransform = controllerTransform;
            Handedness = handedness;
            HandPoseOperator = handPoseOperator;
            DirectInteractor = directInteractor;
        }
    }
    public enum Handedness
    {
        Left,
        Right
    }

    public interface IHandPoseHoverable
    {
        XRBaseInteractor CurrentlyHoveringInteractor { get; }
        void HandleHoverStart(HandPoseProviderArgs args);
        void HandleHoverEnd(HandPoseProviderArgs args);
    }

    public interface IHandPoseSelectable
    {
        XRBaseInteractor CurrentlySelectingInteractor { get; }
        void HandleSelectStart(HandPoseProviderArgs args);
        void HandleSelectEnd(HandPoseProviderArgs args);
    }

    public interface IHandPoseActivateable
    {
        void HandleActivateStart(HandPoseProviderArgs args);
        void HandleActivateEnd(HandPoseProviderArgs args);
    }
}

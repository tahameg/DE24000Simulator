using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SparkVision.HandPoseSystem
{
    public enum PoseActivationType
    {
        OnHover,
        OnSelect
    }
    public class HandPoseProvider : MonoBehaviour, IHandPoseHoverable, IHandPoseSelectable
    {
        [SerializeField]
        PoseActivationType m_activationType;

        public XRBaseInteractor CurrentlyHoveringInteractor { get; private set; }
        public XRBaseInteractor CurrentlySelectingInteractor { get; private set; }

        public void HandleHoverEnd(HandPoseProviderArgs args)
        {
            CurrentlyHoveringInteractor = null;
        }

        public void HandleHoverStart(HandPoseProviderArgs args)
        {
            CurrentlyHoveringInteractor = args.DirectInteractor;
        }

        public void HandleSelectEnd(HandPoseProviderArgs args)
        {
            CurrentlySelectingInteractor = null;
        }

        public void HandleSelectStart(HandPoseProviderArgs args)
        {
            CurrentlySelectingInteractor = args.DirectInteractor;
        }
    }
}

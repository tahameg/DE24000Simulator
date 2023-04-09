using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Katana.XR.Interactables.HandPoseSystem.Utils;

namespace Katana.XR.Interactables.HandPoseSystem
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

    /*
    public class HandPoseProvider : MonoBehaviour
    {
        [SerializeField]
        List<CustomHandPoseConfiguration> m_leftHandConfigurationsList;
        [SerializeField]
        List<CustomHandPoseConfiguration> m_rightHandConfigurationsList;

        Dictionary<Vector3, CustomHandPoseConfiguration> m_leftHandConfigurationsDict;
        Dictionary<Vector3, CustomHandPoseConfiguration> m_rightHandConfigurationsDict;


        private void Awake()
        {
            
        }

        void Initialize()
        {
            m_leftHandConfigurationsDict = new Dictionary<Vector3, CustomHandPoseConfiguration>();
            if (m_leftHandConfigurationsList != null)
            {
                foreach (var config in m_leftHandConfigurationsList)
                {
                    m_leftHandConfigurationsDict.Add(config.LocalPosition, config);
                }
            }

            m_rightHandConfigurationsDict = new Dictionary<Vector3, CustomHandPoseConfiguration>();
            if (m_rightHandConfigurationsList != null)
            {
                foreach (var config in m_rightHandConfigurationsList)
                {
                    m_rightHandConfigurationsDict.Add(config.LocalPosition, config);
                }
            }
        }
    }
    */
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using SparkVision.HandPoseSystem;

namespace SparkVision.InteractionUtilities
{
    public class HandPresence : MonoBehaviour
    {
        public Handedness Handedness;

        [SerializeField]
        XRDirectInteractor m_directInteractor;

        [SerializeField]
        HandPoseOperator m_handPoseOperator;

        bool isGrabbing => GrabAction.action.IsPressed();
        bool isTriggering => TriggerAction.action.IsPressed();
        GameObject m_visualHandObject => m_handPoseOperator.StructuralInfo.HandObject;

        [SerializeField]
        HandRecord m_defaultPose;
        [SerializeField]
        HandRecord m_defaultPinchPose;
        [SerializeField]
        HandRecord m_defaultGrabPose;

        IHandPoseHoverable m_currentlyHoveredPoseable;
        IHandPoseSelectable m_currentlySelectedPoseable;

        public InputActionProperty TriggerAction;
        public InputActionProperty GrabAction;

        private void Start()
        {
            SetVisualHandVisible(true);
            m_directInteractor.hoverEntered.AddListener(OnHoverEnter);
            m_directInteractor.hoverExited.AddListener(OnHoverExit);
            m_directInteractor.selectEntered.AddListener(OnSelectEnter);
            m_directInteractor.selectExited.AddListener(OnSelectExit);
            TriggerAction.action.performed += OnTriggerPress;
            TriggerAction.action.canceled += OnTriggerRelease;

            GrabAction.action.performed += OnGrabPress;
            GrabAction.action.canceled += OnGrabRelease;
            UpdatePoses();
        }

        private void SetVisualHandVisible(bool state)
        {
            m_visualHandObject.transform.gameObject.SetActive(state);
        }

        public void ApplyRecord(HandRecord record) 
            => m_handPoseOperator.ApplyRecord(record);

        public void ApplyRecord(HandRecord record, float animationDuration) 
            => m_handPoseOperator.ApplyRecord(record, animationDuration);

        void SetColor(Transform transform, Color color)
        {
            var renderer = transform.GetComponent<Renderer>();
            MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", color);
        }

        void OnHoverEnter(HoverEnterEventArgs args)
        {
            var hoverable = args.interactableObject.transform.GetComponent<IHandPoseHoverable>();
            if(hoverable != null)
            {
                m_currentlyHoveredPoseable = hoverable;
                var hoverArgs = new HandPoseProviderArgs(transform,
                        Handedness,
                        m_handPoseOperator,
                        m_directInteractor);
                hoverable.HandleHoverStart(hoverArgs);
            }
        }

        void OnHoverExit(HoverExitEventArgs args)
        {
            var hoverable = args.interactableObject.transform.GetComponent<IHandPoseHoverable>();
            if (hoverable != null)
            {
                var hoverArgs = new HandPoseProviderArgs(transform,
                    Handedness,
                    m_handPoseOperator,
                    m_directInteractor);
                hoverable.HandleHoverEnd(hoverArgs);
                m_currentlyHoveredPoseable = null;
            }
        }

        void OnSelectEnter(SelectEnterEventArgs args)
        {
            var selectable = args.interactableObject.transform.GetComponent<IHandPoseSelectable>();
            if(selectable != null)
            {
                var selectArgs = new HandPoseProviderArgs(transform,
                    Handedness,
                    m_handPoseOperator,
                    m_directInteractor);
                selectable.HandleSelectStart(selectArgs);
                m_currentlySelectedPoseable = selectable;
            }
        }

        void OnSelectExit(SelectExitEventArgs args)
        {
            var selectable = args.interactableObject.transform.GetComponent<IHandPoseSelectable>();
            if (selectable != null)
            {
                var selectArgs = new HandPoseProviderArgs(transform,
                    Handedness,
                    m_handPoseOperator,
                    m_directInteractor);
                selectable.HandleSelectEnd(selectArgs);
                m_currentlySelectedPoseable = null;
            }
        }

        void UpdatePoses()
        {
            if (isGrabbing)
            {
                ApplyRecord(m_defaultGrabPose, 0.1f);
            }
            else if (isTriggering)
            {
                ApplyRecord(m_defaultPinchPose, 0.1f);
            }
            else
            {
                ApplyRecord(m_defaultPose, 0.1f);
            }
        }

        void OnTriggerPress(InputAction.CallbackContext context)
        {
            if(m_currentlyHoveredPoseable == null && m_currentlySelectedPoseable == null)
            {
                UpdatePoses();
            }
            else if(m_currentlyHoveredPoseable != null)
            {
                var activateable = (IHandPoseActivateable)m_currentlyHoveredPoseable;
                if(activateable != null)
                {
                    var activateArgs = new HandPoseProviderArgs(transform,
                    Handedness,
                    m_handPoseOperator,
                    m_directInteractor);
                    activateable.HandleActivateStart(activateArgs);
                }
            }
        }
        void OnTriggerRelease(InputAction.CallbackContext context)
        {
            if (m_currentlyHoveredPoseable == null && m_currentlySelectedPoseable == null)
            {
                UpdatePoses();
            }
            else if (m_currentlyHoveredPoseable != null)
            {
                var activateable = (IHandPoseActivateable)m_currentlyHoveredPoseable;
                if (activateable != null)
                {
                    var activateArgs = new HandPoseProviderArgs(transform,
                    Handedness,
                    m_handPoseOperator,
                    m_directInteractor);
                    activateable.HandleActivateEnd(activateArgs);
                }
            }
        }

        void OnGrabPress(InputAction.CallbackContext context)
        {
            if (m_currentlyHoveredPoseable == null && m_currentlySelectedPoseable == null)
            {
                UpdatePoses();
            }
        }

        void OnGrabRelease(InputAction.CallbackContext context)
        {
            if (m_currentlyHoveredPoseable == null && m_currentlySelectedPoseable == null)
            {
                UpdatePoses();
            }
        }
    }
}


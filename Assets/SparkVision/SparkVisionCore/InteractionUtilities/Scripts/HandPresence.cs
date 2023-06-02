using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using SparkVision.HandPoseSystem;

namespace SparkVision.HandPoseSystem
{
    public class HandPresence : MonoBehaviour
    {
        public Handedness Handedness;
        [HideInInspector]
        public HandPoseOperator HandPoseOperator
        {
            get => m_handPoseOperator;
            set => m_handPoseOperator = value;
        }

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

        HandPoseProviderArgs handArgs;
        
        public InputActionProperty TriggerAction;
        public InputActionProperty GrabAction;

        bool m_isPoseFree => (
            m_currentlyHoveredPoseable == null
            && m_currentlySelectedPoseable == null && !HandRecordModeManager.IsRecordModeActive
            );
        
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
            handArgs = new HandPoseProviderArgs(transform,
                Handedness,
                m_handPoseOperator,
                m_directInteractor);
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
            if (hoverable == null) return;
            m_currentlyHoveredPoseable = hoverable;
            hoverable.HandleHoverStart(handArgs);
        }

        void OnHoverExit(HoverExitEventArgs args)
        {
            var hoverable = args.interactableObject.transform.GetComponent<IHandPoseHoverable>();
            if (hoverable == null) return;

            hoverable.HandleHoverEnd(handArgs);
            m_currentlyHoveredPoseable = null;
            if (m_currentlySelectedPoseable == null) UpdatePoses();
        }

        void OnSelectEnter(SelectEnterEventArgs args)
        {
            var selectable = args.interactableObject.transform.GetComponent<IHandPoseSelectable>();
            if(selectable == null) return;

            selectable.HandleSelectStart(handArgs);
            m_currentlySelectedPoseable = selectable;
        }
        
        void OnSelectExit(SelectExitEventArgs args)
        {
            var selectable = args.interactableObject.transform.GetComponent<IHandPoseSelectable>();
            if (selectable == null) return;

            var selectArgs = new HandPoseProviderArgs(transform,
                Handedness,
                m_handPoseOperator,
                m_directInteractor);

            selectable.HandleSelectEnd(selectArgs);
            m_currentlySelectedPoseable = null;
            
            if(m_currentlyHoveredPoseable != null)
            {
                m_currentlyHoveredPoseable.HandleHoverStart(handArgs);
            }
            else
            {
                UpdatePoses();
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
            if(m_isPoseFree)
            {
                UpdatePoses();
            }
            else if(m_currentlySelectedPoseable != null)
            {
                var activateable = m_currentlySelectedPoseable as IHandPoseActivateable;
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
            if (m_isPoseFree)
            {
                UpdatePoses();
            }
            else if (m_currentlySelectedPoseable != null)
            {
                var activateable = m_currentlySelectedPoseable as IHandPoseActivateable;
                if (activateable != null)
                {
                    activateable.HandleActivateEnd(handArgs);
                }
            }
        }

        void OnGrabPress(InputAction.CallbackContext context)
        {
            if (m_isPoseFree)
            {
                UpdatePoses();
            }
        }

        void OnGrabRelease(InputAction.CallbackContext context)
        {
            if (m_isPoseFree)
            {
                UpdatePoses();
            }
        }
    }
}


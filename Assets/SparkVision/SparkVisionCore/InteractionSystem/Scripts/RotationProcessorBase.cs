using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace SparkVision.InteractionSystem
{
    public enum ConfigurationBehaviour
    {
        ResetValues,
        UpdateTransform
    }

    public enum ParameterType
    {
        Position,
        Rotation
    }

    /// <summary>
    /// An abstract base class that simplifies rotation-based behavior.
    /// Requires a CustomInteractable set up and configured.
    /// Provides an angle value (accessed via 'Value') that tracks the total rotational
    /// movement applied to the component by an interactor since the start.
    /// The 'Value' parameter is unbounded and signed, and can be greater than 360,
    /// which is useful for calculating rotational cycles.
    /// See 'Examples/Samples/RotationalInteractions.scene' for usage examples.
    /// </summary>
    [RequireComponent(typeof(CustomInteractable))]
    public abstract class RotationProcessorBase : MonoBehaviour, IXRGrabProcess
    {
        /// <summary>
        /// The target transform used for axis calculations. If 'UseWorldSpace' is unchecked,
        /// the rotation axis is interpreted as a local axis of this transform. If set to null,
        /// the transform of the gameObject is used.
        /// </summary>
        [Tooltip("The target transform used for axis calculations. If 'UseWorldSpace' is unchecked," +
            " the rotation axis is interpreted as a local axis of this transform. If set to null, the" +
            " transform of the gameObject is used.")]
        [SerializeField]
        protected Transform TargetTransform;

        /// <summary>
        /// Defines the behavior of the component when its parameters are changed during runtime.
        /// For example, when the rotation limits are changed during runtime, 'Reset Values' sets
        /// the current value to 0 and starts applying the limits with the current transform.
        /// 'Update Transform' rotates inside the new limits if it is outside of them.
        /// </summary>
        public ConfigurationBehaviour ConfigurationBehaviour;
        /// <summary>
        /// Defines which component of the interacting interactor is used for calculating the angular movement.
        /// If set to "Position", position change is used. If set to "Rotation", rotation change is used.
        /// </summary>
        [Tooltip("Defines which component of the interacting interactor is used for calculating the angular movement. " +
                 "If set to 'Position', position change is used. If set to 'Rotation', rotation change is used.")]
        [SerializeField]
        ParameterType m_usedParameterType;

        /// <summary>
        /// Defines the transform that is at the center of the rotation. Rotation is
        /// calculated around the object.
        /// </summary>
        [Tooltip("Defines the transform that is at the center of the rotation. " +
                 "Rotation is calculated around the object.")]
        [SerializeField] public Transform m_rotatePointReference;

        /// <summary>
        /// Defines the axis that is used for rotation calculation. If UseWorldSpace is
        /// set, the axis is interpreted as a world space axis.
        /// </summary>
        [Tooltip("Defines the axis that is used for rotation calculation. If " +
                 "UseWorldSpace is set, the axis is interpreted as a world space axis.")]
        [SerializeField] public Vector3 m_rotationAxis;

        /// <summary>
        /// If UseWorldSpace is set, the axis is interpreted as a world space axis.
        /// </summary>
        [Tooltip("If UseWorldSpace is set, the axis is interpreted as a world space axis.")]
        [SerializeField] public bool m_useWorldSpace;

        /// <summary>
        /// If is set, the given limits are taken into account. In this case, the
        /// component won't give values that are outside the boundary defined by
        /// ValueMin and ValueMax.
        /// </summary>
        [Tooltip("If set, the given limits are taken into account. In this case, " +
                 "the component won't give values that are outside the boundary " +
                 "defined by ValueMin and ValueMax.")]
        [SerializeField] public bool m_hasLimits;


        [SerializeField]
        public float m_valueMin;
        [SerializeField]
        public float m_valueMax;

        /// <summary>
        /// Defines the transform that is at the center of the rotation. Rotation is calculated around the object.
        /// </summary>
        public Transform RotatePointReference
        {
            get => m_rotatePointReference;
            set
            {
                m_rotatePointReference = value;
            }
        }

        /// <summary>
        /// Sets the RotationAxis.
        /// <remarks>
        /// Warning: Changing this on runtime will reset the component values without 
        /// changing the targetTransform. So, it will the resulting behaviour
        /// will be as the "m_configurationBehaviour"  parameter is set to the "ResetValues".
        /// </remarks>
        /// </summary>
        public Vector3 RotationAxis
        {
            get => m_rotationAxis;
            set => UpdateRotationAxis(value, m_useWorldSpace);
        }

        /// <summary>
        /// Gives bounded rotation angle of the TargetTransform since the start or reset. ( 0 - 360 degreees)
        /// </summary>
        public float RotationAngle => GetCurrentAngle();

        /// <summary>
        /// Amount of the total rotating movement applied to the component by an interactor since the start. Can be bigger than 
        /// 360 or smaller than -360. This can be used for calculating the cycle count. 
        /// (Etc. a Value of 720 the rotational movement is enough to rotate the TargetTransform 2 times)
        /// </summary>
        public float Value
        {
            get => m_value;
            set => SetValueTo(value);
        }

        /// <summary>
        /// If is set, the given limits are taken account. In this case, the component won't 
        /// give values that are out of the boundary that is defined by ValueMin and ValueMax
        /// </summary>
        public bool HasLimits
        {
            get => m_hasLimits;
            set
            {
                m_hasLimits = true;
                UpdateRotationLimits(m_valueMin, m_valueMax);
            }
        }

        /// <summary>
        /// Gives the world space rotation axis.
        /// </summary>
        public Vector3 AbsoluteRotationAxis
        {
            get => m_useWorldSpace ? m_rotationAxis : TargetTransform.TransformDirection(m_rotationAxis);
        }

        /// <summary>
        /// Sets how the m_rotationAxis will be interpreted. It this is set to true,
        /// it will be interpreted in world space.
        /// <remarks>
        /// Warning: Changing this on runtime will reset the component values without 
        /// changing the targetTransform. So, it will the resulting behaviour
        /// will be as the "m_configurationBehaviour"  parameter is set to the "ResetValues".
        /// </remarks>
        /// </summary>
        public bool UseWorldSpace
        {
            get => m_useWorldSpace;
            set => UpdateRotationAxis(m_rotationAxis, value);
        }

        public float ValueMax
        {
            get => m_valueMax;
            set => UpdateRotationLimits(m_valueMin, value);
        }

        public float ValueMin
        {
            get => m_valueMax;
            set => UpdateRotationLimits(value, m_valueMax);
        }

        /// <summary>
        /// current angular velocity that is applied by the hand;
        /// </summary>
        public float CurrentAngularVelocity => m_angularVelocity;

        public bool CanRotate = true;

        Vector3 m_referenceZeroVector;
        Vector3 m_localRefererenceZeroVector;
        float m_angularVelocity;
        float m_value = 0f;

        Pose m_startingWorldPose;
        Transform m_grabTransform;
        Pose m_lastWorldPose;

        [Header("Exposed Events")]
        UnityEvent m_onGrabStart;
        public UnityEvent GrabStarted => m_onGrabStart;


        UnityEvent<float> m_onAngleValueChange;
        public UnityEvent<float> ValueChanged => m_onAngleValueChange;

        UnityEvent<float> m_onGrabEnd;
        public UnityEvent<float> GrabEnded => m_onGrabEnd;

        protected virtual void Awake()
        {
            if (TargetTransform == null)
            {
                TargetTransform = transform;
            }

            if (m_rotatePointReference == null)
            {
                m_rotatePointReference = transform;
            }

            UpdateRotationAxis(m_rotationAxis, m_useWorldSpace);
        }

        protected virtual void StartProcess(CustomInteractionArgs args)
        {
            m_startingWorldPose = args.InteractionPose;
            m_grabTransform = args.GrabTransform;
            m_lastWorldPose = args.InteractionPose;
            m_onGrabStart?.Invoke();
        }

        protected virtual void UpdateProcess(CustomInteractionArgs args)
        {
            float angleDiff = CalculateAngleDifference(m_lastWorldPose, args.InteractionPose);
            m_angularVelocity = (angleDiff / Time.deltaTime);
            if (CanRotate)
            {
                float valueToSet = m_hasLimits ? Mathf.Clamp(m_value + angleDiff, m_valueMin, m_valueMax)
                : (m_value + angleDiff);
                SetValueTo(valueToSet);
            }
            m_lastWorldPose = args.InteractionPose;
        }

        protected virtual void ExitProcess(CustomInteractionArgs args)
        {
            m_onGrabEnd?.Invoke(m_angularVelocity);
            m_angularVelocity = 0.0f;
            Debug.Log("GrabEnd");
        }

        void IXRGrabProcess.StartProcess(CustomInteractionArgs args) => StartProcess(args);

        void IXRGrabProcess.UpdateProcess(CustomInteractionArgs args) => UpdateProcess(args);


        void IXRGrabProcess.ExitProcess(CustomInteractionArgs args) => ExitProcess(args);

        void UpdateRotationAxis(Vector3 rotationAxis, bool useWorldSpace)
        {
            m_rotationAxis = rotationAxis;
            m_useWorldSpace = useWorldSpace;
            Vector3 abs = AbsoluteRotationAxis.normalized;
            Vector3 helperVector = abs != Vector3.up ? Vector3.up : Vector3.right;
            m_referenceZeroVector = Vector3.Cross(AbsoluteRotationAxis, helperVector).normalized;
            m_localRefererenceZeroVector = TargetTransform.InverseTransformDirection(m_referenceZeroVector);

        }

        void UpdateRotationLimits(float limitMin, float limitMax)
        {
            if (limitMin > limitMax)
            {
                Debug.LogWarning("limitMin cannot be greater than limitMax");
                return;
            }

            if (limitMin > 0)
            {
                Debug.LogWarning("limitMin cannot be greater than 0");
                return;
            }

            if (limitMax < 0)
            {
                Debug.LogWarning("limitMax cannot be lower than 0");
                return;
            }

            if (m_hasLimits)
            {
                if (ConfigurationBehaviour == ConfigurationBehaviour.UpdateTransform)
                {
                    if (m_value < limitMin)
                    {
                        SetValueTo(limitMin);
                    }

                    if (m_value > limitMax)
                    {
                        SetValueTo(limitMax);
                    }
                }
                else
                {
                    m_value = 0f;
                }
            }

            m_valueMin = limitMin;
            m_valueMax = limitMax;
        }

        public void ResetProcessor()
        {
            m_value = 0;
        }


        public float GetCurrentAngle()
        {
            Vector3 referenceCurrent = TargetTransform.TransformDirection(m_localRefererenceZeroVector);
            return Vector3.SignedAngle(m_referenceZeroVector, referenceCurrent, AbsoluteRotationAxis);
        }

        public float CalculateAngleDifference(Pose previousPose, Pose currentPose)
        {
            if (m_usedParameterType == ParameterType.Position)
            {
                Vector3 projectedBefore = Vector3.ProjectOnPlane(previousPose.position - m_rotatePointReference.position,
                    AbsoluteRotationAxis).normalized;
                Vector3 proejectedCurrent = Vector3.ProjectOnPlane(currentPose.position - m_rotatePointReference.position,
                    AbsoluteRotationAxis).normalized;
                return Vector3.SignedAngle(projectedBefore, proejectedCurrent, AbsoluteRotationAxis);
            }
            else
            {
                Quaternion rotationDiff = currentPose.rotation * Quaternion.Inverse(previousPose.rotation);
                Vector3 rotatedVector = rotationDiff * m_referenceZeroVector;
                Vector3 projectedRotatedVector = Vector3.ProjectOnPlane(rotatedVector, AbsoluteRotationAxis).normalized;
                float value = Vector3.SignedAngle(m_referenceZeroVector, projectedRotatedVector, AbsoluteRotationAxis);
                return value;
            }
        }

        protected bool IsValidValue(float value)
        {
            return m_hasLimits ? (value <= m_valueMax && value >= m_valueMin) : true;
        }
        public virtual void SetValueTo(float value)
        {
            if (!IsValidValue(value)) return;
            m_value = value;
            m_onAngleValueChange?.Invoke(m_value);
            return;
        }
    }
}
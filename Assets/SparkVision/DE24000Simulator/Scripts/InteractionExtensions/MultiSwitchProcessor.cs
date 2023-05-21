using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Events;
using SparkVision.InteractionSystem;

namespace DE24000Simulator
{
    public class MultiSwitchProcessor : RotationProcessorBase
    {
        /// <summary>
        /// These are the preset resting points of the switch. Each switch point has a corresponding value
        /// in the Rotational processor, which is determined by multiplying the switch point by the
        /// valueMultiplier. This ratio determines the relationship between the real hand rotation and the
        /// predefined switch point. For instance, if there is a switch point at 30 and the valueMultiplier
        /// is set to 2, a hand rotation of 15 degrees will correspond to a value of 30 and toggle to that state.
        /// </summary>
        [Tooltip("These are the predefined resting points of the switch. They do not directly match with the " +
            "'Value' parameter of the Rotational processor. However, there is a relationship such that " +
            "switchPoint * valueMultiplier = Value. The value multiplier determines the ratio between real " +
            "hand rotation and predefined switch point. For example, if there is a switch point at 30, and " +
            "the value multiplier is set to 2, a 15 degree hand rotation will translate to 30 and toggle " +
            "to that state.")]
        [SerializeField]
        List<float> m_switchPoints;
        [SerializeField]
        float m_valueMultiplier = 1f;
        [SerializeField]
        int m_initialSwitchIndex;

        public List<float> SwitchPoints => m_switchPoints;
        public float CurrentSwitchPoint { get; private set; }
        public float PreviousSwitchPoint { get; private set; }
        public int CurrentSwitchPointIndex { get; private set; }

        public AudioClip ClickSound;

        [Tooltip("Returns the previous switch point and the current switch" +
            "point respectively")]
        public UnityEvent<int, int> OnSwitchUpdate;
        bool m_isValid => (m_switchPoints != null && m_switchPoints.Count > 0);

        Quaternion m_initialRotation;

        float m_visualAngle;
        IEnumerator m_returnToSwitchPointRoutine;
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        void Initialize()
        {
            if (!m_isValid)
            {
                Debug.LogWarning("Couldn't be initialized since SwitchPoints was null or empty");
                return;
            }

            m_initialRotation = TargetTransform.localRotation;

            CurrentSwitchPointIndex = (m_initialSwitchIndex >= 0 && m_initialSwitchIndex < m_switchPoints.Count)
                ? m_initialSwitchIndex : 0;

            //Reorder switch points in ascending order
            ReorderSwitchPoints();
            float switchTarget = m_switchPoints[CurrentSwitchPointIndex];
            float valueToSet = switchTarget / m_valueMultiplier;
            SetValueTo(valueToSet);
        }

        public void ReorderSwitchPoints()
        {
            if (!m_isValid) return;
            m_switchPoints.Sort((a, b) => a.CompareTo(b));


            if (CurrentSwitchPointIndex < 0 || CurrentSwitchPointIndex >= m_switchPoints.Count) return;

            float currentValue = m_switchPoints[CurrentSwitchPointIndex];
            if (currentValue != m_switchPoints[CurrentSwitchPointIndex]) //Than currentIndex should be updated
            {
                int newIndex = m_switchPoints.FindIndex(a => a == currentValue);
                CurrentSwitchPointIndex = newIndex;
            }
        }


        void HandleSwitchUpdate(int oldValue, int newValue)
        {
            OnSwitchUpdate?.Invoke(oldValue, newValue);
            if (ClickSound != null)
            {
                AudioManager.PlayAudioAt(ClickSound, TargetTransform.position);
            }
        }

        /// <summary>
        /// Switch is forced in one direction but the force is not 
        /// enough to switch.
        /// </summary>
        /// <param name="angleDiff"> Difference of the force angle
        /// with the closest switch point</param>
        void HandleSwitchForced(float angleDiff)
        {
            SetVisualAngle(m_switchPoints[CurrentSwitchPointIndex] + angleDiff / 10f);
        }

        void SetVisualAngle(float angle)
        {
            Vector3 axis = TargetTransform.InverseTransformDirection(AbsoluteRotationAxis).normalized;
            TargetTransform.localRotation = m_initialRotation * Quaternion.Euler(angle * axis);
            m_visualAngle = angle;
        }

        protected override void ExitProcess(CustomInteractionArgs args)
        {
            base.ExitProcess(args);
            if (m_returnToSwitchPointRoutine != null)
            {
                StopCoroutine(m_returnToSwitchPointRoutine);
                m_returnToSwitchPointRoutine = null;
            }

            m_returnToSwitchPointRoutine = LerpToVisualAngle(m_switchPoints[CurrentSwitchPointIndex]);
            StartCoroutine(m_returnToSwitchPointRoutine);
        }

        protected override void StartProcess(CustomInteractionArgs args)
        {
            base.StartProcess(args);
            if (m_returnToSwitchPointRoutine != null)
            {
                StopCoroutine(m_returnToSwitchPointRoutine);
                m_returnToSwitchPointRoutine = null;
            }
        }

        IEnumerator LerpToVisualAngle(float angle)
        {
            bool condition = Mathf.Abs(m_visualAngle - angle) >= 0.1f;
            while (condition)
            {
                SetVisualAngle(Mathf.LerpAngle(m_visualAngle, angle, 0.3f));
                condition = Mathf.Abs(m_visualAngle - angle) >= 0.1f;
                yield return new WaitForSeconds(0.03f);
            }
        }

        public override void SetValueTo(float value)
        {
            if (!m_isValid)
            {
                base.SetValueTo(value);
                return;
            };

            float multipliedValue = value * m_valueMultiplier;
            int nextIndex = CurrentSwitchPointIndex + 1;
            int previousIndex = CurrentSwitchPointIndex - 1;
            bool shouldSet = false;
            int lastIndex = CurrentSwitchPointIndex;

            if (multipliedValue > m_switchPoints[CurrentSwitchPointIndex])
            {
                if (nextIndex >= m_switchPoints.Count) return;
                while (multipliedValue >= m_switchPoints[nextIndex])
                {
                    CurrentSwitchPointIndex = nextIndex;
                    shouldSet = true;
                    nextIndex += 1;
                    if (nextIndex >= m_switchPoints.Count) break;
                }
            }
            else if (multipliedValue < m_switchPoints[CurrentSwitchPointIndex])
            {
                if (previousIndex < 0) return;
                while (multipliedValue <= m_switchPoints[previousIndex])
                {
                    CurrentSwitchPointIndex = previousIndex;
                    shouldSet = true;
                    previousIndex -= 1;
                    if (previousIndex < 0) break;
                }
            }

            if (shouldSet)
            {
                HandleSwitchUpdate(lastIndex, CurrentSwitchPointIndex);
                SetVisualAngle(m_switchPoints[CurrentSwitchPointIndex]);
            }
            else
            {
                HandleSwitchForced(value - m_switchPoints[CurrentSwitchPointIndex]);
            }

            if (value >= m_switchPoints[m_switchPoints.Count - 1])
            {
                base.SetValueTo(m_switchPoints[m_switchPoints.Count - 1]);
            }

            else if (value <= m_switchPoints[0])
            {
                base.SetValueTo(m_switchPoints[0]);
            }
            else
            {
                base.SetValueTo(value);
            }
        }
    }
}
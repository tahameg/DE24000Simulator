using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;


public class MultiSwitchProcessor : RotationProcessorBase
{
    [SerializeField]
    List<float> m_switchPoints;
    [SerializeField]
    float m_valueMultiplier = 1f;
    [SerializeField]
    int m_initialSwitchIndex;
    
    float m_currentSwitchValue;

    public int CurrentlySelectedIndex { get; private set; }
    bool m_isValid = false;
    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    void Initialize()
    {
        if(m_switchPoints != null && m_switchPoints.Count > 0)
        {
            int initialIndex = 0;
            if(m_initialSwitchIndex >= 0 && m_initialSwitchIndex < m_switchPoints.Count)
            {
                initialIndex = m_initialSwitchIndex;
            }
            float switchTarget = m_switchPoints[initialIndex];
            float valueToSet = switchTarget / m_valueMultiplier;
            SetValueTo(valueToSet);
            CurrentlySelectedIndex = initialIndex;
            m_currentSwitchValue = m_switchPoints[initialIndex];
            TargetTransform.RotateAround(m_rotatePointReference.position,
                    AbsoluteRotationAxis,
                    m_currentSwitchValue);
            m_isValid = true;
        }
        else
        {
            Debug.LogWarning("Couldn't be initialized since SwitchPoints was null or empty");
        }
    }

    public override void SetValueTo(float value)
    {
        if (m_isValid)
        {
            float multipliedValue = value * m_valueMultiplier;
            int nextIndex = CurrentlySelectedIndex + 1;
            int previousIndex = CurrentlySelectedIndex - 1;
            bool shouldSet = false;
            if(multipliedValue > m_switchPoints[CurrentlySelectedIndex])
            {
                if (nextIndex >= m_switchPoints.Count) return;
                while (multipliedValue >= m_switchPoints[nextIndex])
                {
                    CurrentlySelectedIndex = nextIndex;
                    shouldSet = true;
                    nextIndex += 1;
                    if (nextIndex >= m_switchPoints.Count) break;
                }
            }
            else if(multipliedValue < m_switchPoints[CurrentlySelectedIndex])
            {
                if (previousIndex < 0) return;
                while(multipliedValue <= m_switchPoints[previousIndex])
                {
                    CurrentlySelectedIndex = previousIndex;
                    shouldSet = true;
                    previousIndex -= 1;
                    if (previousIndex < 0) break;
                }

            }

            if(shouldSet)
            {
                TargetTransform.RotateAround(m_rotatePointReference.position, 
                    AbsoluteRotationAxis, 
                    m_switchPoints[CurrentlySelectedIndex] - m_currentSwitchValue);
                m_currentSwitchValue = m_switchPoints[CurrentlySelectedIndex];
            }

            if(value >= m_switchPoints[m_switchPoints.Count - 1]) 
            {
                base.SetValueTo(m_switchPoints[m_switchPoints.Count - 1]);
            }

            else if(value <= m_switchPoints[0])
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

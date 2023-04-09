using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : RotationProcessorBase
{
    public override void SetValueTo(float value)
    {
        float beforeValue = Value;
        base.SetValueTo(value);
        TargetTransform.RotateAround(m_rotatePointReference.position, AbsoluteRotationAxis, value - beforeValue);
    }
}

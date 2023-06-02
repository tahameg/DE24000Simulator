using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SparkVision.HandPoseSystem
{
    [RequireComponent(typeof(HandPoseOperator))]
    public class HandRecordReference : MonoBehaviour
    {
        public Handedness Handedness;
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Katana.XR.Interactables.HandPoseSystem
{
    [Serializable]
    public class HandRecord
    {
        public FingerRecord[] FingerRecords;
        public Pose RootPose;
        public int FingerCount { get { return FingerRecords == null ? -1 : FingerRecords.Length; } }

        public HandRecord(FingerRecord[] fingerRecords, Pose rootPose)
        {
            FingerRecords = fingerRecords;
            RootPose = rootPose;
        }
    }
}

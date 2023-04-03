using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Katana.XR.Interactables.HandPoseSystem
{
    [Serializable]
    public class FingerRecord
    {
        public Quaternion[] Joints;

        public FingerRecord() { }

        public FingerRecord(Quaternion[] joints)
        {
            Joints = joints;
        }
    }
}
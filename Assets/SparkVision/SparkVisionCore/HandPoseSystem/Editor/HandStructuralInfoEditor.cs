using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SparkVision.HandPoseSystem;
using SparkVision.HandPoseSystem.Utils;

namespace SparkVision.HandPoseSystem.Editor
{
    [CustomEditor(typeof(HandStructuralInfo))]
    public class HandStructuralInfoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            HandStructuralInfo infoObject = (HandStructuralInfo)target;

            if (GUILayout.Button("Generate Structural Data"))
            {
                if (infoObject.HandObject != null)
                {
                    infoObject.IndexFingerTransforms = HandUtils.GetAllJointsWithPrefix(infoObject.HandObject.transform, "Index");
                    infoObject.MiddleFingerTransforms = HandUtils.GetAllJointsWithPrefix(infoObject.HandObject.transform, "middle");
                    infoObject.RingFingerTransforms = HandUtils.GetAllJointsWithPrefix(infoObject.HandObject.transform, "ring");
                    infoObject.PinkyFingerTransforms = HandUtils.GetAllJointsWithPrefix(infoObject.HandObject.transform, "little");
                    infoObject.ThumbTransforms = HandUtils.GetAllJointsWithPrefix(infoObject.HandObject.transform, "thumb");
                }
            }
        }
    }
}

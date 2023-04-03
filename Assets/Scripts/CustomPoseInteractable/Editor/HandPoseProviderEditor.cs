using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Katana.XR.Interactables.HandPoseSystem;

namespace Katana.XR.Interactables.HandPoseSystem.Editor
{
    [CustomEditor(typeof(HandPoseProvider))]
    public class HandPoseProviderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            HandPoseProvider provider = (HandPoseProvider)target;

            if(GUILayout.Button("Generate Hand Info"))
            {
                provider.AutoGenerateHandConfigurations();
            }
        }
    }
}

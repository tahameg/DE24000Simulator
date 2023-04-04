using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Katana.XR.Interactables.HandPoseSystem;
using Katana.XR.Interactables.HandPoseSystem.Data;

namespace Katana.XR.Interactables.HandPoseSystem.Editor
{
    [CustomEditor(typeof(HandPoseRecorder))]
    public class HandPoseRecorderEditor : UnityEditor.Editor
    {

        void SaveData(HandRecord record)
        {

        }
    }

}
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SparkVision.HandPoseSystem;
using SparkVision.HandPoseSystem.EditorUtils;

namespace SparkVision.HandPoseSystem.Editor
{
    [CustomEditor(typeof(HandPoseRecorder))]
    public class HandPoseRecorderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            HandPoseRecorder recorder = (HandPoseRecorder)target;
            if (HandRecordModeManager.IsRecordModeActive )
            {
                if (GUILayout.Button("Record Hand Pose"))
                {
                    HandRecord record = recorder.GetHandRecord();
                    IOUtils.SaveScriptableObject(recorder.DataSaveDirectory, recorder.DataSaveName, record);
                    recorder.TestHandRecords.Add(record);
                }
            }
            

            for (int i = 0; i < recorder.TestHandRecords.Count; i++)
            {
                if (GUILayout.Button($"Translate to {i}"))
                {
                    recorder.HandPoseOperator.ApplyRecord(recorder.TestHandRecords[i], recorder.transitionTime);
                }
            }
        }
    }

}
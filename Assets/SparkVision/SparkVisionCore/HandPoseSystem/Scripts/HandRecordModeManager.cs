using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SparkVision.HandPoseSystem;

public class HandRecordModeManager : MonoBehaviour
{
    public static bool IsRecordModeActive
        => Instance != null && Instance.ActivateRecordMode && Instance.m_isReadyForRecording;

    public static HandRecordModeManager Instance;
    public bool ActivateRecordMode;
    bool m_isReadyForRecording;
    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        if (!Instance.ActivateRecordMode) return;
        StartCoroutine("RecordingInitializationRoutine");

    }

    IEnumerator RecordingInitializationRoutine()
    {
        bool pass = false;
        HandRecordReference[] references = new HandRecordReference[0];
        HandPresence[] handPresences = new HandPresence[0];
        while (!pass)
        {
            references = FindObjectsOfType<HandRecordReference>();
            if (references != null && references.Length >= 2) pass = true;
            yield return new WaitForSeconds(1f);
        }

        pass = false;
        while (!pass)
        {
            handPresences = FindObjectsOfType<HandPresence>();
            if (handPresences != null && handPresences.Length >= 2) pass = true;
            yield return new WaitForSeconds(1f);
        }

        for(int i = 0; i < handPresences.Length; i++)
        {
            for (int j = 0; j < references.Length; j++)
            {
                if(references[j].Handedness == handPresences[i].Handedness)
                {
                    handPresences[i].HandPoseOperator = references[j].transform.GetComponent<HandPoseOperator>();
                }
            }
        }
        m_isReadyForRecording = true;
    }
}

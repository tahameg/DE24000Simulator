using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SparkVision.HandPoseSystem.EditorUtils;
using System.IO;

namespace SparkVision.HandPoseSystem.Editor
{
    [CustomEditor(typeof(HandPoseProvider))]
    public class HandPoseProviderEditor : UnityEditor.Editor
    {
        bool m_isListInitialized;
        int m_selectedIndex = 0;
        int m_selectedIndexBefore = 0;
        GUIContent[] itemList;
        Dictionary<string, Type> infoBaseClasses;
        HandPoseProvider source;
        Material mat;
        private void OnEnable()
        {
            FetchList();
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            source = (HandPoseProvider)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(15f);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Interaction Pose Infos");
            m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, itemList);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create New Info"))
            {
                source.gameObject.AddComponent(infoBaseClasses[itemList[m_selectedIndex].text]);
            }
            if (GUILayout.Button("Refresh"))
            {
                FetchList();
            }
            GUILayout.EndHorizontal();

            if (HandRecordModeManager.IsRecordModeActive && Application.isPlaying && Application.isEditor)
            {
                if (GUILayout.Button("Record"))
                {
                    HandleRecord();
                }
            }
        }

        void HandleRecord()
        {
            if (source.CurrentlyHoveringInteractor == null) return;
            string saveFolderPath;
            if (!CreateSaveDirectory(out saveFolderPath)) return;
            string selectedClassName = itemList[m_selectedIndex].text;
            switch(selectedClassName)
            {
                case "FloatingCircleHandPoseInfo":
                    HandleFloatingRecord(saveFolderPath);
                    break;
                case "ConstantPointHandPoseInfo":
                    HandleConstantRecord(saveFolderPath);
                    break;

            }

        }

        void HandleFloatingRecord(string savePath)
        {
            var presence = source.CurrentlyHoveringInteractor.transform.parent.transform.GetComponent<HandPresence>();
            if (presence == null) return;
            Transform handTransform = presence.HandPoseOperator.HandObject.transform.GetChild(0);
            HandRecord record = source.GetHandRecord(presence.HandPoseOperator.StructuralInfo);
            FloatingCircleHandPoseInfo poseInfo = new FloatingCircleHandPoseInfo();
            
            poseInfo.Handedness = presence.Handedness;
            
            poseInfo.HandRecord = record;
            
            Vector3 posePosition = source.transform.InverseTransformPoint(handTransform.position);
            Quaternion poseRotation = Quaternion.Inverse(source.transform.rotation) * handTransform.rotation;
            poseInfo.InitialHandPose = new Pose(posePosition, poseRotation);
            
            poseInfo.UpVector = Vector3.up;
            Vector3 realUp = source.transform.TransformVector(poseInfo.UpVector);

            Vector3 newCenter = source.transform.position;
            poseInfo.Radius = Vector3.ProjectOnPlane((handTransform.position - newCenter), realUp).magnitude;

            IOUtils.SaveScriptableObject(savePath, "HandRecord", record);
            IOUtils.SaveScriptableObject(savePath, "PoseRecord", poseInfo);
            source.InteractionPoses.Add(poseInfo);
        }

        void HandleConstantRecord(string savePath)
        {
            var presence = source.CurrentlyHoveringInteractor.transform.parent.transform.GetComponent<HandPresence>();
            if (presence == null) return;
            Transform handTransform = presence.HandPoseOperator.HandObject.transform.GetChild(0);
            Debug.Log(handTransform.name);
            HandRecord record = source.GetHandRecord(presence.HandPoseOperator.StructuralInfo);
            ConstantPointHandPoseInfo poseInfo = new ConstantPointHandPoseInfo();

            poseInfo.Handedness = presence.Handedness;

            poseInfo.HandRecord = record;

            Vector3 posePosition = source.transform.InverseTransformPoint(handTransform.position);
            Quaternion poseRotation = Quaternion.Inverse(source.transform.rotation) * handTransform.rotation;
            poseInfo.InitialHandPose = new Pose(posePosition, poseRotation);

            IOUtils.SaveScriptableObject(savePath, "HandRecord", record);
            IOUtils.SaveScriptableObject(savePath, "PoseRecord", poseInfo);
            source.InteractionPoses.Add(poseInfo);
        }


        bool CreateSaveDirectory(out string result)
        {
            result = "";
            string objectHandDataFolderDirectory = $@"HandPoseData/ObjectPoseDatas/{target.name}";
            IOUtils.CreateDataFolder(objectHandDataFolderDirectory);
            int folderCount = IOUtils.GetFolderCount(objectHandDataFolderDirectory);
            if (folderCount == -1) return false;
            string finalPath = $@"{objectHandDataFolderDirectory}/Pose_{folderCount + 1}";
            IOUtils.CreateDataFolder(finalPath);
            result = finalPath;
            return true;
        }
        Dictionary<string, Type> GetObjectHandInfoBaseClasses()
        {
            var returnList = new Dictionary<string, Type>();
            var childClasses =
                Assembly.GetAssembly(typeof(ObjectHandPoseInfoBase)).GetTypes()
                .Where(myType => 
                    (
                        myType.IsClass 
                        && !myType.IsAbstract 
                        && myType.IsSubclassOf(typeof(ObjectHandPoseInfoBase)
                    )
                ));
            foreach(var cls in childClasses)
            {
                returnList.Add(cls.Name, cls);
            }
            return returnList;
        }

        GUIContent[] FetchList()
        {
            infoBaseClasses = GetObjectHandInfoBaseClasses();
            GUIContent[] returnList = new GUIContent[infoBaseClasses.Count];
            int counter = 0;
            foreach(var info in infoBaseClasses)
            {
                GUIContent newContent = EditorGUIUtility.TrTextContent(info.Key);
                returnList[counter] = newContent;
                counter++;
            }
            itemList = returnList;
            return returnList;
        }
    }
}

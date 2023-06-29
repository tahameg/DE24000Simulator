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
        private bool m_isListInitialized;
        private bool m_showConstantPoses;
        private bool m_showFloatingCirclePoses;

        private int m_selectedIndex = 0;
        private GUIContent[] m_itemList;
        private Dictionary<string, Type> m_infoBaseClasses;
        private HandPoseProvider m_source;
        private MockHandsReferenceHolder m_mockReferenceHolder;
        private Material m_mat;
        
        private ObjectHandPoseInfoBase m_editedHandPoseInfo;
        private bool m_editingActive;
        
        private Vector2[] m_scrollPoses;
        private void OnEnable()
        {
            FetchList();
            var shader = Shader.Find("Hidden/Internal-Colored");
            m_mat = new Material(shader);
            m_source = (HandPoseProvider)target;

            m_mockReferenceHolder = FindObjectOfType<MockHandsReferenceHolder>();

            if(m_mockReferenceHolder == null)
            {
                m_mockReferenceHolder = CreateMockReferenceHolder();
            }
            
            if(m_scrollPoses == null)
            {
                m_scrollPoses = new Vector2[2];
            }
        }

        public void OnSceneGUI()
        {
            if(m_source.InteractionPoses == null) return;

            foreach (var pose in m_source.InteractionPoses)
            {
                if (pose is ConstantPointHandPoseInfo info)
                {
                    HandleConstantPointPoseDraw(info);
                }
                
                
            }
            if (m_editingActive)
            {
                
                foreach (var pose in m_source.InteractionPoses)
                {
                    
                }
            }
        }

        void HandleConstantPointPoseDraw(ConstantPointHandPoseInfo info)
        {
            Handles.color = m_editedHandPoseInfo == info ? Color.yellow :
                info.Handedness == Handedness.Left ? Color.blue : Color.red;
            Vector3 anchorPosition = m_source.transform.InverseTransformPoint(info.InitialHandPose.position);
                    
            Handles.DrawWireCube(anchorPosition, Vector3.one * 0.2f);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(15f);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Interaction Pose Infos");
            m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_itemList);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create New Info"))
            {
                m_source.gameObject.AddComponent(m_infoBaseClasses[m_itemList[m_selectedIndex].text]);
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

            m_showConstantPoses = EditorGUILayout.Foldout(m_showConstantPoses, "Constant Point Poses", EditorStyles.foldoutHeader);
            
            if(m_showConstantPoses)
            {
                int counter = 1;

                m_scrollPoses[0] = EditorGUILayout.BeginScrollView(m_scrollPoses[0], GUILayout.Height(500f));

                foreach (var pose in m_source.InteractionPoses)
                {
                    if (pose is ConstantPointHandPoseInfo)
                    {
                        RenderConstantInteractionPose((ConstantPointHandPoseInfo)pose, $"Pose-{counter}");
                        counter++;
                    }
                }
                EditorGUILayout.EndScrollView();

            }

            m_showFloatingCirclePoses = EditorGUILayout.Foldout(m_showFloatingCirclePoses, "Floating Circle Poses", EditorStyles.foldoutHeader);

            if(m_showFloatingCirclePoses)
            {
                foreach (var pose in m_source.InteractionPoses)
                {
                    if (pose is FloatingCircleHandPoseInfo)
                    {
                        RenderFloatingInteractionPose((FloatingCircleHandPoseInfo)pose);
                    }
                }
            }
        }
        
        void RenderConstantInteractionPose(ConstantPointHandPoseInfo pose, string header)
        {
            SerializedObject obj = new SerializedObject(pose);
            EditorGUILayout.Space(15f);
            Rect lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.width == 1f)
            {
                lastRect.width = EditorGUIUtility.currentViewWidth;
                lastRect.height = 220f;
                
            }
            else
            {
                lastRect.height = 220f;
                lastRect.y += 15f;
            }
            EditorGUI.DrawRect(lastRect, new Color(0.1f, 0.1f, 0.1f));
            
            Color bgColor = GUI.backgroundColor;
            Color textColor = GUI.color;
            
            bool editible = m_editingActive && pose == m_editedHandPoseInfo;
            if (!editible)
            {
                GUI.backgroundColor = new Color(0.2f,0.2f,0.2f);
                GUI.color = Color.gray;
            }
            
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(obj.FindProperty("Handedness"));
            EditorGUILayout.PropertyField(obj.FindProperty("HandRecord"));
            EditorGUILayout.PropertyField(obj.FindProperty("InitialHandPose"));
            EditorGUILayout.PropertyField(obj.FindProperty("CanRotateAroundAxis"));
            EditorGUILayout.PropertyField(obj.FindProperty("RotationAxis"));
            EditorGUILayout.PropertyField(obj.FindProperty("MaxAngle"));
            EditorGUILayout.PropertyField(obj.FindProperty("MinAngle"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(15f);
            
            GUI.backgroundColor = bgColor;
            GUI.color = textColor;
            if (editible)
            {
                obj.ApplyModifiedProperties();
            }
            

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Remove"))
            {
                
            }
            
            if(m_editedHandPoseInfo != null && m_editedHandPoseInfo == pose)
            {
                if (GUILayout.Button("Finish Editing"))
                {
                    m_editedHandPoseInfo = null;
                    m_editingActive = false;
                    // Should Finish editing
                }
                EditorGUILayout.EndHorizontal();
                if(GUILayout.Button("Revert"))
                {
                    
                }

            }
            else
            {
                if (GUILayout.Button("Edit"))
                {
                    m_editedHandPoseInfo = pose;
                    m_editingActive = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            

            EditorGUILayout.EndVertical();
        }

        
        void RenderFloatingInteractionPose(FloatingCircleHandPoseInfo pose)
        {
        }

        // ReSharper disable Unity.PerformanceAnalysis
        void HandleRecord()
        {
            if (m_source.CurrentlyHoveringInteractor == null) return;
            string saveFolderPath;
            if (!CreateSaveDirectory(out saveFolderPath)) return;
            string selectedClassName = m_itemList[m_selectedIndex].text;
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

        // ReSharper disable Unity.PerformanceAnalysis
        void HandleFloatingRecord(string savePath)
        {
            var presence = m_source.CurrentlyHoveringInteractor.transform.parent.transform.GetComponent<HandPresence>();
            if (presence == null) return;
            Transform handTransform = presence.HandPoseOperator.HandObject.transform.GetChild(0);
            HandRecord record = m_source.GetHandRecord(presence.HandPoseOperator.StructuralInfo);
            FloatingCircleHandPoseInfo poseInfo = new FloatingCircleHandPoseInfo();
            
            poseInfo.Handedness = presence.Handedness;
            
            poseInfo.HandRecord = record;
            
            Vector3 posePosition = m_source.transform.InverseTransformPoint(handTransform.position);
            Quaternion poseRotation = Quaternion.Inverse(m_source.transform.rotation) * handTransform.rotation;
            poseInfo.InitialHandPose = new Pose(posePosition, poseRotation);
            
            poseInfo.UpVector = Vector3.up;
            Vector3 realUp = m_source.transform.TransformVector(poseInfo.UpVector);

            Vector3 newCenter = m_source.transform.position;
            poseInfo.Radius = Vector3.ProjectOnPlane((handTransform.position - newCenter), realUp).magnitude;

            IOUtils.SaveScriptableObject(savePath, "HandRecord", record);
            IOUtils.SaveScriptableObject(savePath, "PoseRecord", poseInfo);
            m_source.InteractionPoses.Add(poseInfo);
        }

        void HandleConstantRecord(string savePath)
        {
            var presence = m_source.CurrentlyHoveringInteractor.transform.parent.transform.GetComponent<HandPresence>();
            if (presence == null) return;
            Transform handTransform = presence.HandPoseOperator.HandObject.transform.GetChild(0);
            Debug.Log(handTransform.name);
            HandRecord record = m_source.GetHandRecord(presence.HandPoseOperator.StructuralInfo);
            ConstantPointHandPoseInfo poseInfo = CreateInstance<ConstantPointHandPoseInfo>();

            poseInfo.Handedness = presence.Handedness;

            poseInfo.HandRecord = record;

            Transform sourceTransform = m_source.transform;
            Vector3 posePosition = sourceTransform.InverseTransformPoint(handTransform.position);
            Quaternion poseRotation = Quaternion.Inverse(sourceTransform.rotation) * handTransform.rotation;
            poseInfo.InitialHandPose = new Pose(posePosition, poseRotation);

            IOUtils.SaveScriptableObject(savePath, "HandRecord", record);
            IOUtils.SaveScriptableObject(savePath, "PoseRecord", poseInfo);
            m_source.InteractionPoses.Add(poseInfo);
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
            m_infoBaseClasses = GetObjectHandInfoBaseClasses();
            GUIContent[] returnList = new GUIContent[m_infoBaseClasses.Count];
            int counter = 0;
            foreach(var info in m_infoBaseClasses)
            {
                GUIContent newContent = EditorGUIUtility.TrTextContent(info.Key);
                returnList[counter] = newContent;
                counter++;
            }
            m_itemList = returnList;
            return returnList;
        }

        MockHandsReferenceHolder CreateMockReferenceHolder()
        {
            GameObject holderObject = new GameObject("MockHandReferenceHolder");
            var holder = holderObject.AddComponent<MockHandsReferenceHolder>();
            holder.LoadResources();
            return holder;
        }
    }
}

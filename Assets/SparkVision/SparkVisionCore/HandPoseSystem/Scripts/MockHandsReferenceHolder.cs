using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SparkVision.HandPoseSystem
{
    [ExecuteInEditMode]
    public class MockHandsReferenceHolder : MonoBehaviour
    {
        [SerializeField]
        HandPoseOperator m_leftMockHandPrefab;
        [SerializeField]
        HandPoseOperator m_rightMockHandPrefab;
        
        [SerializeField]
        HandPoseOperator m_leftMockHand;
        [SerializeField]
        HandPoseOperator m_rightMockHand;

        public static MockHandsReferenceHolder Instance;

        private void Start()
        {
            if (Instance != null) return;
            
            Instance = this;

            LoadResources();
        }

        public void LoadResources()
        {
            var left = (GameObject)Resources.Load("LeftHand");
            var right = (GameObject)Resources.Load("RightHand");
            //Remove if the references have already initialized
            if (m_leftMockHand != null)
            {
                DestroyImmediate(m_leftMockHand.gameObject);
                m_leftMockHand = null;
            }

            if (m_rightMockHand != null)
            {
                DestroyImmediate(m_rightMockHand.gameObject);
                m_rightMockHand = null;
            }


            if (m_leftMockHandPrefab == null)
            {
                m_leftMockHand = Instantiate(left).GetComponent<HandPoseOperator>();
                m_leftMockHandPrefab = left.GetComponent<HandPoseOperator>();
            }
            else
            {
                m_leftMockHand = Instantiate(m_leftMockHandPrefab).GetComponent<HandPoseOperator>();
            }

            if (m_rightMockHandPrefab == null)
            {
                m_rightMockHand = Instantiate(right).GetComponent<HandPoseOperator>();
                m_rightMockHandPrefab = right.GetComponent<HandPoseOperator>();
            }
            else
            {
                m_rightMockHand = Instantiate(m_rightMockHandPrefab).GetComponent<HandPoseOperator>();
            }

            ClearHand(Handedness.Left);
            ClearHand(Handedness.Right);
        }

        public HandPoseOperator BringHand(Handedness handedness, Transform parent = null)
        {
            if(m_leftMockHandPrefab == null || m_rightMockHandPrefab == null)
            {
                Debug.LogWarning("Mock hand was not set and thus the HandPoseSystem will not work properly");
                return null;
            }
            GameObject target, targetInstance;
            HandPoseOperator returnVal;
            if(handedness == Handedness.Left)
            {
                target = m_leftMockHandPrefab.gameObject;
                targetInstance = m_leftMockHand?.gameObject;

                if(targetInstance == null)
                {
                    m_leftMockHand = Instantiate(m_leftMockHandPrefab).GetComponent<HandPoseOperator>();
                    targetInstance = m_leftMockHand.gameObject;
                }
                returnVal = m_leftMockHand;
            }
            else
            {
                target = m_rightMockHandPrefab.gameObject;
                targetInstance = m_rightMockHand?.gameObject;

                if (targetInstance == null)
                {
                    m_rightMockHand = Instantiate(m_rightMockHandPrefab).GetComponent<HandPoseOperator>();
                    targetInstance = m_rightMockHand.gameObject;
                }
                returnVal = m_rightMockHand;
            }

            if(parent != null)
            {
                targetInstance.transform.parent = parent;
                targetInstance.transform.localPosition = Vector3.zero;
                targetInstance.transform.localRotation = Quaternion.identity;
            }
            targetInstance.SetActive(true);

            return returnVal;
        }

        public void ClearHand(Handedness handedness)
        {
            if (m_leftMockHandPrefab == null || m_rightMockHandPrefab == null) return;
            if (m_leftMockHand == null || m_rightMockHand == null) return;

            if(handedness == Handedness.Left)
            {
                m_leftMockHand.transform.parent = null;
                m_leftMockHand.gameObject.SetActive(false);
            }
            else
            {
                m_rightMockHand.transform.parent = null;
                m_rightMockHand.gameObject.SetActive(false);
            }
        }
    }
}

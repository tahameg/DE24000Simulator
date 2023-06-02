using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SparkVision.HandPoseSystem
{
    public class MockHandsReferenceHolder : MonoBehaviour
    {
        [SerializeField]
        HandPoseOperator m_leftMockHandPrefab;
        [SerializeField]
        HandPoseOperator m_rightMockHandPrefab;

        HandPoseOperator m_leftMockHand;
        HandPoseOperator m_rightMockHand;

        public static MockHandsReferenceHolder Instance;

        private void Start()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            m_leftMockHand = Instantiate(m_leftMockHandPrefab).GetComponent<HandPoseOperator>();
            m_rightMockHand = Instantiate(m_rightMockHandPrefab).GetComponent<HandPoseOperator>();
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

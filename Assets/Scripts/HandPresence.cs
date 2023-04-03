using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Katana.XR.Controller
{
    public class HandPresence : MonoBehaviour
    {
        [SerializeField]
        XRDirectInteractor m_directInteractor;
        [SerializeField]
        Transform m_handRootTransform;
        [SerializeField]
        Transform m_physicsHandTransform;
        [SerializeField]
        Transform m_visualHandTransform;
        [SerializeField]
        List<Collider> m_touchColliders;


        Rigidbody m_physicsHandRb;

        bool m_initialized;
        private void Start()
        {
            m_physicsHandRb = m_physicsHandTransform.GetComponent<Rigidbody>();
            ShowVisualHand();
            if (m_physicsHandRb != null)
            {
                m_initialized = true;
                ShowPhysicsHand();
            }
            else
            {
                Debug.LogError("Physics hand transform should have a " +
                "Rigidbody component attached");
                ShowVisualHand();
            }
        }

        private void ShowVisualHand()
        {
            m_physicsHandTransform.gameObject.SetActive(false);
            m_visualHandTransform.gameObject.SetActive(true);
        }

        private void ShowPhysicsHand()
        {
            m_physicsHandTransform.gameObject.SetActive(true);
            m_visualHandTransform.gameObject.SetActive(false);
        }

        private void SetTouchColliders(bool state)
        {
            foreach(Collider c in m_touchColliders)
            {
                c.enabled = state;
            }
        }

        private void FixedUpdate()
        {
            if (!m_initialized) return;

            m_physicsHandRb.velocity = CalculateVelocity() / Time.fixedDeltaTime;
            m_physicsHandRb.angularVelocity = CalculateAngularVelocity() / Time.fixedDeltaTime;
            
        }

        Vector3 CalculateVelocity()
        {
            return (m_visualHandTransform.position - m_physicsHandTransform.position);
        }

        Vector3 CalculateAngularVelocity()
        {
            Quaternion rotationDiff = m_visualHandTransform.rotation * Quaternion.Inverse(m_physicsHandTransform.rotation);

            float angle;
            Vector3 axis;
            rotationDiff.ToAngleAxis(out angle, out axis);

            Vector3 rotationDiffV3 = angle * axis;
            return rotationDiffV3 * Mathf.Deg2Rad;
        }

        void SetColor(Transform transform, Color color)
        {
            var renderer = transform.GetComponent<Renderer>();
            MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", color);
        }
    }
}


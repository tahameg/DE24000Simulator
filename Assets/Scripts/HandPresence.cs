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
        Transform m_visualHandTransform;

        bool m_initialized;
        private void Start()
        {
            ShowVisualHand();
        }

        private void ShowVisualHand()
        {
            m_visualHandTransform.gameObject.SetActive(true);
        }

        private void FixedUpdate()
        {
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


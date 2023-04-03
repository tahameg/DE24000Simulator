using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Katana.VR.Locomotion
{
    public class TeleportationManager : MonoBehaviour
    {
        [SerializeField]
        TeleportationProvider teleportationProvider;

        public InputActionProperty TeleportBegin;
        private void Start()
        {
            TeleportBegin.action.performed += OnTeleportBeginAction; 
        }

        void OnTeleportBeginAction(InputAction.CallbackContext context)
        {
            Debug.Log(context.ReadValue<float>());
        }

    }
}

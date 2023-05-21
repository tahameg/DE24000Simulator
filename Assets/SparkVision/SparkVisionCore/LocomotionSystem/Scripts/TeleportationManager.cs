using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    [Range(0.1f, 1f)]
    public float rotationInputThreshold;

    [SerializeField]
    InteractorManager m_interactorManager;

    [SerializeField]
    TeleportationProvider m_teleportationProvider;
    [SerializeField]
    ActionBasedSnapTurnProvider m_snapTurnProvider;
    [SerializeField]
    XRRayInteractor m_teleportationInteractor;

    [SerializeField]
    InputActionProperty Activate;
    [SerializeField]
    InputActionProperty RotationAxis;
    [SerializeField]
    InputActionProperty Cancel;

    bool m_teleportModeActive;
    Vector2 m_lastRotationInput;

    [SerializeField]
    XRDirectInteractor interactor;
       

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        Activate.action.performed += OnTeleportStart;
        Activate.action.canceled += OnTeleportFinish;
        Cancel.action.performed += OnCancel;
        m_interactorManager.Register(m_teleportationInteractor);
    }

    void OnTeleportStart(InputAction.CallbackContext context)
    {
        if (!Cancel.action.ReadValue<bool>())
        {

        }
        if (!m_teleportModeActive)
        {
            m_interactorManager.ActivateInteractor(m_teleportationInteractor);
            m_snapTurnProvider.enabled = false;
            m_teleportModeActive = true;

        }
    }

    void OnTeleportFinish(InputAction.CallbackContext context)
    {
        if (m_teleportModeActive)
        {
            HandleTeleport();
            m_interactorManager.DeactivateInteractor(m_teleportationInteractor);
            m_lastRotationInput = Vector2.zero;
            m_snapTurnProvider.enabled = true;
            m_teleportModeActive = false;
        }
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        if (m_teleportModeActive)
        {
            m_teleportModeActive = false;
            m_lastRotationInput = Vector2.zero;
            m_snapTurnProvider.enabled = true;
            m_interactorManager.DeactivateInteractor(m_teleportationInteractor);
        }
    }

    void HandleTeleport()
    {
        if (m_teleportationInteractor.enabled)
        {
            RaycastHit rayHit;
            bool result  = m_teleportationInteractor.TryGetCurrent3DRaycastHit(out rayHit);
            if (result)
            {
                TeleportRequest tr = new TeleportRequest();
                tr.destinationPosition = rayHit.point;
                m_teleportationProvider.QueueTeleportRequest(tr);
            }
        }
    }


    /*
    Quaternion CalculateRotation()
    {
        if(m_lastRotationInput == Vector2.zero)
        {
            return Quaternion.LookRotation(Vector3.ProjectOnPlane(m_teleportationInteractor.transform.forward, Vector3.up), Vector3.up);
        }

        Vector3 asWorldVector = new Vector3(m_lastRotationInput.x, 0f, m_lastRotationInput.y);
        Quaternion fromTo = Quaternion.FromToRotation(Vector3.forward, asWorldVector);
        Vector3 translated = (fromTo * Vector3.ProjectOnPlane(m_teleportationInteractor.transform.forward, Vector3.up).normalized);
        return Quaternion.LookRotation(translated, Vector3.up);
    }
    */

    private void Update()
    {
       
    }
}

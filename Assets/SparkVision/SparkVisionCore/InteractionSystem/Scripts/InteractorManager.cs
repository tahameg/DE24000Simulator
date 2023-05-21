using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractorManager : MonoBehaviour
{
    [SerializeField]
    public List<XRRayInteractor> InteractorsToRegisterAtStart;

    List<XRBaseInteractor> m_registeredInteractors;

    public XRBaseInteractor ActiveInteractor { get; private set; }

    private void Start()
    {
        Initialize();
    }
    public bool Register<T>(T interactor, bool activate = false) where T: XRBaseInteractor
    {
        if (interactor == null) return false;
        if (!m_registeredInteractors.Contains(interactor))
        {
            m_registeredInteractors.Add(interactor);
            if (activate)
            {
                ActivateInteractor(interactor);
            }
            else
            {
                interactor.enabled = false;
            }
            return true;
        }
        return false;
    }

    public bool Unregister<T>(T interactor) where T : XRBaseInteractor
    {
        if (interactor == null) return false;
        if (m_registeredInteractors.Contains(interactor))
        {
            DeactivateInteractor(interactor);
            m_registeredInteractors.Remove(interactor);
            return true;
        }
        return false;
    }

    public void ActivateInteractor<T>(T interactor ) where T: XRBaseInteractor
    {
        if (interactor == null) return;
        if (m_registeredInteractors.Contains(interactor))
        {
            if (ActiveInteractor != null && ActiveInteractor != interactor)
            {
                ActiveInteractor.enabled = false;
            }
            ActiveInteractor = interactor;
            interactor.enabled = true;
        }
    }

    public void DeactivateInteractor<T>(T interactor) where T : XRBaseInteractor
    {
        if (interactor == null) return;
        if (m_registeredInteractors.Contains(interactor))
        {
           if(ActiveInteractor == interactor)
            {
                interactor.enabled = false;
                if(interactor is XRRayInteractor)
                {
                    var visualizer = interactor.transform.GetComponent<XRInteractorLineVisual>();
                    visualizer?.reticle?.SetActive(false);
                    visualizer?.blockedReticle?.SetActive(false);
                }
                ActiveInteractor = null;
            }
        }
    }

    public void DeactivateAll()
    {
        foreach(var interactor in m_registeredInteractors)
        {
            interactor.enabled = false;
            ActiveInteractor = null;
        }
    }

    private void Initialize()
    {
        m_registeredInteractors = new List<XRBaseInteractor>();
        if(InteractorsToRegisterAtStart != null)
        {
            foreach(var interactor in InteractorsToRegisterAtStart)
            {
                Register(interactor);
            }
        }
        DeactivateAll();
    }



}

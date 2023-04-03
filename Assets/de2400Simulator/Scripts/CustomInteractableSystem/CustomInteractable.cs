using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Filtering;


public class CustomInteractable : XRBaseInteractable
{
    [Header("CustomInteractableParameters")]
    public Transform GrabTransform;
    public List<IXRGrabProcess> InitialProcessors;

    List<IXRGrabProcess> m_processors;

    readonly HashSetList<XRDirectInteractor> m_directInteractorsHovering = new HashSetList<XRDirectInteractor>();
    XRDirectInteractor m_currentSelectingInteractor;

    public List<XRDirectInteractor> DirectInteractorsHovering => (List<XRDirectInteractor>)m_directInteractorsHovering.AsList();
    
    public XRDirectInteractor CurrentSelectingInteractor
    {
        get => m_currentSelectingInteractor;
        set => m_currentSelectingInteractor = value;
    }


    [SerializeField]
    HoverEnterEvent m_directInteractorHoverEnter;

    public HoverEnterEvent DirectInteractorHoverEntered
    {
        get => m_directInteractorHoverEnter;
        set => m_directInteractorHoverEnter = value;
    }

    [SerializeField]
    HoverExitEvent m_directInteractorHoverExit;

    public HoverExitEvent DirectInteractorHoverExit
    {
        get => m_directInteractorHoverExit;
        set => m_directInteractorHoverExit = value;
    }

    [SerializeField]
    SelectEnterEvent m_directInteractorSelectEnter;

    [SerializeField]
    SelectExitEvent m_directInteractorSelectExit;

    [SerializeField]
    UnityAction<CustomInteractionArgs> OnInteractionBegin;

    [SerializeField]
    UnityAction<CustomInteractionArgs> OnInteractionUpdate;

    [SerializeField]
    UnityAction<CustomInteractionArgs> OnInteractionExit;

    CustomInteractionArgs m_interactionArgs = new CustomInteractionArgs();

    bool m_currentlySelecting;
    protected override void Awake()
    {
        base.Awake();
        if(GrabTransform == null)
        {
            GrabTransform = transform;
        }
        if(InitialProcessors != null)
        {
            m_processors = new List<IXRGrabProcess>(InitialProcessors);
        }
        else
        {
            m_processors = new List<IXRGrabProcess>();
        }
        IXRGrabProcess[] componentProcessors = GetComponents<IXRGrabProcess>();
        foreach(var processor in componentProcessors)
        {
            if (!m_processors.Contains(processor))
            {
                m_processors.Add(processor);
            }
        }
    }

    private void Update()
    {
        if (m_currentlySelecting)
        {
            UpdateInteractionArgs(CurrentSelectingInteractor, ref m_interactionArgs);
            InvokeProcessorsUpdate(m_interactionArgs);
        }
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        Debug.Log("HoverEntered");
    }

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        base.OnHoverEntering(args);
        if (!(args.interactorObject is XRDirectInteractor)) return;
        
        XRDirectInteractor asDirectInteractor = (XRDirectInteractor)args.interactorObject;
        if (!m_directInteractorsHovering.Contains(asDirectInteractor))
        {
            m_directInteractorsHovering.Add(asDirectInteractor);
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        Debug.Log("HoverExited");
    }

    protected override void OnHoverExiting(HoverExitEventArgs args)
    {
        base.OnHoverExiting(args);
        if (!(args.interactorObject is XRDirectInteractor)) return;
        
        XRDirectInteractor asDirectInteractor = (XRDirectInteractor)args.interactorObject;
        if (m_directInteractorsHovering.Contains(asDirectInteractor))
        {
            m_directInteractorsHovering.Remove(asDirectInteractor);
        }
    }


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!(args.interactorObject is XRDirectInteractor)) return;
        if(m_currentSelectingInteractor == null)
        {
            XRDirectInteractor asDirectInteractor = (XRDirectInteractor)args.interactorObject;
            m_currentSelectingInteractor = asDirectInteractor;
            UpdateInteractionArgs(asDirectInteractor, ref m_interactionArgs);
            InvokeProcessorsBegin(m_interactionArgs);
            OnInteractionBegin?.Invoke(m_interactionArgs);
            m_currentlySelecting = true;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (!(args.interactorObject is XRDirectInteractor)) return;
        if(m_currentSelectingInteractor != null )
        {
            XRDirectInteractor asDirectInteractor = (XRDirectInteractor)args.interactorObject;
            if(m_currentSelectingInteractor == asDirectInteractor)
            {
                m_currentlySelecting = false;
                m_currentSelectingInteractor = null;
                UpdateInteractionArgs(asDirectInteractor, ref m_interactionArgs);
                InvokeProcessorsExit(m_interactionArgs);
                OnInteractionExit?.Invoke(m_interactionArgs);
                m_currentlySelecting = false;
            }
        }
    }

    void InvokeProcessorsBegin(CustomInteractionArgs args)
    {
        foreach(var processor in m_processors)
        {
            processor.StartProcess(args);
        }
    }

    void InvokeProcessorsUpdate(CustomInteractionArgs args)
    {
        foreach (var processor in m_processors)
        {
            processor.UpdateProcess(args);
        }
    }

    void InvokeProcessorsExit(CustomInteractionArgs args)
    {
        foreach (var processor in m_processors)
        {
            processor.ExitProcess(args);
        }
    }

    void UpdateInteractionArgs(XRDirectInteractor interactor, ref CustomInteractionArgs args )
    {
        Pose globalPose = new Pose(interactor.attachTransform.position, interactor.attachTransform.rotation);
        Pose localPose = new Pose(interactor.attachTransform.localPosition, interactor.attachTransform.localRotation);
        args.InteractionPose = globalPose;
        args.InteractionLocalPose = localPose;
        args.GrabTransform = GrabTransform;
    }
}

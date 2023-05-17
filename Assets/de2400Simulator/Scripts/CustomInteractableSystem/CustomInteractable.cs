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
    /// <summary>
    /// This parameter defines the grab transform for the interactable object. 
    /// The grab transform determines where the object should be held or manipulated from. 
    /// For example, if the object is a door, the grab transform should be set to the door handle. 
    /// Make sure to set this parameter to the appropriate transform for your specific interactable object. 
    /// </summary>
    [Header("CustomInteractableParameters")]
    [Tooltip("Defines the grab transform for the interactable object. " +
        "The grab transform determines where the object should be held or " +
        "manipulated from. For example, if the object is a door, the grab " +
        "transform should be set to the door handle. If not set, will be set" + 
        " to transform of the interactable. ")]
    public Transform GrabTransform;

    /// <summary>
    /// Initial processors are registered at start.
    /// </summary>
    [Tooltip("Initial processors are registered at start.")]
    public List<IXRGrabProcess> InitialProcessors;
    List<IXRGrabProcess> m_processors;

    /// <summary>
    /// There is only one XR interactor allowed to select at a time.
    /// This behaviour can be changed in the future.
    /// </summary>
    XRDirectInteractor m_currentSelectingInteractor;

    readonly HashSetList<XRDirectInteractor> m_directInteractorsHovering = new HashSetList<XRDirectInteractor>();

    /// <summary>
    /// List of the hovering direct interactors.
    /// </summary>
    public List<XRDirectInteractor> DirectInteractorsHovering => (List<XRDirectInteractor>)m_directInteractorsHovering.AsList();

    /// <summary>
    /// The single currently selected interactor.
    /// </summary>
    public XRDirectInteractor CurrentSelectingInteractor
    {
        get => m_currentSelectingInteractor;
        set => m_currentSelectingInteractor = value;
    }

    /*
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
    */

    //Todo
    //SelectEnterEvent m_directInteractorSelectEnter;
    //SelectExitEvent m_directInteractorSelectExit;

    [HideInInspector]
    public UnityAction<CustomInteractionArgs> OnInteractionBegin;

    [HideInInspector]
    public UnityAction<CustomInteractionArgs> OnInteractionUpdate;

    [HideInInspector]
    public UnityAction<CustomInteractionArgs> OnInteractionExit;

    CustomInteractionArgs m_interactionArgs = new CustomInteractionArgs();

    //Is the interactable selected by any direct interactor.
    bool m_currentlySelecting;
   
     protected override void Awake()
    {
        base.Awake();
        //Set grab transfrom if not set
        if(GrabTransform == null)
        {
            GrabTransform = transform;
        }

        //Initialize the processors
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
        // Update only dispatches the latest state if there is a interactor currently selected.
        // m_currentlySelecting is handled by OnHoverEntering and OnHoverExiting callbacks.
        if (m_currentlySelecting)
        {
            //Modify the interaction args to be sent
            UpdateInteractionArgs(CurrentSelectingInteractor, ref m_interactionArgs);
            InvokeProcessorsUpdate(m_interactionArgs);
        }
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


    // What happens if the object is currently selected by another interactor. Shouldn't you call
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!(args.interactorObject is XRDirectInteractor)) return;
        XRDirectInteractor asDirectInteractor = (XRDirectInteractor)args.interactorObject;

        //If the interactor is already selected, do nothing.
        if (m_currentSelectingInteractor != null && m_currentSelectingInteractor == asDirectInteractor)
            return;

        //If there is anothere interactor that is selecting, end its interaction.
        //This will automatically call the OnSelectExited.
        if(m_currentSelectingInteractor != null)
        {
            var arg = new SelectExitEventArgs();
            arg.interactableObject = this;
            arg.isCanceled = false;
            arg.manager = interactionManager;
            arg.interactorObject = m_currentSelectingInteractor;
            OnSelectExited(arg);
        }
        m_currentSelectingInteractor = asDirectInteractor;
        UpdateInteractionArgs(asDirectInteractor, ref m_interactionArgs);
        InvokeProcessorsBegin(m_interactionArgs);
        OnInteractionBegin?.Invoke(m_interactionArgs);
        m_currentlySelecting = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (!(args.interactorObject is XRDirectInteractor)) 
            return;
        
        XRDirectInteractor asDirectInteractor = (XRDirectInteractor)args.interactorObject;
        
        if (m_currentSelectingInteractor == null)
            return;

        if (m_currentSelectingInteractor != asDirectInteractor)
            return;

        m_currentlySelecting = false;
        m_currentSelectingInteractor = null;
        UpdateInteractionArgs(asDirectInteractor, ref m_interactionArgs);
        InvokeProcessorsExit(m_interactionArgs);
        OnInteractionExit?.Invoke(m_interactionArgs);
        m_currentlySelecting = false;
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
        args.Interactor = interactor;
    }
}

public class CustomInteractionArgs
{
    public Pose InteractionPose;
    public Pose InteractionLocalPose;
    public Transform GrabTransform;
    public XRDirectInteractor Interactor;
    public CustomInteractionArgs(Pose interactionPose, Pose interactionLocalPose)
    {
        InteractionPose = interactionPose;
        InteractionLocalPose = interactionLocalPose;
    }

    public CustomInteractionArgs(Pose interactionPose, Pose interactionLocalPose, XRDirectInteractor interactor)
        : this(interactionPose, interactionLocalPose)
    {
        Interactor = interactor;
    }
    public CustomInteractionArgs() { }

}



//Todo: system behaviour is not defined for edge cases.

//Todo: system only works with the single hand. Default Behaviour:
// * If no hand is currently interacting. Interaction begins.
// * If one hand is interacting, the interaction is ended for one hand
// and begins for the other hand.
// * In this case, select end is called for one hand and select begin is called for the other hand.
// Todo: Interaction is only called on select. It can also start on hover. Rewrite the system accordingly.




using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomInteractionArgs
{
    public Pose InteractionPose;
    public Pose InteractionLocalPose;
    public Transform GrabTransform;
    public CustomInteractionArgs(Pose interactionPose, Pose interactionLocalPose)
    {
        InteractionPose = interactionPose;
        InteractionLocalPose = interactionLocalPose;
    }

    public CustomInteractionArgs() { }

}

public interface IXRGrabProcess 
{
    void StartProcess(CustomInteractionArgs args);
    void UpdateProcess(CustomInteractionArgs args);
    void ExitProcess(CustomInteractionArgs args);
}

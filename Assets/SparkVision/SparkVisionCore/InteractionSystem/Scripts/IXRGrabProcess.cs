using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SparkVision.InteractionSystem
{
    public interface IXRGrabProcess
    {
        void StartProcess(CustomInteractionArgs args);
        void UpdateProcess(CustomInteractionArgs args);
        void ExitProcess(CustomInteractionArgs args);
    }

}
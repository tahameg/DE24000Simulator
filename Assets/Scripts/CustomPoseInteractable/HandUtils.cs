using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Katana.XR.Interactables.HandPoseSystem;
using UnityEngine;

namespace Katana.XR.Interactables.HandPoseSystem.Utils
{
    public static class HandUtils
    {
        public static HandStructuralInfo AutoGenerateConfiguration(GameObject HandObject, string[] fingerPrefixes)
        {
            if (HandObject == null) return null;
            HandStructuralInfo returnValue = new HandStructuralInfo();
            List<FingerStructuralInfo> fingerInfos = new List<FingerStructuralInfo>();
            for (int i = 0; i < fingerPrefixes.Length; i++)
            {
                var jointInfo = GetAllJointsWithPrefix(HandObject.transform, fingerPrefixes[i]);
                if (jointInfo != null)
                {
                    FingerStructuralInfo fingerInfo = new FingerStructuralInfo(jointInfo.ToArray());
                    fingerInfos.Add(fingerInfo);
                }
            }

            returnValue.FingerConfiguration = fingerInfos.ToArray();
            return returnValue;
        }

        static Transform FindFirstChildMatch(Transform transform, string prefix)
        {
            Regex rx = new Regex(@".*" + prefix + @".*", RegexOptions.IgnoreCase);
            Transform returnTranform = null;

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (rx.IsMatch(child.name))
                {
                    return child.transform;
                }
                else
                {
                    returnTranform = FindFirstChildMatch(child.transform, prefix);
                    if (returnTranform != null)
                    {
                        return returnTranform;
                    }
                }
            }

            return returnTranform;
        }

        static List<InitializedTransform> GetAllJointsWithPrefix(Transform transform, string prefix)
        {
            List<InitializedTransform> found = new List<InitializedTransform>(); ;
            Transform newTransform = FindFirstChildMatch(transform, prefix);
            while (newTransform != null)
            {
                InitializedTransform it = new InitializedTransform(newTransform,
                    new Pose(newTransform.localPosition, newTransform.localRotation));
                found.Add(it);
                newTransform = FindFirstChildMatch(newTransform, prefix);
            }

            if (found.Count == 0) return null;
            return found;
        }
    }
}

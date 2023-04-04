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

        public static List<Transform> GetAllJointsWithPrefix(Transform transform, string prefix)
        {
            List<Transform> found = new List<Transform>(); ;
            Transform newTransform = FindFirstChildMatch(transform, prefix);
            while (newTransform != null)
            {
                Transform t = newTransform;
                found.Add(t);
                newTransform = FindFirstChildMatch(newTransform, prefix);
            }

            if (found.Count == 0) return null;
            return found;
        }
    }
}

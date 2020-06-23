using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculates the Tipdistances between the skeleton
/// 
/// Author: Cédric Girardin
/// </summary>
public static class HandTipData
{
    /// <summary>
    /// Calculates Tip distances between finger form given skeleton
    /// </summary>
    /// <param name="skeleton">Skelteton from Hand that should be calculated</param>
    /// <returns>tip distances in a array from thumb to pinky</returns>
    public static float[] GetCurrentTipDistances(OVRSkeleton skeleton)
    {
        int tipStartId = (int)OVRSkeleton.BoneId.Hand_MaxSkinnable;
        int tipAmount = (int)(OVRSkeleton.BoneId.Max - tipStartId);
        float[] tempTipDistances = new float[tipAmount];

        for (int p = 1; p < tipAmount + 1; p++)
        {
            OVRBone startBone = skeleton.Bones[tipStartId + p - 1];

            OVRBone endBone = p == tipAmount ? skeleton.Bones[tipStartId] : skeleton.Bones[tipStartId + p];

            tempTipDistances[p - 1] = CalculateTipDistance(startBone, endBone);
        }
        return tempTipDistances;
    }

    private static float CalculateTipDistance(OVRBone startBone, OVRBone endBone)
    {
        Vector3 startPos = startBone.Transform.position;
        Vector3 endPos = endBone.Transform.position;

        return Vector3.Distance(startPos, endPos) * 1000;
    }

}

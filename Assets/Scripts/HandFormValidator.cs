using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Compares 2 Handforms by subtracting their values hand validates them
/// 
/// Author: Cédric Girardin
/// </summary>
public class HandFormValidator : MonoBehaviour
{
    #region Properties
    public DebugCanvas DebugCanvas;
    public OVRHand CurrentUserHand;
    public HandFormRenderer VirtualHand;

    public float MaxAngleDiff = 20.0f;
    public float MaxTippDiff = 15.0f;
    public float MaxWristDiff = 80.0f;

    public float FingerAngleWeight = 0.75f;
    public float TipDistanceWeight = 1.25f;
    public float WristOrientationWeight = 1.0f;

    // Wristbone id 
    private static int WristId = (int)OVRSkeleton.BoneId.Hand_WristRoot;

    // Intermediate- and Distalbone id for Thumb
    private static int[] ThumbIds = { (int)OVRSkeleton.BoneId.Hand_Thumb2,
                                      (int)OVRSkeleton.BoneId.Hand_Thumb3};

    // Intermediate- and Distalbone id for Indexfinger
    private static int[] IndexIds = { (int)OVRSkeleton.BoneId.Hand_Index2,
                                        (int)OVRSkeleton.BoneId.Hand_Index3};

    // Intermediate- and Distalbone id for Middlefinger
    private static int[] MiddleIds = { (int)OVRSkeleton.BoneId.Hand_Middle2,
                                        (int)OVRSkeleton.BoneId.Hand_Middle2};

    // Intermediate- and Distalbone id for Ringfinger
    private static int[] RingIds = { (int)OVRSkeleton.BoneId.Hand_Ring2,
                                        (int)OVRSkeleton.BoneId.Hand_Ring2};

    // Intermediate- and Distalbone id for Pinkyfinger
    private static int[] PinkyIds = { (int)OVRSkeleton.BoneId.Hand_Pinky2,
                                        (int)OVRSkeleton.BoneId.Hand_Pinky3};

    // All Intermediate- and Distalbone ids
    private static int[][] FingerIds = { ThumbIds, IndexIds, MiddleIds, RingIds, PinkyIds };

    private OVRSkeleton _CurrentSkeleton;
    private GameObject _CurrentGO;
    private string _TrimString = "Hand_";


    #endregion

    #region UnityFunctions

    private void Awake()
    {
        _CurrentSkeleton = CurrentUserHand.gameObject.GetComponent<OVRSkeleton>();
        _CurrentGO = CurrentUserHand.transform.parent.gameObject;
        
    }

    #endregion

    #region HandFormValidatorFunctions

    /// <summary>
    /// Checks if the current Handform is approximetly the same with the handForm parameter
    /// </summary>
    /// <param name="handForm">The Handform that should be compared</param>
    /// <returns>Ture when Hands match, false when not</returns>
    public bool IsHandCorrect(HandForm handForm)
    {
        DebugCanvas.Clear();
        if (CanCompare(handForm))
        {
            DebugCanvas.Log("Start comparing:");
            HandForm currentHand = new HandForm(_CurrentSkeleton.Bones, 
                                                HandTipData.GetCurrentTipDistances(_CurrentSkeleton), 
                                                _CurrentGO);

            return CompareHands(handForm, currentHand);
        }
        DebugCanvas.Log("cant compare!");
        return false;
    }

    private bool CompareHands(HandForm savedHand, HandForm currentHand)
    {
        // Compare Handorientation
        float orientationDiff = HandOrientationDiff(savedHand.SavedBones[WristId],
                                                       currentHand.SavedBones[WristId]);

        DebugCanvas.Log("HandOrientationDiff: " + orientationDiff);


        // compare fingerangle
        float[] fingersDiff = CompareFingerAngles(savedHand, currentHand);

        // compare tip distances
        float[] tipDiff = CompareTipDiff(savedHand, currentHand);


        // Evaluate score
        bool handOk = EvaluateScore(orientationDiff, fingersDiff, tipDiff);

        // Set Color of virtual Hand
        VirtualHand.SetMeshMaterialAcitve(handOk);
        DebugCanvas.Log("CorrectHandform:" + handOk);

        return handOk;


    }

    private bool CanCompare(HandForm handForm)
    {
        return handForm.BoneGO != null && CurrentUserHand.IsTracked;
    }


    #region HandOrientationFunctions
    private float HandOrientationDiff(OVRBone savedWrist, OVRBone currentWrist)
    {
        Vector3 savedDir = savedWrist.Transform.up;
        Vector3 currentDir = currentWrist.Transform.up;

        float angleDiff = Mathf.Abs(Vector3.Angle(savedDir, currentDir));

        VirtualHand.SetWristOrientationActive(angleDiff < MaxWristDiff);

        return angleDiff;
        
    }
    #endregion

    #region FingerAngleFunctions

    private float[] CompareFingerAngles(HandForm savedFingers, HandForm currentFingers)
    {
        float[] fingersDiff = new float[FingerIds.Length];
        for (int i = 0; i < FingerIds.Length; i++)
        {
            fingersDiff[i] = FingerAngleDiff(savedFingers, currentFingers, FingerIds[i]);

            // find name of the Bone for DebugCanvas
            string fingerEnumName = Enum.GetName(typeof(OVRSkeleton.BoneId), FingerIds[i][0]);
            string fingerName = fingerEnumName.Trim(_TrimString.ToCharArray());
            DebugCanvas.Log(fingerName + ": " + fingersDiff[i]);
        }

        return fingersDiff;
    }

    private float FingerAngleDiff(HandForm savedFingers, HandForm currentFingers, int[] ids)
    {
        float diffInter = CompareZAngles(savedFingers.SavedBones[ids[0]].Transform.localEulerAngles,
                                         currentFingers.SavedBones[ids[0]].Transform.localEulerAngles);

        float diffDistal = CompareZAngles(savedFingers.SavedBones[ids[1]].Transform.localEulerAngles,
                                          currentFingers.SavedBones[ids[1]].Transform.localEulerAngles);

        VirtualHand.SetBoneActive(ids[0], diffInter < MaxAngleDiff);
        VirtualHand.SetBoneActive(ids[1], diffDistal < MaxAngleDiff);

        return (diffInter * 1f + diffDistal * 0.75f) / 2;
    } 

    private float CompareZAngles(Vector3 v1, Vector3 v2)
    {
        return Mathf.Abs(Mathf.DeltaAngle(v1.z, v2.z));
    }
    #endregion

    #region TipDistanceFunctions

    private float[] CompareTipDiff(HandForm savedTips, HandForm currentTips)
    {
        float[] tipDiff = new float[savedTips.SavedTipDistances.Length];
        for (int i = 0; i < tipDiff.Length - 1; i++)
        {
            tipDiff[i] = TipDistanceDiff(savedTips, currentTips, i);

            DebugCanvas.Log("TipDiff" + i + ": " + tipDiff[i]);
        }

        return tipDiff;
    }

    private float TipDistanceDiff(HandForm savedHand, HandForm currentHand, int id)
    {
        float diffDistance = CalcDiffDistance(savedHand.SavedTipDistances[id], currentHand.SavedTipDistances[id]);

        VirtualHand.SetTipActive(id, diffDistance < MaxTippDiff);

        return diffDistance;
    }

    private float CalcDiffDistance(float distance1, float distance2)
    {
        return Mathf.Abs(distance1 - distance2);
    }

    #endregion

    #region ScoreFunctions
    private bool EvaluateScore(float orientationDiff, float[] fingersDiff, float[] tipsDiff)
    {
        
        float averageFingerDiff = CalculateAverage(fingersDiff);
        float averageTipDiff = CalculateAverage(tipsDiff);

        return (
                    (orientationDiff * WristOrientationWeight) < MaxWristDiff &&
                    (averageFingerDiff * FingerAngleWeight) < MaxAngleDiff &&
                    (averageTipDiff * TipDistanceWeight) < MaxTippDiff
                
                );

        
    }

    private float CalculateAverage(float[] values)
    {
        float total = 0;
        foreach(float value in values)
        {
            total += value;
        }
        return (total / values.Length);
    }
    #endregion

    #endregion

}

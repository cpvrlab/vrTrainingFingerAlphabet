using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// struct wich saves all important information to display and compare a Hand
/// 
/// Author: Cédric Girardin
/// </summary>
public struct HandForm
{
    #region Properties

    public char AlphabeticCharacter;
    public IList<OVRBone> SavedBones;
    public IList<OVRBone> SavedBindPoses;
    public GameObject BoneGO;
    public float[] SavedTipDistances;

    #endregion

    #region Constructors
    /// <summary>
    /// HandForm constructor to comapre other HandForms to
    /// Not suitable for saving a HandForm!
    /// </summary>
    /// <param name="bones">Bones form the OVRSkeleton</param>
    /// <param name="tipDistances">float array with the distances between the tipbones</param>
    /// <param name="boneGO">the root of the Hand</param>
    public HandForm( IList<OVRBone> bones, float[] tipDistances, GameObject boneGO)
    {
        AlphabeticCharacter = '-';
        SavedBones = bones;
        SavedBindPoses = null;
        SavedTipDistances = tipDistances;
        BoneGO = boneGO;
    }


    /// <summary>
    /// HandForm consturctor with all properties
    /// Suitable for saving a Handform
    /// </summary>
    /// <param name="alphabetChar">The alphabetic character corresponding to the HandForm in ASL</param>
    /// <param name="bones">Bones from the OVRSkeleton</param>
    /// <param name="bindPoses">BindPoses from the OVRSkeleton</param>
    /// <param name="tipDistances">float array with the distances between the tipbones</param>
    /// <param name="boneGO">The root of the hand</param>
    public HandForm(char alphabetChar, List<OVRBone> bones, List<OVRBone> bindPoses, float[] tipDistances, GameObject boneGO)
    {
        AlphabeticCharacter = alphabetChar;
        SavedBones = bones;
        SavedBindPoses = bindPoses;
        SavedTipDistances = tipDistances;
        BoneGO = boneGO;
    }


    #endregion

    #region HandFormFunctions

    /// <summary>
    /// Converts HandForm into a String and displays all values and lengths of the lists
    /// </summary>
    /// <returns>String with 4 new lines</returns>
    public override string ToString()
    {
        return (
                    AlphabeticCharacter.ToString() + "\n" +
                    "Bones Length: " + SavedBones.Count + "\n" +
                    "BindPoses Length: " + SavedBindPoses.Count + "\n" +
                    "BoneGo"            + BoneGO + "\n" +
                    "tipDistances Length: " + SavedTipDistances.Length


                );
    }

    #endregion
}

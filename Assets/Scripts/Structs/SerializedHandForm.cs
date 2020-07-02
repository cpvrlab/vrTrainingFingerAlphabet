using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// struct wich saves all important information to display and compare a Hand
/// 
/// Author: Cédric Girardin
/// </summary>
[System.Serializable]
public struct SerializedHandForm
{
    #region Properties

    public char AlphabeticCharacter;
    public List<SerializedOVRBone> SavedBones;
    public List<SerializedOVRBone> SavedBindPoses;
    // NOTE: Create a GameObject
    public SerializedTransform BoneGO;
    public float[] SavedTipDistances;
    public Language[] languages;
    public OVRHand.Hand handedness;

    #endregion

    public SerializedHandForm(char alphabetChar, 
                              List<SerializedOVRBone> bones, 
                              List<SerializedOVRBone> bindPoses, 
                              float[] tipDistances, 
                              GameObject boneGO, 
                              Language[] lang,
                              OVRHand.Hand hand)
    {
        AlphabeticCharacter = alphabetChar;
        SavedBones = bones;
        SavedBindPoses = bindPoses;
        SavedTipDistances = tipDistances;
        BoneGO = new SerializedTransform(boneGO.transform);
        languages = lang;
        handedness = hand;
    }

    public SerializedHandForm(HandForm handform)
    {
        AlphabeticCharacter = handform.AlphabeticCharacter;
        SavedBones          = SerializeBoneList(handform.SavedBones);
        SavedBindPoses      = SerializeBoneList(handform.SavedBones);
        BoneGO              = new SerializedTransform(handform.BoneGO.transform);
        SavedTipDistances   = handform.SavedTipDistances;
        languages           = handform.languages;
        handedness          = handform.handedness;
    }

    public override string ToString()
    {
        return (
                    AlphabeticCharacter.ToString() + "\n" +
                    "Bones Length: " + SavedBones.Count + "\n" +
                    "BindPoses Length: " + SavedBindPoses.Count + "\n" +
                    "BoneGo: " + BoneGO + "\n" +
                    "tipDistances Length: " + SavedTipDistances.Length + "\n" +
                    "Handedness: " + Enum.GetName(typeof(OVRHand.Hand), handedness)

                );
    }

    private static List<SerializedOVRBone> SerializeBoneList(IList<OVRBone> bones)
    {
        List<SerializedOVRBone> sBones = new List<SerializedOVRBone>();

        foreach (OVRBone bone in bones)
            sBones.Add(new SerializedOVRBone(bone));

        return sBones;
    }

  
}
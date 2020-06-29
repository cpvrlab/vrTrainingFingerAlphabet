using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #endregion

    public SerializedHandForm(char alphabetChar, List<SerializedOVRBone> bones, List<SerializedOVRBone> bindPoses, float[] tipDistances, GameObject boneGO, Language[] lang)
    {
        AlphabeticCharacter = alphabetChar;
        SavedBones = bones;
        SavedBindPoses = bindPoses;
        SavedTipDistances = tipDistances;
        BoneGO = new SerializedTransform(boneGO.transform);
        languages = lang;
    }

    public SerializedHandForm(HandForm handform)
    {
        AlphabeticCharacter = handform.AlphabeticCharacter;
        SavedBones          = SerializeBoneList(handform.SavedBones);
        SavedBindPoses      = SerializeBoneList(handform.SavedBones);
        BoneGO              = new SerializedTransform(handform.BoneGO.transform);
        SavedTipDistances   = handform.SavedTipDistances;
        languages           = handform.languages;
    }

    public override string ToString()
    {
        return (
                    AlphabeticCharacter.ToString() + "\n" +
                    "Bones Length: " + SavedBones.Count + "\n" +
                    "BindPoses Length: " + SavedBindPoses.Count + "\n" +
                    "BoneGo" + BoneGO + "\n" +
                    "tipDistances Length: " + SavedTipDistances.Length


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
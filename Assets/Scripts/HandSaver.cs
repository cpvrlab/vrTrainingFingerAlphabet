using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

/// <summary>
/// converts the current Hand from the User in a HandForm object
/// Saves a HandForm object in a Textfile
/// 
/// Author: Cédric Girardin
/// </summary>
public class HandSaver : MonoBehaviour
{
    #region Properties

    public OVRSkeleton CurrentSkeleton;
    public Transform HandAnchor;
    public OVRHand CurrentHand;

    // All possible Alphabet letters
    private static char[] ALPHABET = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'S', 'T', 'U', 'V', 'W', 'X', 'Y' };

    private string _FileName;
    private char _CurrentAlphabetLetter;
    private int _AlphabetId;
    private List<OVRBone> _TempBones;
    private List<OVRBone> _TempBindPoses;
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        _AlphabetId = 0;
        _CurrentAlphabetLetter = ALPHABET[_AlphabetId];
    }
    #endregion

    #region HandSaverFunctions
    /// <summary>
    /// Converts the current Hand in a HandForm Object and Saves it in a Textfile
    /// </summary>
    public void SaveHand()
    {
        try
        {
            HandForm nHandForm = GenerateFromCurrentHandForm();

            SaveHandFormToTxt(nHandForm);

        }
        catch(System.Exception e)
        {
            Debug.Log(e.ToString());
        }

        GetNextAlphabetLetter();

    }

    private void GetNextAlphabetLetter()
    {
        _AlphabetId++;
        _CurrentAlphabetLetter = ALPHABET[_AlphabetId];
    }

    #region GenerateHandForm
    private HandForm GenerateFromCurrentHandForm()
    {
        _TempBones = new List<OVRBone>();
        _TempBindPoses = new List<OVRBone>();

        for (int i = 0; i < CurrentSkeleton.Bones.Count; i++)
        {
            _TempBones.Add(CloneOVRBone(CurrentSkeleton.Bones[i]));
            _TempBindPoses.Add(CloneOVRBone(CurrentSkeleton.BindPoses[i]));
        }

        GameObject tempRootBone = GenerateRootBoneTransform();


        return new HandForm(_CurrentAlphabetLetter,
                            _TempBones,
                            _TempBindPoses,
                            HandTipData.GetCurrentTipDistances(CurrentSkeleton),
                            tempRootBone);
    }

    private OVRBone CloneOVRBone(OVRBone b)
    {
        return new OVRBone(b.Id,
                           b.ParentBoneIndex,
                           CloneTransform(b.Transform));
    }

    private Transform CloneTransform(Transform t)
    {
        GameObject obj = new GameObject();
        obj.transform.position = t.position;
        obj.transform.rotation = t.rotation;

        obj.transform.localScale = t.localScale;

        return obj.transform;
    }

    private GameObject GenerateRootBoneTransform()
    {
        GameObject bonesGO = new GameObject("Bones");
        bonesGO.transform.localPosition = HandAnchor.position;
        bonesGO.transform.localRotation = HandAnchor.rotation;

        return bonesGO;
    }

    #endregion

    #region SaveHandForm

    private void SaveHandFormToTxt(HandForm handForm)
    {
        _FileName = Application.persistentDataPath + "/" + "handdata.txt";

        FileInfo f = new FileInfo(_FileName);

        // create new textfile
        if (!f.Exists)
        {
            using (StreamWriter sw = f.CreateText())
            {
                WriteHandFormToTxt(sw, handForm);

            }
        }
        // add to existing textfile
        else
        {
            using (StreamWriter sw = f.AppendText())
            {
                WriteHandFormToTxt(sw, handForm);

            }
        }
    }

    private void WriteHandFormToTxt(StreamWriter sw, HandForm handForm)
    {
        sw.Write(handForm.AlphabeticCharacter + "/");
        WriteOVRBoneListToTxt(sw, handForm.SavedBones);
        WriteOVRBoneListToTxt(sw, handForm.SavedBindPoses);
        sw.Write(ConvertTransformToString(handForm.BoneGO.transform) + "/");
        WriteTipDistancesToTxt(sw, handForm.SavedTipDistances);
        sw.WriteLine();

    }

    private void WriteOVRBoneListToTxt(StreamWriter sw, IList<OVRBone> bones)
    {
        foreach (OVRBone bone in bones)
        {
            sw.Write("{");
            sw.Write(((int)bone.Id) + ":");
            sw.Write(bone.ParentBoneIndex + ":");
            sw.Write(ConvertTransformToString(bone.Transform));
            sw.Write("}");
        }
        sw.Write("/");
    }

    private void WriteTipDistancesToTxt(StreamWriter sw,float[] tipDistances)
    {
        sw.Write("(");
        for(int i = 0; i < tipDistances.Length-1; i++)
        {
            sw.Write(tipDistances[i] + ",");
        }

        sw.Write(tipDistances[tipDistances.Length - 1] + ")");
    }

    private string ConvertTransformToString(Transform transform)
    {
        return transform.position.ToString("F8") + transform.eulerAngles.ToString("F8") + transform.localScale.ToString();
    }

    #endregion

    #endregion

}

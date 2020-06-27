using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using Newtonsoft.Json;

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

    public DebugCanvas debug;

    // All possible Alphabet letters
    private static char[] ALPHABET = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'S', 'T', 'U', 'V', 'W', 'X', 'Y' };

    private string _FileName;
    private char _CurrentAlphabetLetter;
    private int _AlphabetId;


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
        debug.Log("SaveHand");
        try
        {
            SerializedHandForm nHandForm = GenerateSerializableFromCurrentHandForm();

            
            string handFormJson = JsonConvert.SerializeObject(nHandForm);

            debug.Log(handFormJson);

            SaveJsonToFile(handFormJson);

        }
        catch(System.Exception e)
        {
            debug.Log(e.ToString());
        }

        GetNextAlphabetLetter();

    }

    private void GetNextAlphabetLetter()
    {
        _AlphabetId++;
        _CurrentAlphabetLetter = ALPHABET[_AlphabetId];
    }

    #region GenerateHandForm
    private SerializedHandForm GenerateSerializableFromCurrentHandForm()
    {
        List<SerializedOVRBone> tempBones = new List<SerializedOVRBone>();
        List<SerializedOVRBone> tempBindPoses = new List<SerializedOVRBone>();

        for (int i = 0; i < CurrentSkeleton.Bones.Count; i++)
        {
            OVRBone bone = CloneOVRBone(CurrentSkeleton.Bones[i]);
            OVRBone bindPose = CloneOVRBone(CurrentSkeleton.BindPoses[i]);
            tempBones.Add(new SerializedOVRBone(bone));
            tempBindPoses.Add(new SerializedOVRBone(bindPose));
        }

        GameObject tempRootBone = GenerateRootBoneTransform();


        return new SerializedHandForm(_CurrentAlphabetLetter,
                                      tempBones,
                                      tempBindPoses,
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

    private void SaveJsonToFile(string json)
    {
        _FileName = Application.persistentDataPath + "/" + "handdata.json";

        FileInfo f = new FileInfo(_FileName);

        // create new textfile
        if (!f.Exists)
        {
            using (StreamWriter sw = f.CreateText())
            {
                sw.Write(json);
            }
        }
        // add to existing textfile
        else
        {
            using (StreamWriter sw = f.AppendText())
            {
                sw.Write(json);
            }
        }
    }


    private void SaveHandFormToJson(HandForm handForm)
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

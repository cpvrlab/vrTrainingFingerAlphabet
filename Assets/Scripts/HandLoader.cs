using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// Loads handdata.txt file and converts them to HandForm struct
/// 
/// Author: Cédric Girardin
/// </summary>
public class HandLoader : MonoBehaviour
{
    #region Properties

    public string FileName = "handdata.txt";

    private List<HandForm> _LoadedHandForms;
    private bool _Loaded = false;
    private string _TestPath = "Assets/TestFiles/handdata.txt";

    #endregion

    #region UnityFunctions
    private void Awake()
    {

        _LoadedHandForms = new List<HandForm>();
        FileName = Application.persistentDataPath + "/" + FileName;
        
        _Loaded = false;
        try
        {
            if (!File.Exists(FileName))
            {
                string tempPath = System.IO.Path.Combine(Application.streamingAssetsPath, "handdata.txt");

                // Android only use WWW to read file
                WWW reader = new WWW(tempPath);
                while (!reader.isDone) { }

                System.IO.File.WriteAllBytes(FileName, reader.bytes);
            }
            LoadAllSavedHandForms(FileName);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }

    }

    #endregion

    #region HandLoaderFunctions
    /// <summary>
    /// Gets the loaded HandForm object relative to the alphabetChar
    /// </summary>
    /// <param name="alphabetChar">The Alphabetic character corresponding to the HandForm</param>
    /// <returns>HandForm object</returns>
    public HandForm GetHandForm(char alphabetChar)
    {
        foreach(HandForm hand in _LoadedHandForms)
        {
            if(hand.AlphabeticCharacter == alphabetChar)
            {
                return hand;
            }
        }
        return new HandForm();
    }

    /// <summary>
    /// Checks if all Handforms form handata.txt are loaded and ready
    /// </summary>
    /// <returns>true when loaded, false when still loading</returns>
    public bool IsDataLoaded()
    {
        return _Loaded;
    }


    private void LoadAllSavedHandForms(string fileName)
    {
        using (StreamReader r = new StreamReader(fileName))
        {
            // reads handata.txt 
            string input = r.ReadToEnd();

            // splits input for each new line
            string[] handFormsString = input.Split(
                        new[] { "\n" },
                        StringSplitOptions.None
                        );

            // convert each line from handata.txt to HandForm
            foreach(String s in handFormsString)
            {
                if (s.Length > 1)
                {
                    _LoadedHandForms.Add(StringToHandForm(s));
                }

            }

        }
        _Loaded = true;

    }

    #region StringConvertFunctions

    private HandForm StringToHandForm(string handFormStirng)
    {
        string[] properties = handFormStirng.Split('/');
        char alphabetChar           = properties[0].ToCharArray()[0];
        List<OVRBone> bones         = StringToBoneList(properties[1]);
        List<OVRBone> bindPoses     = StringToBoneList(properties[2]);
        Transform transformGO       = StringToTransfrom(properties[3]);
        float[] tipDistances        = StringToFloatArray(properties[4]);

        transformGO.gameObject.name = alphabetChar + "-BoneGO";
        bones[0].Transform.parent = transformGO;
        bindPoses[0].Transform.parent = transformGO;

        // set parents of the bones
        for (int i = 1; i < bones.Count; i++)
        {
            bones[i].Transform.parent = GetParentBone(i, bones);
            bindPoses[i].Transform.parent = GetParentBindPoses(i, bindPoses);
        }

        return new HandForm(alphabetChar, bones, bindPoses, tipDistances, transformGO.gameObject);
    }

    private List<OVRBone> StringToBoneList(string boneListString)
    {
        List<OVRBone> tempBoneList = new List<OVRBone>();

        string pattern = @"\{(.*?)\}";
        MatchCollection matches = Regex.Matches(boneListString, pattern);
        foreach(Match m in matches)
        {
            tempBoneList.Add(StringToOVRBone(m.Value));
        }
        return tempBoneList;
    }

    private OVRBone StringToOVRBone(string boneString)
    {
        string boneStringTrimed = boneString.Trim(new char[] { '{', '}' });
        string[] properties = boneStringTrimed.Split(':');

        OVRSkeleton.BoneId boneId   = (OVRSkeleton.BoneId)Convert.ToInt32(properties[0]);
        short parentId              = (short)Convert.ToInt32(properties[1]);
        Transform transform         = StringToTransfrom(properties[2]);

        transform.gameObject.name = boneId.ToString();
        return new OVRBone(boneId,parentId,transform);
    }

    private Transform StringToTransfrom(string transformString)
    {
        string pattern = @"\((.*?)\)";
        MatchCollection matches = Regex.Matches(transformString, pattern);
        
        GameObject obj = new GameObject();
        obj.transform.position      = StringToVector3(matches[0].Value);
        obj.transform.eulerAngles   = StringToVector3(matches[1].Value);
        obj.transform.localScale    = StringToVector3(matches[2].Value);

        return obj.transform;
    }

    private Vector3 StringToVector3(string vectorString)
    {
        string vectorStringTrimed = vectorString.Trim(new char[] { '(', ')' });
        string[] floatValues = vectorStringTrimed.Split(',');
        return new Vector3(
                    float.Parse(floatValues[0]),
                    float.Parse(floatValues[1]),
                    float.Parse(floatValues[2])
                    );
    }

    private float[] StringToFloatArray(string floatArrayString)
    {
        string floatArrayTrimed = floatArrayString.Trim(new char[] { '(', ')' });
        string[] floatValues = floatArrayTrimed.Split(',');

        float[] floatArray = new float[floatValues.Length];
        for(int i = 0; i < floatArray.Length; i++)
        {
            floatArray[i] = float.Parse(floatValues[i]);
        }

        return floatArray;
    }

    #endregion

    #region FindParentFunctions

    private Transform GetParentBone(int index, List<OVRBone> TempBones)
    {
        return TempBones[TempBones[index].ParentBoneIndex].Transform;
    }
    /// <summary>
    /// Get the parent transform of the bindposes List _SavedBindPoses
    /// </summary>
    /// <param name="index">index of the _SavedBindPoses List</param>
    /// <returns>Transform of the parent</returns>
    private Transform GetParentBindPoses(int index, List<OVRBone> TempBindPoses)
    {
        return TempBindPoses[TempBindPoses[index].ParentBoneIndex].Transform;
    }

    #endregion

    #endregion

}

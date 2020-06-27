using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a Serializable OVRBone to Save in a Json
/// 
/// Author: Cédric Girardin
/// </summary>
[System.Serializable]
public struct SerializedOVRBone
{
    // NOTE: In the Json the Id will be a int!
    public OVRSkeleton.BoneId Id { get; set; }
    public short ParentBoneIndex { get; set; }
    public SerializedTransform Transform { get; set; }

    public SerializedOVRBone(OVRSkeleton.BoneId id, short parentBoneIndex, SerializedTransform trans)
    {
        Id = id;
        ParentBoneIndex = parentBoneIndex;
        Transform = trans;
    }

    public SerializedOVRBone(OVRSkeleton.BoneId id, short parentBoneIndex, Transform trans)
    {
        Id = id;
        ParentBoneIndex = parentBoneIndex;
        Transform = new SerializedTransform(trans);
    }

    public SerializedOVRBone(OVRBone bone)
    {
        Id = bone.Id;
        ParentBoneIndex = bone.ParentBoneIndex;
        Transform = new SerializedTransform(bone.Transform);
    }
}

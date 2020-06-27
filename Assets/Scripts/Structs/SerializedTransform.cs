using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// a Serializable Transform to Save in a Json
/// 
/// Author: arielfel
/// Source: https://forum.unity.com/threads/how-to-save-a-transform.495981/
/// </summary>
[System.Serializable]
public struct SerializedTransform
{
    public float[] _position;
    public float[] _rotation;
    public float[] _scale;
    
    public SerializedTransform(float[] pos, float[] rot, float[] scl)
    {
        _position = pos;
        _rotation = rot;
        _scale   = scl;
    }

    public SerializedTransform(Transform transform, bool worldSpace = false)
    {
        _position = new float[3];
        _position[0] = transform.localPosition.x;
        _position[1] = transform.localPosition.y;
        _position[2] = transform.localPosition.z;

        _rotation = new float[4];
        _rotation[0] = transform.localRotation.w;
        _rotation[1] = transform.localRotation.x;
        _rotation[2] = transform.localRotation.y;
        _rotation[3] = transform.localRotation.z;

        _scale = new float[3];
        _scale[0] = transform.localScale.x;
        _scale[1] = transform.localScale.y;
        _scale[2] = transform.localScale.z;

    }

    public override string ToString()
    {
        return (
                    _position.ToString() + "\n" +
                    _rotation.ToString() + "\n" +
                    _scale.ToString() + "\n"

                );
    }
}

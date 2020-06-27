using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Convert SerializedTransform to Transform
/// 
/// Author: arielfel
/// Source: https://forum.unity.com/threads/how-to-save-a-transform.495981/
/// </summary>
public static class SerializedTransformExtention
{
    public static void DeserialTransform(SerializedTransform _serializedTransform, Transform _transform)
    {
        _transform.localPosition = new Vector3(_serializedTransform._position[0], _serializedTransform._position[1], _serializedTransform._position[2]);
        _transform.localRotation = new Quaternion(_serializedTransform._rotation[1], _serializedTransform._rotation[2], _serializedTransform._rotation[3], _serializedTransform._rotation[0]);
        _transform.localScale = new Vector3(_serializedTransform._scale[0], _serializedTransform._scale[1], _serializedTransform._scale[2]);
    }
}

public static class TransformExtention
{
    public static void SetTransformEX(this Transform original, Transform copy)
    {
        original.position = copy.position;
        original.rotation = copy.rotation;
        original.localScale = copy.localScale;
    }
}

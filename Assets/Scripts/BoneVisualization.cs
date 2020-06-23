using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visualization of a Bone with a Linerenderer
/// 
/// Author: Cédric Girardin and Facebook Technologies
/// </summary>
public class BoneVisualization
{

    #region Properties
    private const float LINE_RENDERER_WIDTH = 0.005f;
    public GameObject BoneGO;
    public Transform BoneBegin;
    public Transform BoneEnd;
    public LineRenderer Line;

    #endregion


    #region BoneVisualizationFunctions
    /// <summary>
    /// Constructor of the BoneVisualization
    /// </summary>
    /// <param name="rootGO">Gameobject for the Linerenderer</param>
    /// <param name="mat">Material for the Bone</param>
    /// <param name="scale">Scale of the Bonewidth</param>
    /// <param name="begin">Start of the Bone position</param>
    /// <param name="end">End of the Bone position</param>
    public BoneVisualization(GameObject rootGO, Material mat, float scale, Transform begin, Transform end)
    {
        BoneBegin = begin;
        BoneEnd = end;

        BoneGO = new GameObject(begin.name);
        BoneGO.transform.SetParent(rootGO.transform, false);

        Line = BoneGO.AddComponent<LineRenderer>();
        Line.sharedMaterial = mat;
        Line.useWorldSpace = true;
        Line.positionCount = 2;

        Line.SetPosition(0, BoneBegin.position);
        Line.SetPosition(1, BoneEnd.position);

        Line.startWidth = LINE_RENDERER_WIDTH * scale;
        Line.endWidth = LINE_RENDERER_WIDTH * scale;
    }

    /// <summary>
    /// Updates the Linerenderer
    /// </summary>
    /// <param name="scale">Scale of the Bonewidth</param>
    /// <param name="shouldRender">Should the bone be visible</param>
    public void Update(float scale, bool shouldRender)
    {
        Line.enabled = shouldRender;

        Line.SetPosition(0, BoneBegin.position);
        Line.SetPosition(1, BoneEnd.position);

        Line.startWidth = LINE_RENDERER_WIDTH * scale;
        Line.endWidth = LINE_RENDERER_WIDTH * scale;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays HandForm struct as a OVRMesh in the Scene
/// Requires the Components OVRMesh and SkinnedMeshRenderer
/// 
/// Author: Cédric Girardin
/// </summary>
[RequireComponent(typeof(OVRMesh), typeof(SkinnedMeshRenderer))]
public class HandFormRenderer : MonoBehaviour
{

    #region Properties

    public Material ActiveMeshMaterial;
    public Material ActiveMaterial;
    public Material WrongMaterial;
    public Vector3 OffsetToHead = new Vector3(-0.15f,-0.35f,0.4f);

    // default width for the Linerenderer
    private const float LINE_RENDERER_WIDTH = 0.005f;

    private List<BoneVisualization> _BoneVisualizations;
    private BoneVisualization _WristOrientationVisualization;
    private List<BoneVisualization> _TipVisualization;
    private GameObject _SkeletonGO;
    private float _Scale = 1f;
    private SkinnedMeshRenderer _SkinnedMeshRenderer;
    private OVRMesh[] _OVRMeshs;
    private Material _SkeletonMaterial;
    private Material _DefaultMaterialMeshRenderer;
    private bool _Initalized = false;

    #endregion

    #region UnityFunctions

    void Start()
    {
        _OVRMeshs = GetComponents<OVRMesh>();
        _SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        _DefaultMaterialMeshRenderer = _SkinnedMeshRenderer.material;
        if ((int)HandednessManager.ActiveHandedness == 1)
            OffsetToHead.x = -(OffsetToHead.x);
    }

    #endregion

    #region DisplayFunctions

    /// <summary>
    /// Renders savedHand as Bonevisualizations objects and with a Skinnedmeshrenderer
    /// </summary>
    /// <param name="savedHand">The Handdata that should be Renderd</param>
    public void DisplayHandForm(HandForm savedHand)
    {
        _Initalized = false;
        // Destroy previous Virtualhand 
        Destroy(_SkeletonGO);

        // Skeleton
        ShowSkeletonBones(savedHand);



        // Orientation
        ShowHandOrientation(savedHand);

        // Tip Distances
        ShowTipDistances(savedHand);

        _Initalized = true;

        if (_OVRMeshs[(int)HandednessManager.ActiveHandedness] == null)
            return;
        // Mesh
        ShowSkinnedMeshRenderer(savedHand);


    }


    private void ShowTipDistances(HandForm savedHand)
    {
        _TipVisualization = InitalizeTipRenderer(savedHand.SavedBones, savedHand.SavedTipDistances.Length);
        foreach(BoneVisualization tipVisaul in _TipVisualization)
        {
            tipVisaul.Update(_Scale, true);
        }

    }

    private void ShowHandOrientation(HandForm savedHand)
    {
        Transform wristRoot = savedHand.SavedBones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform;
        GameObject orientation = new GameObject(savedHand.AlphabeticCharacter + "-OrientationBone");
        orientation.transform.parent = wristRoot;
        orientation.transform.localPosition = wristRoot.up * 0.25f;

        _WristOrientationVisualization = new BoneVisualization(_SkeletonGO,
                                                                  _SkeletonMaterial,
                                                                  _Scale,
                                                                  wristRoot,
                                                                  orientation.transform);
        _WristOrientationVisualization.Update(_Scale, true);

    }

    private void ShowSkeletonBones(HandForm savedHand)
    {

        _BoneVisualizations = InitalizeBoneRenderer(savedHand.SavedBones);
        foreach(BoneVisualization boneVisual in _BoneVisualizations)
        {
            boneVisual.Update(_Scale, true);
        }
    }

    private void ShowSkinnedMeshRenderer(HandForm savedHand)
    {
        InitializeMeshRenderer(savedHand);
        _SkinnedMeshRenderer.enabled = true;
    }
    
    #endregion

    #region InitalizationFunctions
    /// <summary>
    /// Initalizeses a List of BoneVisualizations
    /// </summary>
    /// <param name="bones">the bones that will be visualized</param>
    /// <returns>List of the class BoneVisualization</returns>
    private List<BoneVisualization> InitalizeBoneRenderer(IList<OVRBone> bones)
    {

        List<BoneVisualization> tempBoneVisualization = new List<BoneVisualization>();
        _SkeletonGO = new GameObject("SkeletonRenderer");
        _SkeletonGO.transform.SetParent(transform, false);

        _SkeletonMaterial = ActiveMaterial;

        for (int i = 0; i < bones.Count; i++)
        {
            var boneVis = new BoneVisualization(
                    _SkeletonGO,
                    _SkeletonMaterial,
                    _Scale,
                    bones[i].Transform,
                    bones[i].Transform.parent);

            tempBoneVisualization.Add(boneVis);
        }

        return tempBoneVisualization;


    }

    private List<BoneVisualization> InitalizeTipRenderer(IList<OVRBone> bones, int tipLength)
    {
        List<BoneVisualization> tempTipVisual = new List<BoneVisualization>();
        int startTipId = (int)OVRSkeleton.BoneId.Hand_MaxSkinnable;

        for (int i = 1; i < tipLength + 1; i++)
        {
            BoneVisualization tipVisual = null;
            if (i != tipLength)
            {
                tipVisual = new BoneVisualization(_SkeletonGO,
                                                  _SkeletonMaterial,
                                                  _Scale,
                                                  bones[startTipId + i - 1].Transform,
                                                  bones[startTipId + i].Transform);
            }
            else
            {
                tipVisual = new BoneVisualization(_SkeletonGO,
                                                  _SkeletonMaterial,
                                                  _Scale,
                                                  bones[startTipId + i - 1].Transform,
                                                  bones[startTipId].Transform);
            }




            tempTipVisual.Add(tipVisual);
        }

        return tempTipVisual;
    }



    /// <summary>
    /// Initalizes the skinned meshrenderer
    /// </summary>
    /// <param name="savedBones">bones that will be set to the skinned meshrenderer</param>
    /// <param name="savedBindPoses">bindposes that will be set to the skinned meshrenderer</param>
    private void InitializeMeshRenderer(HandForm savedHand)
    {
        int handId = (int)HandednessManager.ActiveHandedness;
        _SkinnedMeshRenderer.sharedMesh = _OVRMeshs[handId].Mesh;

        int numSkinnableBones = (int)OVRSkeleton.BoneId.Hand_MaxSkinnable;
        var bindPoses = new Matrix4x4[numSkinnableBones];
        var bones = new Transform[numSkinnableBones];
        var localToWorldMatrix = savedHand.BoneGO.transform.localToWorldMatrix;
        for (int i = 0; i < numSkinnableBones && i < savedHand.SavedBones.Count; ++i)
        {
            bones[i] = savedHand.SavedBones[i].Transform;
            bindPoses[i] = savedHand.SavedBindPoses[i].Transform.worldToLocalMatrix * localToWorldMatrix;
        }
        _OVRMeshs[handId].Mesh.bindposes = bindPoses;
        _SkinnedMeshRenderer.bones = bones;
        _SkinnedMeshRenderer.updateWhenOffscreen = true;
    }

    #endregion

    #region VisualIndicators


    public void UpdateVirtualHandPosition(Transform handRoot)
    {
        Debug.Log("UpdateVirtualHandPosition");
        if (!_Initalized)
            return;

        handRoot.transform.position = Camera.main.transform.position + OffsetToHead;

        foreach (BoneVisualization tipVisaul in _TipVisualization)
        {
            tipVisaul.Update(_Scale, true);
        }
        foreach (BoneVisualization boneVisual in _BoneVisualizations)
        {
            boneVisual.Update(_Scale, true);
        }
        _WristOrientationVisualization.Update(_Scale, true);
    }

    /// <summary>
    /// Changes the material of the skinned meshrenderer
    /// </summary>
    /// <param name="on">true for active material, false for default material</param>
    public void SetMeshMaterialAcitve(bool on)
    {
        _SkinnedMeshRenderer.sharedMaterial = GetRightMaterial(on);
    }

    /// <summary>
    /// Changes the Bone Material
    /// </summary>
    /// <param name="index">bone index of the OVRBone List</param>
    /// <param name="on">true for active material, false for default material</param>
    public void SetBoneActive(int index, bool on)
    {
        SetLineMaterial(_BoneVisualizations[index], on);
    }

    /// <summary>
    /// Changes the Wristbone Material
    /// </summary>
    /// <param name="on">true for active material, false for default material</param>
    public void SetWristOrientationActive(bool on)
    {
        _WristOrientationVisualization.Line.sharedMaterial = GetActiveMaterial(on);
        _WristOrientationVisualization.Update(_Scale, true);
    }

    /// <summary>
    /// Chnages the TipDistance Material
    /// </summary>
    /// <param name="index">tip index of the SavedTipDistances</param>
    /// <param name="on"></param>
    public void SetTipActive(int index, bool on)
    {
        SetLineMaterial(_TipVisualization[index], on);
    }

    /// <summary>
    /// Changes material of the BoneVisualiztation
    /// </summary>
    /// <param name="boneV">BoneVisualization to change Material</param>
    /// <param name="on">true for active material, false for default material</param>
    private void SetLineMaterial(BoneVisualization boneV, bool on)
    {
        boneV.Line.sharedMaterial = GetActiveMaterial(on);
        boneV.Update(_Scale, true);
    }

    /// <summary>
    /// Gives the active Material or the default Material for the Bones depending on the "on" parameter
    /// </summary>
    /// <param name="on">true for active material, false for default material</param>
    /// <returns>Return Material depending on the "on" parameter</returns>
    private Material GetActiveMaterial(bool on)
    {
        if (on)
            return ActiveMaterial;
        else
            return WrongMaterial;
    }

    /// <summary>
    /// Gives the active Material or the default Material for the MeshRenderer depending on the "on" parameter
    /// </summary>
    /// <param name="on">true for active material, false for default material</param>
    /// <returns>Return Material depending on the "on" parameter</returns>
    private Material GetRightMaterial(bool on)
    {
        if (on)
            return ActiveMeshMaterial;
        else
            return _DefaultMaterialMeshRenderer;
    }
    #endregion

}

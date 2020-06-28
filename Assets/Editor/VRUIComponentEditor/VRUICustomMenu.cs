/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;
using UnityEditor;

/// <summary>
/// This script adds options to the context menu of the hierarchy view for creating VRUIElements.
/// </summary>
public class VRUICustomMenu : MonoBehaviour
{
    [MenuItem("GameObject/VRUI Component/VRUIPanel", false, 10)]
    private static void CreateVRUIPanel()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/VRUI/VRUIPanel.prefab", typeof(GameObject)) as GameObject;
        GameObject instance = Instantiate(prefab);
        instance.name = "VRUIPanel";
        if (Selection.activeGameObject)
        {
            instance.transform.SetParent(Selection.activeGameObject.transform);
        }
        Undo.RegisterCreatedObjectUndo(instance, "Create VRUIPanel");
    }

    [MenuItem("GameObject/VRUI Component/VRUIScrollPanel", false, 10)]
    private static void CreateVRUIScrollPanel()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/VRUI/VRUIScrollPanel.prefab", typeof(GameObject)) as GameObject;
        GameObject instance = Instantiate(prefab);
        instance.name = "VRUIScrollPanel";
        if (Selection.activeGameObject)
        {
            instance.transform.SetParent(Selection.activeGameObject.transform);
        }
        Undo.RegisterCreatedObjectUndo(instance, "Create VRUIScrollPanel");
    }

    [MenuItem("GameObject/VRUI Component/VRUIButton", false, 10)]
    private static void CreateVRUIButton()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/VRUI/VRUIButton.prefab", typeof(GameObject)) as GameObject;
        GameObject instance = Instantiate(prefab);
        instance.name = "VRUIButton";
        if (Selection.activeGameObject)
        {
            instance.transform.SetParent(Selection.activeGameObject.transform);
        }
        Undo.RegisterCreatedObjectUndo(instance, "Create VRUIButton");
    }

    [MenuItem("GameObject/VRUI Component/VRUIToggle", false, 10)]
    private static void CreateVRUIToggle()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/VRUI/VRUIToggle.prefab", typeof(GameObject)) as GameObject;
        GameObject instance = Instantiate(prefab);
        instance.name = "VRUIToggle";
        if (Selection.activeGameObject)
        {
            instance.transform.SetParent(Selection.activeGameObject.transform);
        }
        Undo.RegisterCreatedObjectUndo(instance, "Create VRUIToggle");
    }

    [MenuItem("GameObject/VRUI Component/VRUISlider", false, 10)]
    private static void CreateVRUISlider()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/VRUI/VRUISlider.prefab", typeof(GameObject)) as GameObject;
        GameObject instance = Instantiate(prefab);
        instance.name = "VRUISlider";
        if (Selection.activeGameObject)
        {
            instance.transform.SetParent(Selection.activeGameObject.transform);
        }
        Undo.RegisterCreatedObjectUndo(instance, "Create VRUISlider");
    }

    [MenuItem("GameObject/VRUI Component/VRUITextcontainer", false, 10)]
    private static void CreateVRUITextcontainer()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/VRUI/VRUITextcontainer.prefab", typeof(GameObject)) as GameObject;
        GameObject instance = Instantiate(prefab);
        instance.name = "VRUITextcontainer";
        if (Selection.activeGameObject)
        {
            instance.transform.SetParent(Selection.activeGameObject.transform);
        }
        Undo.RegisterCreatedObjectUndo(instance, "Create VRUITextcontainer");
    }
}

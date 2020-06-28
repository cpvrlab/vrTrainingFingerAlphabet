/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;

/// <summary>
/// Simulates a toggle group. If one of the toggles in the list was pressed all other toggles get unstuck.
/// </summary>
public class VRUIToggleGroupHelper : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Add the Toggles that belong to the same toggle group here.")]
    private VRUIToggleBehaviour[] toggleList;

    // Start is called before the first frame update
    void Start()
    {
        foreach (VRUIToggleBehaviour toggle in toggleList)
        {
            toggle.ToggleIsStuck = false;
        }
    }

    public void ToggleInGroupWasPressed(string name)
    {
        foreach (VRUIToggleBehaviour toggle in toggleList)
        {
            if (toggle.ToggleIsStuck && toggle.name != name)
            {
                toggle.ToggleIsStuck = false;
            }
        }
    }

    public VRUIToggleBehaviour[] GetToggleList()
    {
        return toggleList;
    }

    public void SetToggleList(VRUIToggleBehaviour[] list)
    {
        toggleList = list;
    }

    public void AddElementAtPosition(VRUIToggleBehaviour toggle, int i)
    {
        toggleList[i] = toggle;
    }
}

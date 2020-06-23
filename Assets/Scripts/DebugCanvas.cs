using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Log messages for debuggin within the oculus Quest
/// 
/// Author: Cédric Girardin
/// </summary>
public class DebugCanvas : MonoBehaviour
{
    #region Properties
    public Text Ouput;

    #endregion

    #region DebugFunctions
    /// <summary>
    /// Log a message to the DebugCanvas
    /// </summary>
    /// <param name="Message">The Message that will be displayd on the Canvas</param>
    public void Log(string Message)
    {
        Ouput.text += Message + "\n";
    }

    /// <summary>
    /// Clears all log messages on the DebugCanvas
    /// </summary>
    public void Clear()
    {
        Ouput.text = "";
    }

    #endregion

}

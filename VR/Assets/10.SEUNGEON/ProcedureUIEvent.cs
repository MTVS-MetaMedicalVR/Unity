using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ProcedureSystem/Data/ProcedureUIEvent.cs
[System.Serializable]
public class ProcedureUIEvent
{
    public bool showUI;
    public string position;  // "front", "side"
    public string buttonText;
    public string type;  // "instruction", "warning", "critical"
    public bool blockProgress;
}
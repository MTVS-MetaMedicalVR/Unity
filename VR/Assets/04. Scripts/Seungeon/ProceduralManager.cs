using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class ProceduralData
{
    public string id;
    public string name;
    public List<ProceduralStep> steps;
}

[System.Serializable]
public class ProceduralStep
{
    public string id;
    public string Description;
    public string action;

}

public class ProceduralManager : MonoBehaviour
{
    public static ProceduralManager Instance;

    private List<ProceduralObject> referenceObjectList;




}

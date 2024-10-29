using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TempProcedureNormalObject : TempProcedureObjectBase
{
    
    public void OnTriggerEnter(Collider other)
    {
        //if (!other.transform.tag.Contains("PlayerHand")) return;
        
        isDone = true;
        this.gameObject.SetActive(false);   
    }
}

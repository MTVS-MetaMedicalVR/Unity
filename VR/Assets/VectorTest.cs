using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorTest : MonoBehaviour
{
    public Transform chairBack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("chairBack : " + chairBack.forward);
        transform.rotation = Quaternion.LookRotation(chairBack.up, -chairBack.forward);
    }
}

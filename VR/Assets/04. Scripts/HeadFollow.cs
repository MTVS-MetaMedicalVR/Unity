using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeadFollow : MonoBehaviour
{
    public Transform neckTransform;
    public float nowRotation = 0;
    public bool isLeft = false;
    public bool isRight = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = neckTransform.position;

        if (isLeft)
        {
            nowRotation += Time.deltaTime * -45f;
        }
        else if (isRight)
        {
            nowRotation += Time.deltaTime * 45f;
        }
        else
        {
            nowRotation = Mathf.Lerp(nowRotation, 0, Time.deltaTime * 10f);
        }

        nowRotation = Mathf.Clamp(nowRotation, -35f, 35f);

        Vector3 rotateDir = Quaternion.AngleAxis(nowRotation, neckTransform.up) * neckTransform.forward;
        transform.rotation = Quaternion.LookRotation(rotateDir , neckTransform.up );
    }
}

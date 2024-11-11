using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JawFollow : MonoBehaviour
{
    public Transform jawRoot;
    public float nowRotation = 0;
    public bool isOpen = false;

    UnitChairController unitChairController;

    // Start is called before the first frame update
    void Start()
    {
        unitChairController = GameObject.Find("Button_Back").GetComponent<UnitChairController>();
    }

    // Update is called once per frame
    void Update()
    {
        //JawRoot 위치 따라 이동하자. 
        transform.position = jawRoot.transform.position;

        if (isOpen)
        {
            nowRotation += Time.deltaTime * -30f;


        }
        else
        {
            nowRotation += Time.deltaTime * 30f;

        }

        nowRotation = Mathf.Clamp(nowRotation, -30f, 0f);

        Vector3 openDir = (Quaternion.AngleAxis(nowRotation, jawRoot.parent.forward) * jawRoot.parent.right);



        transform.rotation = Quaternion.LookRotation(-Vector3.right, openDir);

        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y, nowRotation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JawFollow : MonoBehaviour
{
    public Transform jawRoot;
    public float nowRotation = 0;
    public bool isOpen = false;

    UnitChairController unitChairController;
    public GameObject closeMouse;
    public GameObject closeTongue;
    public GameObject openMouse;

    // Start is called before the first frame update
    void Start()
    {
        unitChairController = GameObject.Find("Button_Back").GetComponentInParent<UnitChairController>();

        //Open용 입 오브젝트 false
        openMouse.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //// UnitChairController의 isBack 상태에 따라 isOpen 설정
        //if (unitChairController.isBack == true)
        //{
        //    isOpen = true;
        //}
        //else
        //{
        //    isOpen = false;
        //}

        //JawRoot 위치 따라 이동하자. 
        transform.position = jawRoot.transform.position;

        if (isOpen)
        {
            nowRotation += Time.deltaTime * -30f;

            openMouse.SetActive(true);

            closeMouse.SetActive(false);
            closeTongue.SetActive(false);
        }
        else
        {
            nowRotation += Time.deltaTime * 30f;

            openMouse.SetActive(false);

            closeMouse.SetActive(true);
            closeTongue.SetActive(true);
        }

        nowRotation = Mathf.Clamp(nowRotation, -30f, 0f);

        Vector3 openDir = (Quaternion.AngleAxis(nowRotation, jawRoot.parent.forward) * jawRoot.parent.right);



        transform.rotation = Quaternion.LookRotation(-Vector3.right, openDir);

        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y, nowRotation);
    }
}

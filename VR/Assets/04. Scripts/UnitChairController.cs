using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitChairController : UnitChairAnimator
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 키 입력 감지 및 트리거 호출
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 숫자 1 키
        {
            TableOn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 숫자 2 키
        {
            TableOff();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 숫자 3 키
        {
            BackOn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // 숫자 4 키
        {
            BackOff();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) // 숫자 5 키
        {
            BaseOn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) // 숫자 6 키
        {
            BaseOff();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) // 숫자 7 키
        {
            LightOn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) // 숫자 8 키
        {
            LightOff();
        }
    }
}

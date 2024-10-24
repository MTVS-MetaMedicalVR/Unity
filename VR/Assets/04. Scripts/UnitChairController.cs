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
        // Ű �Է� ���� �� Ʈ���� ȣ��
        if (Input.GetKeyDown(KeyCode.Alpha1)) // ���� 1 Ű
        {
            TableOn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // ���� 2 Ű
        {
            TableOff();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // ���� 3 Ű
        {
            BackOn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // ���� 4 Ű
        {
            BackOff();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) // ���� 5 Ű
        {
            BaseOn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) // ���� 6 Ű
        {
            BaseOff();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) // ���� 7 Ű
        {
            LightOn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) // ���� 8 Ű
        {
            LightOff();
        }
    }
}

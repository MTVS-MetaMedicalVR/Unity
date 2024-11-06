using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientController : MonoBehaviour
{
    UnitChairController unitChairController;
    public Transform patient;
    Quaternion originalRotate;

    // Start is called before the first frame update
    void Start()
    {
        unitChairController = GetComponent<UnitChairController>();
        originalRotate = patient.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (unitChairController.isBack == true)
        {
            // ���� y, z ȸ���� �����ϸ鼭 x ȸ���� ����
            patient.transform.rotation = Quaternion.Euler(61f, patient.eulerAngles.y, patient.eulerAngles.z);
        }
        else
        {
            patient.transform.rotation = originalRotate;
        }
    }
}

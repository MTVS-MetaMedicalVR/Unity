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
            // 기존 y, z 회전을 유지하면서 x 회전만 수정
            patient.transform.rotation = Quaternion.Euler(61f, patient.eulerAngles.y, patient.eulerAngles.z);
        }
        else
        {
            patient.transform.rotation = originalRotate;
        }
    }
}

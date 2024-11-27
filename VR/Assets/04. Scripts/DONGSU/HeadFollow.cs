using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollow : MonoBehaviour
{
    public Transform neckTransform; // �Ӹ��� ���� ������ �Ǵ� Transform
    public Transform fabric; // õ�� Transform

    public float nowRotation = 0; // �Ӹ��� ���� ȸ����
    public float fabricRotation = 0; // õ�� ���� ȸ����

    public bool isLeft = false; // �Ӹ��� �������� ������ ����
    public bool isRight = false; // �Ӹ��� ���������� ������ ����

    public float headTurnSpeed = 45f; // �Ӹ� ȸ�� �ӵ�
    public float fabricTurnSpeed = 3f; // õ ȸ�� �ӵ�

    // Update is called once per frame
    void LateUpdate()
    {
        // 1. �Ӹ��� ��ġ�� ���� ��ġ�� ����ȭ
        transform.position = neckTransform.position;

        // 2. �Ӹ� ȸ���� ���
        if (isLeft)
        {
            nowRotation += Time.deltaTime * -headTurnSpeed; // �������� ȸ��
            fabricRotation = Mathf.Lerp(fabricRotation, 15f, Time.deltaTime * fabricTurnSpeed); // õ�� �ε巴�� �������� ȸ��
        }
        else if (isRight)
        {
            nowRotation += Time.deltaTime * headTurnSpeed; // ���������� ȸ��
            fabricRotation = Mathf.Lerp(fabricRotation, -15f, Time.deltaTime * fabricTurnSpeed); // õ�� �ε巴�� ���������� ȸ��
        }
        else
        {
            nowRotation = Mathf.Lerp(nowRotation, 0, Time.deltaTime * 10f); // �Ӹ��� �������� ����
            fabricRotation = Mathf.Lerp(fabricRotation, 0, Time.deltaTime * fabricTurnSpeed); // õ�� �������� ����
        }

        // �Ӹ� ȸ������ ���� (-35�� ~ 35��)
        nowRotation = Mathf.Clamp(nowRotation, -35f, 35f);

        // 3. �Ӹ� ȸ�� ����
        Vector3 rotateDir = Quaternion.AngleAxis(nowRotation, neckTransform.up) * neckTransform.forward;
        transform.rotation = Quaternion.LookRotation(rotateDir, neckTransform.up);

        // 4. õ�� ȸ�� ����
        fabric.localRotation = Quaternion.Euler(0, 0, fabricRotation);
    }
}

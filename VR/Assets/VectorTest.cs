using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorTest : MonoBehaviour
{
    public Transform chairBack; // ġ�� ������ ����� Transform
    public Transform waistBone; // ĳ������ Waist �� Transform (ȸ�� ����)
    public Transform characterRoot; // ĳ���� ��ü �ֻ��� Transform (��ġ ����)
    public UnitChairController unitChairController; // ������ ���¸� �����ϴ� ��Ʈ�ѷ�
    public Animator riggedAnimator;

    public Vector3 rootPositionOffsetWhenBack = new Vector3(0, -0.3f, 0.1f); // ����̰� �ڷ� ���� �� ��ġ ���� ��
    public Vector3 rootPositionOffsetWhenUpright = new Vector3(0, -0.315f, 0.275f); // ����̰� �⺻ ��ġ�� ���� �� ��ġ ���� ��

    public Vector3 waistRotationOffsetWhenBack = new Vector3(-95.95f, 0, 0); // ����̰� �ڷ� ���� �� ȸ�� ���� ��
    public Vector3 waistRotationOffsetWhenUpright = new Vector3(-88.25f, 0, 10f); // ����̰� �⺻ ��ġ�� ���� �� ȸ�� ���� ��

    void Update()
    {
        if (unitChairController.isBack)
        {
            // ����̰� �ڷ� ������ ������ ��
            UpdatePositionAndRotation(rootPositionOffsetWhenBack, waistRotationOffsetWhenBack);
            riggedAnimator.SetBool("RiggedBack", true);
        }
        else
        {
            // ����̰� �⺻ ������ ��
            UpdatePositionAndRotation(rootPositionOffsetWhenUpright, waistRotationOffsetWhenUpright);
            riggedAnimator.SetBool("RiggedBack", false);
        }
    }

    private void UpdatePositionAndRotation(Vector3 positionOffset, Vector3 rotationOffset)
    {
        // 1. ĳ���� �ֻ��� Transform ��ġ�� ����
        Vector3 targetRootPosition = chairBack.position + positionOffset;
        characterRoot.position = targetRootPosition;

        // 2. Waist �� ȸ���� ����
        Quaternion targetWaistRotation = chairBack.rotation * Quaternion.Euler(rotationOffset);
        waistBone.rotation = targetWaistRotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorTest : MonoBehaviour
{
    public Transform chairBack; // 치과 의자의 등받이 Transform
    public Transform waistBone; // 캐릭터의 Waist 본 Transform (회전 전용)
    public Transform characterRoot; // 캐릭터 전체 최상위 Transform (위치 전용)
    public UnitChairController unitChairController; // 의자의 상태를 제어하는 컨트롤러
    public Animator riggedAnimator;

    public Vector3 rootPositionOffsetWhenBack = new Vector3(0, -0.25f, 0.125f); // 등받이가 뒤로 갔을 때 위치 보정 값
    public Vector3 rootPositionOffsetWhenUpright = new Vector3(0, 0.1f, 0.35f); // 등받이가 기본 위치에 있을 때 위치 보정 값

    public Vector3 waistRotationOffsetWhenBack = new Vector3(0, 0, 0); // 등받이가 뒤로 갔을 때 회전 보정 값
    public Vector3 waistRotationOffsetWhenUpright = new Vector3(0, 0, 10f); // 등받이가 기본 위치에 있을 때 회전 보정 값

    void Update()
    {
        if (unitChairController.isBack)
        {
            // 등받이가 뒤로 기울어진 상태일 때
            UpdatePositionAndRotation(rootPositionOffsetWhenBack, waistRotationOffsetWhenBack);
            //riggedAnimator.SetBool("RiggedBack", true);
        }
        else
        {
            // 등받이가 기본 상태일 때
            UpdatePositionAndRotation(rootPositionOffsetWhenUpright, waistRotationOffsetWhenUpright);
            //riggedAnimator.SetBool("RiggedBack", false);
        }
    }

    private void UpdatePositionAndRotation(Vector3 positionOffset, Vector3 rotationOffset)
    {
        // 1. 캐릭터 최상위 Transform 위치를 조정
        Vector3 targetRootPosition = chairBack.position
                                    + chairBack.up * positionOffset.y
                                    + chairBack.right * positionOffset.x
                                    + chairBack.forward * positionOffset.z;
        characterRoot.position = targetRootPosition;

        // 2. Waist 본 회전을 조정
        Quaternion targetWaistRotation = chairBack.rotation * Quaternion.Euler(rotationOffset);
        waistBone.rotation = targetWaistRotation;
    }
}

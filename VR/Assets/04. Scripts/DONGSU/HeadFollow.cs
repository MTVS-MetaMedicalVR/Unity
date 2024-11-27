using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollow : MonoBehaviour
{
    public Transform neckTransform; // 머리와 목의 기준이 되는 Transform
    public Transform fabric; // 천의 Transform

    public float nowRotation = 0; // 머리의 현재 회전값
    public float fabricRotation = 0; // 천의 현재 회전값

    public bool isLeft = false; // 머리를 왼쪽으로 돌릴지 여부
    public bool isRight = false; // 머리를 오른쪽으로 돌릴지 여부

    public float headTurnSpeed = 45f; // 머리 회전 속도
    public float fabricTurnSpeed = 3f; // 천 회전 속도

    // Update is called once per frame
    void LateUpdate()
    {
        // 1. 머리의 위치를 목의 위치에 동기화
        transform.position = neckTransform.position;

        // 2. 머리 회전값 계산
        if (isLeft)
        {
            nowRotation += Time.deltaTime * -headTurnSpeed; // 왼쪽으로 회전
            fabricRotation = Mathf.Lerp(fabricRotation, 15f, Time.deltaTime * fabricTurnSpeed); // 천을 부드럽게 왼쪽으로 회전
        }
        else if (isRight)
        {
            nowRotation += Time.deltaTime * headTurnSpeed; // 오른쪽으로 회전
            fabricRotation = Mathf.Lerp(fabricRotation, -15f, Time.deltaTime * fabricTurnSpeed); // 천을 부드럽게 오른쪽으로 회전
        }
        else
        {
            nowRotation = Mathf.Lerp(nowRotation, 0, Time.deltaTime * 10f); // 머리를 정면으로 복귀
            fabricRotation = Mathf.Lerp(fabricRotation, 0, Time.deltaTime * fabricTurnSpeed); // 천을 정면으로 복귀
        }

        // 머리 회전값을 제한 (-35도 ~ 35도)
        nowRotation = Mathf.Clamp(nowRotation, -35f, 35f);

        // 3. 머리 회전 적용
        Vector3 rotateDir = Quaternion.AngleAxis(nowRotation, neckTransform.up) * neckTransform.forward;
        transform.rotation = Quaternion.LookRotation(rotateDir, neckTransform.up);

        // 4. 천의 회전 적용
        fabric.localRotation = Quaternion.Euler(0, 0, fabricRotation);
    }
}

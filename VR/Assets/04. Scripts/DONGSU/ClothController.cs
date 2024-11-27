using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothController : MonoBehaviour
{
    public GameObject cloth;
    public UnitChairController unitChairController;

    // 코루틴 중복 실행 방지용
    private bool isCoroutineRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        cloth.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (unitChairController.isBack && !isCoroutineRunning)
        {
            StartCoroutine(ActivateClothAfterDelay(3.5f));
        }
        else if (!unitChairController.isBack)
        {
            // isBack = false일 때 cloth 비활성화 및 코루틴 초기화
            cloth.SetActive(false);
            isCoroutineRunning = false;
            StopAllCoroutines();
        }


    }

    // 일정 시간 후 Cloth 활성화하는 코루틴
    IEnumerator ActivateClothAfterDelay(float delay)
    {
        isCoroutineRunning = true;
        yield return new WaitForSeconds(delay);
        cloth.SetActive(true);
    }
}

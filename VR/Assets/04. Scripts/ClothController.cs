using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothController : MonoBehaviour
{
    public GameObject cloth;
    UnitChairController unitChairController;

    // �ڷ�ƾ �ߺ� ���� ������
    private bool isCoroutineRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        unitChairController = GetComponent<UnitChairController>();
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
            // isBack = false�� �� cloth ��Ȱ��ȭ �� �ڷ�ƾ �ʱ�ȭ
            cloth.SetActive(false);
            isCoroutineRunning = false;
            StopAllCoroutines();
        }


    }

    // ���� �ð� �� Cloth Ȱ��ȭ�ϴ� �ڷ�ƾ
    IEnumerator ActivateClothAfterDelay(float delay)
    {
        isCoroutineRunning = true;
        yield return new WaitForSeconds(delay);
        cloth.SetActive(true);

        // �� ������ ������ �Ǹ� Neck ���� ������Ʈ�� ����
        cloth.transform.SetParent(GameObject.Find("NeckTwist02").transform);
    }
}

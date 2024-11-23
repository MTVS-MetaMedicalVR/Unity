using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairLightController : MonoBehaviour
{
    private UnitChairController unitChairController;
    
    public MeshRenderer bulbMeshRenderer;
    public GameObject Chairlight;

    // Start is called before the first frame update
    void Start()
    {
        // UnitChairController �ʱ�ȭ
        unitChairController = GetComponent<UnitChairController>();
        if (unitChairController == null)
        {
            Debug.Log("Light : unitChairController�� �����ϴ�.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bulbMeshRenderer != null)
        {
            // ���� ������ ���� ������ �� ���� ����
            if (unitChairController.isLight)
            {
                bulbMeshRenderer.material.EnableKeyword("_EMISSION");
                Chairlight.SetActive(true);
            }
            else
            {
                bulbMeshRenderer.material.DisableKeyword("_EMISSION");
                Chairlight.SetActive(false);
            }
        }
    }
}

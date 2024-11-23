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
        // UnitChairController 초기화
        unitChairController = GetComponent<UnitChairController>();
        if (unitChairController == null)
        {
            Debug.Log("Light : unitChairController가 없습니다.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bulbMeshRenderer != null)
        {
            // 불이 켜졌을 때와 꺼졌을 때 색상 변경
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

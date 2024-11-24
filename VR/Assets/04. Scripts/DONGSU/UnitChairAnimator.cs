using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitChairAnimator : MonoBehaviour
{
    public Animator animator;
    public bool isBase = false;
    public bool isBack = false;
    public bool isLight = false;
    public bool isTable = false;

    public List<MeshRenderer> buttonMRList = new List<MeshRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        buttonMRList.Add(GameObject.Find("Button_Base_Button").GetComponent<MeshRenderer>());
        buttonMRList.Add(GameObject.Find("Button_Back_Button").GetComponent<MeshRenderer>());
        buttonMRList.Add(GameObject.Find("Button_Light_Button").GetComponent<MeshRenderer>());
        buttonMRList.Add(GameObject.Find("Button_Table_Button").GetComponent<MeshRenderer>());

    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public void TableOn()
    {
        animator.SetTrigger("TableOn");
        isTable = true;
        buttonMRList[3].material.SetColor("_BaseColor", Color.green);
    }

    public void TableOff()
    {
        animator.SetTrigger("TableOff");
        isTable = false;
        buttonMRList[3].material.SetColor("_BaseColor", Color.white);
    }

    public void BackOn()
    {
        animator.SetTrigger("BackOn");
        isBack = true;
        buttonMRList[1].material.SetColor("_BaseColor", Color.green);


    }

    public void BackOff()
    {
        animator.SetTrigger("BackOff");
        isBack = false;
        buttonMRList[1].material.SetColor("_BaseColor", Color.white);
    }

    public void BaseOn()
    {
        animator.SetTrigger("BaseOn");
        isBase = true;
        buttonMRList[0].material.SetColor("_BaseColor", Color.green);


    }

    public void BaseOff()
    {
        animator.SetTrigger("BaseOff");
        isBase = false;
        buttonMRList[0].material.SetColor("_BaseColor", Color.white);
    }

    public void LightOn()
    {
        animator.SetTrigger("LightOn");
        isLight = true;
        buttonMRList[2].material.SetColor("_BaseColor", Color.green);

    }

    public void LightOff()
    {
        animator.SetTrigger("LightOff");
        isLight = false;
        buttonMRList[2].material.SetColor("_BaseColor", Color.white);
    }

}

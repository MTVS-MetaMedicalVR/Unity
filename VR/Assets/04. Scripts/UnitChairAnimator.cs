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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TableOn()
    {
        animator.SetTrigger("TableOn");
        isTable = true;
    }

    public void TableOff()
    {
        animator.SetTrigger("TableOff");
        isTable = false;
    }

    public void BackOn()
    {
        animator.SetTrigger("BackOn");
        isBack = true;
    }

    public void BackOff()
    {
        animator.SetTrigger("BackOff");
        isBack = false;
    }

    public void BaseOn()
    {
        animator.SetTrigger("BaseOn");
        isBase = true;
    }

    public void BaseOff()
    {
        animator.SetTrigger("BaseOff");
        isBase = false;
    }

    public void LightOn()
    {
        animator.SetTrigger("LightOn");
        isLight = true;
    }

    public void LightOff()
    {
        animator.SetTrigger("LightOff");
        isLight = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitChairAnimator : MonoBehaviour
{
    public Animator animator;

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
    }

    public void TableOff()
    {
        animator.SetTrigger("TableOff");
    }

    public void BackOn()
    {
        animator.SetTrigger("BackOn");
    }

    public void BackOff()
    {
        animator.SetTrigger("BackOff");
    }

    public void BaseOn()
    {
        animator.SetTrigger("BaseOn");
    }

    public void BaseOff()
    {
        animator.SetTrigger("BaseOff");
    }

    public void LightOn()
    {
        animator.SetTrigger("LightOn");
    }

    public void LightOff()
    {
        animator.SetTrigger("LightOff");
    }

}

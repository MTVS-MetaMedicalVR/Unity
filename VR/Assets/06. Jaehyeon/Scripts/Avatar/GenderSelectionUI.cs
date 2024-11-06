using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderSelectionUI : MonoBehaviour
{
    // PlayerSelectionManager와 연결된 메서드를 호출하는 역할을 담당합니다.

    public void OnSelectMale()
    {
        FindObjectOfType<PlayerSelectionManager>().SelectPlayer("Male");
    }

    public void OnSelectFemale()
    {
        FindObjectOfType<PlayerSelectionManager>().SelectPlayer("Female");
    }
}


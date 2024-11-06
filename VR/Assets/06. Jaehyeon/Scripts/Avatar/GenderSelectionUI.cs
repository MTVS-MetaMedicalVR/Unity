using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderSelectionUI : MonoBehaviour
{
    // PlayerSelectionManager�� ����� �޼��带 ȣ���ϴ� ������ ����մϴ�.

    public void OnSelectMale()
    {
        FindObjectOfType<PlayerSelectionManager>().SelectPlayer("Male");
    }

    public void OnSelectFemale()
    {
        FindObjectOfType<PlayerSelectionManager>().SelectPlayer("Female");
    }
}


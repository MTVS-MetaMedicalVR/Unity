using UnityEngine;

public class Forceps : MonoBehaviour
{
    private Tooth currentTooth;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tooth"))
        {
            Tooth tooth = other.GetComponent<Tooth>();
            if (tooth != null && currentTooth == null)
            {
                currentTooth = tooth;
                Debug.Log($"������ ġ�Ƹ� ����: {other.name}");
                tooth.StartExtraction(transform);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tooth"))
        {
            Tooth tooth = other.GetComponent<Tooth>();
            if (currentTooth != null && tooth == currentTooth)
            {
                Debug.Log($"������ ġ�ƿ��� �и���: {other.name}");
                currentTooth.StopExtraction();
                currentTooth = null;
            }
        }
    }
}
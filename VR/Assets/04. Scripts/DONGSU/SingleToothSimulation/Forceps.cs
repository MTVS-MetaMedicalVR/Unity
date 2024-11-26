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
                Debug.Log($"포셉이 치아를 잡음: {other.name}");
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
                Debug.Log($"포셉이 치아에서 분리됨: {other.name}");
                currentTooth.StopExtraction();
                currentTooth = null;
            }
        }
    }
}
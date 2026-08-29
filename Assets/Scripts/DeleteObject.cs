using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        Destroy(other.gameObject);
    }
}

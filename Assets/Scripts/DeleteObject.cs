using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    [SerializeField] private string _tagCompare;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_tagCompare))
        {
            Destroy(other.gameObject);

            // TODO:
            // - Fix ontrigger enter for cardriving speed. Find out why trigger not called
            // - Fix to have the time second spawner based on time too
        }
    }
}

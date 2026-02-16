using UnityEngine;

public class Gate : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Key key = GameObject.FindWithTag("Key").GetComponent<Key>();
            if (key.IsCollected)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}

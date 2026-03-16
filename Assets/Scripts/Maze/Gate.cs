using UnityEngine;

public class Gate : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControl player = other.gameObject.GetComponent<PlayerControl>();
            if (player.hasKey)
            {
                this.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}

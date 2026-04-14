using UnityEngine;

public class Bullet : MonoBehaviour
{
    //temp script for water bullets
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collision");
        collision.gameObject.GetComponent<JackOLanternAI>().Stun();
        Destroy(gameObject);
    }
}

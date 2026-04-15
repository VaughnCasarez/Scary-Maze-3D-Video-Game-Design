using UnityEngine;

public class Bullet : MonoBehaviour
{
    //temp script for water bullets
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<JackOLanternAI>() != null)
        {
            collision.gameObject.GetComponent<JackOLanternAI>().Stun();
            Destroy(gameObject);
        }
        
    }
}

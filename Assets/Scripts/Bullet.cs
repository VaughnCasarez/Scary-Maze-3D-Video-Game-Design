using UnityEngine;

public class Bullet : MonoBehaviour
{
    //temp script for water bullets
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name.Contains("Pumpkin"))
        {
            Debug.Log("collision");
            collision.gameObject.GetComponent<JackOLanternAI>().Stun();
        } 
        Destroy(gameObject);
    }
}

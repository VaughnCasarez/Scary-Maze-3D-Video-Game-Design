using UnityEngine;

public class Bullet : MonoBehaviour
{
    //temp script for water bullets
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("collision");
            Destroy(collision.gameObject);
        } 
        Destroy(gameObject);
    }
}

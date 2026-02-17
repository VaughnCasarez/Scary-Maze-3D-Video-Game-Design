using UnityEngine;

public class Bullet : MonoBehaviour
{
    //temp script for water bullets
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        } 
        Destroy(gameObject);
    }
}

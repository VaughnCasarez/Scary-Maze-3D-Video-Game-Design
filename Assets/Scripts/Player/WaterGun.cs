using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;

    private Transform shootPoint;
    void Start()
    {
        shootPoint = transform.Find("shoot point");
    }

    public void Shoot(float speed, Vector3 direction)
    {
        GameObject b = Instantiate(bullet, shootPoint);
        b.GetComponent<Rigidbody>().linearVelocity = speed * direction.normalized;
    }
}

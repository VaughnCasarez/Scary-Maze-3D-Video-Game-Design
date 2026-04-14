using UnityEngine;

public class WaterGun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private ParticleSystem flash;

    private Transform shootPoint;
    void Start()
    {
        shootPoint = transform.Find("shoot point");
    }

    public void Shoot(float speed, Vector3 direction)
    {
        Quaternion bulletRotation = Quaternion.LookRotation(direction.normalized) * Quaternion.Euler(90f, 0f, 0f);
        GameObject b = Instantiate(bullet, shootPoint.position, bulletRotation);
        b.GetComponent<Rigidbody>().linearVelocity = speed * direction.normalized;
        flash.Play();
    }
}

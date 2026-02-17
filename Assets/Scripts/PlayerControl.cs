using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 150f; 
    public float shootSpeed = 10f;

    public bool inKeyRange;
    public bool hasKey;
    private WaterGun gun;
    private Key key;
    void Start()
    {
        gun = transform.Find("gun").GetComponent<WaterGun>();
        key = GameObject.FindWithTag("Key").GetComponent<Key>();
    }
    void Update()
    {
        //for cam and player
        float move =  0f;
        float turn  = 0f;
        if (Keyboard.current.wKey.isPressed)  
            move =1f; 
        if (Keyboard.current.sKey.isPressed)  
            move =-1f;
        if (Keyboard.current.aKey.isPressed)
            turn =-1f;  
        if (Keyboard.current.dKey.isPressed)
            turn =1f;
        if (Input.GetKeyUp(KeyCode.F)) {
            gun.Shoot(shootSpeed, transform.forward);
        } 
        if (inKeyRange && Input.GetKeyUp(KeyCode.E)) {
            key.CollectKey();
            hasKey = true;
        } 
        transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("You Lost!");
            Time.timeScale = 0f;
            Destroy(gameObject);
        }
    }
}

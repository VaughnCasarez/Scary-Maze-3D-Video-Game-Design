using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 150f; 
    public float shootSpeed = 10f;
    public bool seesScarecrow = false;

    public float doubleTime = 0f;
    private bool isSprinting = false;
    private bool isCrouching = false;
    
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
        //sprint double tap
        if (Keyboard.current.wKey.wasPressedThisFrame){
            if (Time.time - doubleTime < .3f){
                isSprinting = true;
            }
            doubleTime = Time.time;
        }
        // crouching -L

        if (Keyboard.current.lKey.wasPressedThisFrame){
            isCrouching = !isCrouching;
            if (isCrouching){
                transform.localScale = new Vector3(1f, 0.5f, 1f);
            } else {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        if (Keyboard.current.wKey.wasReleasedThisFrame){
            isSprinting = false;
        }
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

        float currentSpeed = moveSpeed;
        if (isSprinting){
            currentSpeed = 10f;
        } 
        if (isCrouching)
        {
            currentSpeed = 2f;
        } 

        transform.Translate(Vector3.forward * move * currentSpeed  * Time.deltaTime);
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);

        //CheckScarecrowRange();
    }

    // private void CheckScarecrowRange()
    // {
    //     Vector3 rayStartPoint = transform.position + transform.forward * 0.5f;
    //     Vector3 rayDirection = transform.forward;
    //     Raycast hit;

    //     if (Physics.Raycast(rayOrigin, rayDirection, out hit, 100f, 6))
    //     {
    //         seesScarecrow = true;
    //     }
    // }

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

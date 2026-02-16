using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 150f; 
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
        transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float doubleTime = 0f;
    private bool isSprinting = false;
    private bool isCrouching = false;


    void Update()
    {
        //for cam and player
        float move =  0f;
        float turn  = 0f;
        if (Keyboard.current.wKey.wasPressedThisFrame){
            if (Time.time - doubleTime <.3f){
                isSprinting = true;
            }
            doubleTime = Time.time;
        }

        // sprinting
        if (Keyboard.current.wKey.wasReleasedThisFrame){
            isSprinting = false;
        }

        // crouching
        if (Keyboard.current.lKey.wasPressedThisFrame){
            isCrouching = !isCrouching;
            if (isCrouching){
                transform.localScale = new Vector3(1f, 0.5f, 1f);
            } else {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        if (Keyboard.current.wKey.isPressed){
            move =1f; 
        }
            
        if (Keyboard.current.sKey.isPressed){
            move =-1f;
        }
        if (Keyboard.current.aKey.isPressed){
            turn =-1f; 
        }
             
        if (Keyboard.current.dKey.isPressed){
            turn =1f;
        }

        //figure out what speed
        float currentSpeed = 5f;
        if (isSprinting) currentSpeed = 10f;
        if (isCrouching) currentSpeed = 2f;
            
        transform.Translate(Vector3.forward * move * currentSpeed  * Time.deltaTime);
        transform.Rotate(Vector3.up * turn * 150f * Time.deltaTime);
    }
}

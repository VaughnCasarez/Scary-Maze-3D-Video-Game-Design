using UnityEngine;

public class WeepingAngel : MonoBehaviour
{
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private float moveSpeed = 3f;
    
    private GameObject player;
    private Camera playerCamera;

    // State machine
    public enum AngelState { SeekPlayer, Stop }
    public AngelState currentState;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        
        playerCamera = player.GetComponentInChildren<Camera>();
        currentState = AngelState.SeekPlayer;
    }

    void Update()
    {
        if (player == null || playerCamera == null)
            return;

        if (IsInPlayerLineOfSight())
        {
            currentState = AngelState.Stop;
        }
        else
        {
            currentState = AngelState.SeekPlayer;
        }

        switch(currentState)
        {
            case AngelState.SeekPlayer:
                SeekPlayer();
                break;
            case AngelState.Stop:
                // do nothing
                break;
        }
    }

    private void SeekPlayer()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
    }
    
    private bool IsInPlayerLineOfSight()
    {
        Vector3 directionToAngel = transform.position - playerCamera.transform.position;
        float distToAngel = directionToAngel.magnitude;
        
        // If angel is too far or if not within the player's angle of view
        // Camera angle is a bit weird, maybe instead of playerCamera, should be player's center?
        float angleToAngel = Vector3.Angle(playerCamera.transform.forward, directionToAngel);
        if (distToAngel > detectionRange || angleToAngel > viewAngle)
            return false;
        
        Vector3 origin = playerCamera.transform.position;
        Vector3 dir = directionToAngel.normalized;
        RaycastHit hit;

        // if player raycast hits angel
        if (Physics.Raycast(origin, dir, out hit, distToAngel))
        {
            Debug.DrawLine(origin, hit.point, hit.collider.gameObject == gameObject ? Color.green : Color.red);
            return hit.collider.gameObject == gameObject;
        }
        Debug.DrawLine(origin, transform.position, Color.green);
        return true;
    }
}

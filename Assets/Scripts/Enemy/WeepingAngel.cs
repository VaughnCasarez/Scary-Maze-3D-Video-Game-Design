using UnityEngine;
using UnityEngine.AI;

public class WeepingAngel : MonoBehaviour
{
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private float moveSpeed = 3f;
    
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject player;
    private Camera playerCamera;

    private bool isActive;

    // State machine
    public enum AngelState { SeekPlayer, Stop }
    public AngelState currentState;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed; 
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        
        playerCamera = player.GetComponentInChildren<Camera>();
        currentState = AngelState.Stop;
    }

    void Update()
    {
        if (player == null || playerCamera == null|| !isActive)
            return;

        if (IsInPlayerLineOfSight())
        {
            anim.SetTrigger("idle");
            currentState = AngelState.Stop;
        }
        else
        {
            anim.SetTrigger("walk");
            currentState = AngelState.SeekPlayer;
        }

        switch(currentState)
        {
            case AngelState.SeekPlayer:
                SeekPlayer();
                break;
            case AngelState.Stop:
                agent.velocity = Vector3.zero;
                break;
        }
    }

    private void SeekPlayer()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        //transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
        agent.SetDestination(player.transform.position);
    }
    
    private bool IsInPlayerLineOfSight()
    {
        Vector3 directionToAngel = transform.position - playerCamera.transform.position;
        float distToAngel = directionToAngel.magnitude;
        
        // If angel is too far or if not within the player's angle of view
        // Camera angle is a bit weird, maybe instead of playerCamera, should be player's center?
        float angleToAngel = Vector3.Angle(player.transform.forward, directionToAngel);
        if (distToAngel > detectionRange || angleToAngel > viewAngle)
            return false;
        
        Vector3 origin = playerCamera.transform.position;
        Vector3 dir = directionToAngel.normalized;
        RaycastHit hit;

        // if player raycast hits angel
        if (Physics.Raycast(origin, dir, out hit, distToAngel, this.gameObject.layer))
        {
            Debug.DrawLine(origin, hit.point, hit.collider.gameObject == gameObject ? Color.green : Color.red);
            return hit.collider.gameObject == gameObject;
        }
        Debug.DrawLine(origin, transform.position, Color.green);
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isActive = true;
        }
    }
}

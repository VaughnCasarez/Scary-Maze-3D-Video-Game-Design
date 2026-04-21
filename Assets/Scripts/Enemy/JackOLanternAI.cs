using UnityEngine;
using UnityEngine.AI;
public class JackOLanternAI : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float stunTime = 5f;
    public GameObject light;
    public BoxCollider aggroCollider;
    private Transform player;
    private Animator anim;
    private NavMeshAgent agent;
    private bool isActive = false;
    private bool isStunned = false;
    private float curStunTime = 0f;

    private Vector3 startPos;
    private PlayerControl playerControl;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        anim = GetComponent<Animator>();
        anim.SetBool("isChasing", false);
        startPos = transform.position;
    }

    void Update()
    {
        if (!isActive) {
            return;
        }
        if (!isStunned && player != null && (playerControl == null || !playerControl.IsCrouching))
        {
            anim.SetBool("isChasing", true);

            Vector3 victim = player.position;
            victim.y = transform.position.y;

            agent.SetDestination(victim);
            //transform.position = Vector3.MoveTowards(transform.position, victim, movementSpeed * Time.deltaTime);
        }
        else
        {
            anim.SetBool("isChasing", false);
            curStunTime += Time.deltaTime;
            if (curStunTime > stunTime)
            {
                isStunned = false;
                light.SetActive(true);
                // GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
                // GetComponent<Rigidbody>().isKinematic = false;
                agent.isStopped = false;
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.transform;
            playerControl = other.GetComponent<PlayerControl>();
            isActive = true;
            aggroCollider.enabled = false;
        }

    }

    public void Stun()
    {
        // GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, 0f, 0f);
        isStunned = true;
        curStunTime = 0f;
        light.SetActive(false);
        anim.SetBool("isChasing", false);
        // GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        // GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        // GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        // GetComponent<Rigidbody>().isKinematic = true;
        agent.isStopped = true;
    }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.GetComponent<LittleKid>() != null)
    //     {
    //         inRange = false;
    //     }
    // }
}

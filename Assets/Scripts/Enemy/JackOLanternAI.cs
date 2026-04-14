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
        if (!isStunned && player != null)
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
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.transform;
            isActive = true;
            aggroCollider.enabled = false;
        }
    }

    public void Stun()
    {
        GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, 0f, 0f);
        isStunned = true;
        curStunTime = 0f;
        light.SetActive(false);
        anim.SetBool("isChasing", false);
    }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.GetComponent<LittleKid>() != null)
    //     {
    //         inRange = false;
    //     }
    // }
}

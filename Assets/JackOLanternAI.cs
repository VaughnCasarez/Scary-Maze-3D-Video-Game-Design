using UnityEngine;
using UnityEngine.AI;
public class JackOLanternAI : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float stunTime = 5f;
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
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<LittleKid>() != null)
        {
            player = other.transform;
            isActive = true;
        }
    }

    public void Stun()
    {
        isStunned = true;
        curStunTime = 0f;
    }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.GetComponent<LittleKid>() != null)
    //     {
    //         inRange = false;
    //     }
    // }
}

using UnityEngine;
using UnityEngine.AI;
public class JackOLanternAI : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float stunTime = 5f;
    public AudioClip[] footstepSfx;
    public float footstepInterval = 0.45f;
    public GameObject light;
    public BoxCollider aggroCollider;
    private Transform player;
    private Animator anim;
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private bool isActive = false;
    private bool isStunned = false;
    private float curStunTime = 0f;
    private float footstepTimer = 0f;

    private Vector3 startPos;
    private PlayerControl playerControl;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        anim.SetBool("isChasing", false);
        startPos = transform.position;
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        if (!isStunned && player != null && (playerControl == null || !playerControl.IsCrouching))
        {
            anim.SetBool("isChasing", true);

            Vector3 victim = player.position;
            victim.y = transform.position.y;

            agent.SetDestination(victim);

            Vector3 direction = (victim - transform.position).normalized;
            if (direction != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(direction);
            }
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

        HandleFootstepAudio();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.transform;
            playerControl = other.GetComponent<PlayerControl>();
            if (!playerControl.IsCrouching)
            {
                isActive = true;
                aggroCollider.enabled = false;
            }
            
        }

    }

    public void Stun()
    {
        GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, 0f, 0f);
        isStunned = true;
        curStunTime = 0f;
        footstepTimer = 0f;
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        light.SetActive(false);
        anim.SetBool("isChasing", false);
    }

    private void HandleFootstepAudio()
    {
        if (audioSource == null || footstepSfx == null || footstepSfx.Length == 0)
        {
            return;
        }

        bool isMoving = isActive && !isStunned && agent != null && agent.velocity.sqrMagnitude > 0.05f;
        if (!isMoving)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;
        if (footstepTimer > 0f)
        {
            return;
        }

        int randomIndex = Random.Range(0, footstepSfx.Length);
        audioSource.PlayOneShot(footstepSfx[randomIndex]);
        footstepTimer = footstepInterval;
    }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.GetComponent<LittleKid>() != null)
    //     {
    //         inRange = false;
    //     }
    // }
}

using UnityEngine;

public class JackOLanternAI : MonoBehaviour
{
    public float movementSpeed = 2f;
    private Transform player;
    private Animator anim;
    private bool inRange = false;
    private Vector3 startPos;

    void Awake()
        {
            anim = GetComponent<Animator>();
            startPos = transform.position;
        }

        void Update()
        {
            if (inRange && player != null)
            {
                anim.SetBool("isChasing", true);

                Vector3 victim = player.position;
                victim.y = transform.position.y;

                transform.position = Vector3.MoveTowards(transform.position, victim, movementSpeed * Time.deltaTime);
            }
            else
            {
                anim.SetBool("isChasing", false);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<LittleKid>() != null)
            {
                player = other.transform;
                inRange = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<LittleKid>() != null)
            {
                inRange = false;
            }
        }
}

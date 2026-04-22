using UnityEngine;

public enum CandyType {LIFE, TIMER, BULLET}

[RequireComponent(typeof(Animator))]
public class Candy : MonoBehaviour
{
    private GameManager manager;
    private Animator anim;
    [SerializeField] private CandyType type = CandyType.LIFE;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            anim.ResetTrigger("fall");
            anim.SetTrigger("float");
            c.gameObject.GetComponent<PlayerControl>().inCandyRange = true;
            c.gameObject.GetComponent<PlayerControl>().curCandy = this;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            anim.ResetTrigger("float");
            anim.SetTrigger("fall");
            c.gameObject.GetComponent<PlayerControl>().inCandyRange = false;
            c.gameObject.GetComponent<PlayerControl>().curCandy = null;
        }
    }

    public void CollectCandy(PlayerControl player)
    {
        if (type == CandyType.LIFE)
        {
            player.GainHealth(1);
        } else if (type == CandyType.BULLET)
        {
            player.GainBullet(1);
        } else
        {
            manager.GainTime(15f);
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}

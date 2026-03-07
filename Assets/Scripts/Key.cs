using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Key : MonoBehaviour
{
    [SerializeField] private bool isCollected;

    public bool IsCollected {get {return isCollected; } set {isCollected = value;}}
    private Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            anim.ResetTrigger("fall");
            anim.SetTrigger("float");
            c.gameObject.GetComponent<PlayerControl>().inKeyRange = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            anim.ResetTrigger("float");
            anim.SetTrigger("fall");
            c.gameObject.GetComponent<PlayerControl>().inKeyRange = false;
        }
    }

    public void CollectKey()
    {
        Deactivate();
    }

    private void Deactivate()
    {
        this.transform.Find("key holder").gameObject.SetActive(false);
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}

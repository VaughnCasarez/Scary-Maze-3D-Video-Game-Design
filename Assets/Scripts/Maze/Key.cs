using UnityEngine;
using UnityEngine.VFX;
using System;

[RequireComponent(typeof(Animator))]
public class Key : MonoBehaviour
{
    [SerializeField] private bool isCollected;
    [SerializeField] private GameObject keyBeam;
    public bool IsCollected {get {return isCollected; } set {isCollected = value;}}
    private Animator anim;
    private VisualEffect beam;
    private ParticleSystem particles;
    public Action KeyCollected;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        beam = GetComponentInChildren<VisualEffect>();
        particles = GetComponentInChildren<ParticleSystem>();
    }
    void Start()
    {
        beam.Reinit();
        beam.Play();
        particles.Play();
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
        KeyCollected?.Invoke();
        Deactivate();
    }

    private void Deactivate()
    {
        keyBeam.SetActive(false);
        this.transform.Find("key holder").gameObject.SetActive(false);
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}

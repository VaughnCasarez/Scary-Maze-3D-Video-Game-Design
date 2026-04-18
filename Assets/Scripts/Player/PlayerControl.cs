using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    public float turnSpeed = 150f;
    public float shootSpeed = 10f;
    [SerializeField] private int playerHealth = 5;
    private TextMeshProUGUI healthbarText; 
    private float nextDamageTime = 0f;

    [Header("Audio")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField][Range(0f, 1f)] private float walkVolume = 0.6f;
    [SerializeField][Range(0f, 1f)] private float shootVolume = 0.9f;

    [Header("Level Audio")]
    [SerializeField] private AudioClip backgroundTrack;
    [SerializeField][Range(0f, 1f)] private float backgroundVolume = 0.35f;


    public float doubleTime = 0f;
    private bool isSprinting = false;
    private bool isCrouching = false;

    public bool inKeyRange;
    public bool hasKey;
    private WaterGun gun;
    private Key key;
    private AudioSource playerAudioSource;
    private AudioSource backgroundAudioSource;
    private bool isWalkSoundPlaying;
    private bool hasEndedLevel;
    private Animator anim;
    public Action PlayerDeath;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isWalking", false);
        gun = transform.Find("gun").GetComponent<WaterGun>();
        key = GameObject.FindWithTag("Key").GetComponent<Key>();
        healthbarText = GameObject.FindWithTag("Healthbar").GetComponent<TextMeshProUGUI>();
        healthbarText.text = $"Health: {playerHealth}";

        playerAudioSource = GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            playerAudioSource = gameObject.AddComponent<AudioSource>();
        }
        playerAudioSource.playOnAwake = false;
        playerAudioSource.loop = true;
        playerAudioSource.clip = walkSound;

        StartLevelBackgroundTrack();
    }

    void Update()
    {
        //for cam and player
        float move = 0f;
        float turn = 0f;
        //sprint double tap
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            if (Time.time - doubleTime < .3f)
            {
                isSprinting = true;
            }
            doubleTime = Time.time;
        }
        // crouching -L

        if (Keyboard.current.ctrlKey.wasPressedThisFrame)
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                transform.localScale = new Vector3(1f, 0.5f, 1f);
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        if (Keyboard.current.shiftKey.wasReleasedThisFrame)
        {
            isSprinting = false;
        }
        if (Keyboard.current.wKey.isPressed)
            move = 1f;
        if (Keyboard.current.sKey.isPressed)
            move = -1f;
        if (Keyboard.current.aKey.isPressed)
            turn = -1f;
        if (Keyboard.current.dKey.isPressed)
            turn = 1f;
        if (Input.GetKeyUp(KeyCode.F))
        {
            gun.Shoot(shootSpeed, transform.forward);
            PlayShootSound();
        }
        if (inKeyRange && Input.GetKeyUp(KeyCode.E))
        {
            key.CollectKey();
            hasKey = true;
        }

        float currentSpeed = moveSpeed;
        if (isSprinting)
        {
            currentSpeed = 10f;
        }
        if (isCrouching)
        {
            currentSpeed = 2f;
        }

        HandleWalkAudio(move);
        if (move != 0f)
        {
            anim.SetBool("isWalking", true);
        } else
        {
            anim.SetBool("isWalking", false);
        }
        transform.Translate(Vector3.forward * move * currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);

    }

    void HandleWalkAudio(float move)
    {
        bool isWalking = Mathf.Abs(move) > 0.01f;
        if (isWalking)
        {
            if (!isWalkSoundPlaying && walkSound != null)
            {
                playerAudioSource.clip = walkSound;
                playerAudioSource.volume = walkVolume;
                playerAudioSource.loop = true;
                playerAudioSource.Play();
                isWalkSoundPlaying = true;
            }
            return;
        }

        if (isWalkSoundPlaying)
        {
            playerAudioSource.Stop();
            isWalkSoundPlaying = false;
        }
    }

    void PlayShootSound()
    {
        if (shootSound == null)
        {
            return;
        }
        playerAudioSource.PlayOneShot(shootSound, shootVolume);
    }

    void StartLevelBackgroundTrack()
    {
        if (backgroundTrack == null)
        {
            return;
        }

        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource.playOnAwake = false;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.clip = backgroundTrack;
        backgroundAudioSource.volume = backgroundVolume;
        backgroundAudioSource.spatialBlend = 0f;
        backgroundAudioSource.Play();
    }

    // void LoadEndScene(GameResult result)
    // {
    //     if (hasEndedLevel)
    //     {
    //         return;
    //     }

    //     hasEndedLevel = true;
    //     GameResultState.LastResult = result;
    //     Time.timeScale = 1f;
    //     UnityEngine.SceneManagement.SceneManager.LoadScene(endSceneName);
    // }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && Time.time > nextDamageTime)
        {
            playerHealth--;
            healthbarText.text = $"Health: {playerHealth}";
            Debug.Log($"Player hit! Remaining health: {playerHealth}");
            if (playerHealth <= 0)
            {
                Debug.Log(PlayerDeath == null);
                PlayerDeath?.Invoke();
                // LoadEndScene(GameResult.Lose);
            }
            nextDamageTime = Time.time + 1f; // 1 sec dmg cooldown
        }
    }
}

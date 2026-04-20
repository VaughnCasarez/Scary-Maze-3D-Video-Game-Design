using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    [Range(1f, 30f)] public float turnSpeed = 8f;
    public float shootSpeed = 10f;
    [SerializeField] private int playerHealth = 5;
    private float nextDamageTime = 0f;

    [Header("Audio")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField][Range(0f, 1f)] private float walkVolume = 0.6f;
    [SerializeField][Range(0f, 1f)] private float shootVolume = 0.9f;

    [Header("Level Audio")]
    [SerializeField] private AudioClip backgroundTrack;
    [SerializeField][Range(0f, 1f)] private float backgroundVolume = 0.35f;

    public bool IsCrouching => isCrouching;

    private float doubleTime = 0f;
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
    public Action<int> PlayerDamage;
    public Action PausePressed;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isWalking", false);
        gun = transform.Find("gun").GetComponent<WaterGun>();
        key = GameObject.FindWithTag("Key").GetComponent<Key>();

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
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PausePressed?.Invoke();
        }
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            if (Time.time - doubleTime < .3f)
                isSprinting = true;
            doubleTime = Time.time;
        }
        if (Keyboard.current.shiftKey.wasReleasedThisFrame)
            isSprinting = false;

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

        float moveX = 0f, moveZ = 0f;
        if (Keyboard.current.wKey.isPressed) moveZ = 1;
        if (Keyboard.current.sKey.isPressed) moveZ = -1;
        if (Keyboard.current.aKey.isPressed) moveX = -1;
        if (Keyboard.current.dKey.isPressed) moveX = 1;

        float move = new Vector2(moveX, moveZ).magnitude;
        Vector3 moveDir = (transform.forward * moveZ + transform.right * moveX).normalized;

        float mouseX = Mouse.current.delta.ReadValue().x * 0.1f;
        transform.Rotate(Vector3.up * mouseX * turnSpeed * Time.deltaTime);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            gun.Shoot(shootSpeed, transform.forward);
            PlayShootSound();
        }
        if (inKeyRange && Keyboard.current.eKey.wasReleasedThisFrame)
        {
            key.CollectKey();
            hasKey = true;
        }

        float currentSpeed = isSprinting ? 10f : isCrouching ? 1f : moveSpeed;

        HandleWalkAudio(move);
        anim.SetBool("isWalking", move != 0f);
        transform.Translate(moveDir * currentSpeed * Time.deltaTime, Space.World);
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

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && Time.time > nextDamageTime)
        {
            playerHealth--;
            PlayerDamage?.Invoke(playerHealth);
            Debug.Log($"Player hit! Remaining health: {playerHealth}");
            if (playerHealth <= 0)
            {
                // UnlockCursor();
                PlayerDeath?.Invoke();
            }
            nextDamageTime = Time.time + 1f; // 1 sec dmg cooldown
        }
    }

    // public static void UnlockCursor()
    // {
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    // }
}


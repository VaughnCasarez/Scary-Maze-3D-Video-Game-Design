using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    public float  turnSpeed = 150f;
    public float shootSpeed = 10f ;

    [Header("Audio")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField][Range(0f, 1f)] private float walkVolume = 0.6f;
    [SerializeField][Range(0f, 1f)] private float shootVolume = 0.9f;

    [Header("Level Audio")]
    [SerializeField] private AudioClip backgroundTrack;
    [SerializeField][Range(0f, 1f)] private float backgroundVolume = 0.35f;

    [Header("Scene")]
    [SerializeField] private string endSceneName = "End_Scene";

    // sprint stuff
    public float doubleTime = 0f;
    private bool isSprinting = false;

    // crouching / stealth
    private bool isCrouching = false;
    public bool isStealthed = false;

    public bool inKeyRange;
    public bool hasKey;
    private WaterGun gun;
    private Key key;
    private AudioSource playerAudioSource;
    private AudioSource backgroundAudioSource;
    private bool isWalkSoundPlaying;
    private bool hasEndedLevel;
    private Animator anim;

    // for mouse look
    private Camera playerCam;
    private float camX = 0f;

    void Start()
    {
        anim =GetComponent<Animator>();
        anim.SetBool("isWalking", false); 

        gun =transform.Find("gun").GetComponent<WaterGun>();
        key =GameObject.FindWithTag("Key" ).GetComponent<Key>();

        playerAudioSource =GetComponent<AudioSource >();
        if (playerAudioSource == null)
            playerAudioSource= gameObject.AddComponent<AudioSource>();

        playerAudioSource.playOnAwake =false;
        playerAudioSource.loop =true;
        playerAudioSource.clip =walkSound; 

        playerCam =Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible= false;

        StartLevelBackgroundTrack();
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleStealth();
        HandleShooting();
        HandleKeyPickup();
    }

    void HandleMouseLook()
{
    float mouseX = Mouse.current.delta.x.ReadValue() *0.2f;
    camX += mouseX;
    transform.rotation = Quaternion.Euler(0f, camX,0f);
}

    void HandleMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            if (Time.time - doubleTime < .3f)
                    isSprinting = true;
                doubleTime = Time.time;
        }
        if (Keyboard.current.shiftKey.wasReleasedThisFrame)
            isSprinting = false;

        if (Keyboard.current.wKey.isPressed ) moveZ =1f;
        if (Keyboard.current.sKey.isPressed ) moveZ =-1f;
        if (Keyboard.current.aKey.isPressed ) moveX =-1f;
        if (Keyboard.current.dKey.isPressed ) moveX =1f;

        // figure out speed
        float currentSpeed= moveSpeed;
        if (isSprinting&& !isCrouching )
            currentSpeed= 10f;
        if (isCrouching )
            currentSpeed= 1.5f; 

        Vector3 moveDir = (transform.forward *moveZ + transform.right* moveX).normalized;
        transform.position += moveDir* currentSpeed * Time.deltaTime;

        // walking animation
        bool isMoving = moveDir.magnitude >0.01f;
        anim.SetBool("isWalking", isMoving);
        if (isMoving)
        {
            HandleWalkAudio(1f) ;
        }
        else
        {
            HandleWalkAudio(0f);
        }
    }

    void HandleStealth()
    {
        // ctrl toggles crouch/stealth
        if (Keyboard.current.ctrlKey.wasPressedThisFrame)
        {
            isCrouching =!isCrouching;
            isStealthed =isCrouching;

            if (isCrouching)
            {
                transform.localScale =new Vector3(1f,0.5f, 1f);
                Debug.Log("Stealthed - enemies cant see you");
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                Debug.Log("Unstealth");
            }
        }
    }

    void HandleShooting()
    {
        // shoot
        bool shotFired =Mouse.current.leftButton.wasPressedThisFrame
                         || Input.GetKeyUp(KeyCode.F);

        if(shotFired)
        {
            gun.Shoot(shootSpeed, transform.forward);
            PlayShootSound();
        }
    }

    void HandleKeyPickup()
    {
        if(inKeyRange && Input.GetKeyUp(KeyCode.E))
        {
            key.CollectKey();
            hasKey = true;
        }
    }

    //audio stuff

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
        if (shootSound == null) return;
        playerAudioSource.PlayOneShot(shootSound, shootVolume);
    }

    void StartLevelBackgroundTrack()
    {
        if (backgroundTrack == null) return;

        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource.playOnAwake = false;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.clip = backgroundTrack;
        backgroundAudioSource.volume = backgroundVolume;
        backgroundAudioSource.spatialBlend = 0f;
        backgroundAudioSource.Play();
    }

    void LoadEndScene(GameResult result)
{
    if (hasEndedLevel)return;
    hasEndedLevel = true;
    GameResultState.LastResult =result;
    Time.timeScale =1f;

    // unlock cursor for end screen
    Cursor.lockState =CursorLockMode.None;
    Cursor.visible =true;

    UnityEngine.SceneManagement.SceneManager.LoadScene(endSceneName );
}
//
    void OnCollisionEnter(Collision other)
{
    if (other.gameObject.CompareTag("Enemy"))
    {
        Debug.Log("You Lost!");
        LoadEndScene(GameResult.Lose);
    }
}
}
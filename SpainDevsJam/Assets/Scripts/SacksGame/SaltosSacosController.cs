using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltosSacosController : MonoBehaviour
{
    [SerializeField] CharacterController controller = default;
    [SerializeField] public Animator anim;

    [SerializeField] bool isPlayable;

    [Header("Jump Settings")]
    [SerializeField, Min(0)] float jumpHeight;
    [SerializeField, Min(0)] float jumpDistance;
    [SerializeField, Min(0)] float jumpTime;
    [SerializeField] float gravity = -9.8f;

    [Header("Detect Collisions")]
    [SerializeField] Transform groundCheck = default;
    [SerializeField, Range(0, .5f)] float groundDistance = .4f;
    [SerializeField]LayerMask groundMask;
    bool isGrounded;

    [Header("Fall Settings")]
    [SerializeField] float fallingTime;

    [Header("IA Settings")]
    [SerializeField, Min(0)] float maxDesviation;

    Vector3 velocity;
    bool falling = false;
    bool gameFinished = false;
    bool gameStarted = false;

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material baseMaterial;
    [SerializeField] Material fallingMaterial;
    
    private float timeOfJump;
    private float timeSinceJump;

    private SacksGameController gameController;

    private void Awake()
    {
        if(gameObject.CompareTag("Player"))
            anim = GetComponent<CharacterIdController>().SetPlayerCustom();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayable && gameStarted && !gameFinished)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                CheckJump();
            }
        }
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            timeOfJump += Time.deltaTime;
        } else
        {
            timeOfJump = 0;
            velocity.y = -4f;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    public bool GetIsPlayable()
    {
        return isPlayable;
    }

    private void CheckJump()
    {
        if (IsGrounded())
        {
            if (!falling)
            {
                Jump();
            }
        } else
        {
            Fall();
        }
    }

    private void Fall()
    {
        StartCoroutine(FallCoroutine());
    }

    private void Jump()
    {
        StartCoroutine(JumpCoroutine());
        if (isPlayable)
        {
            AudioManager.instance.PlayOneShot("SFX_saltoSacos" + Random.Range(1, 5), true);
        }
        else
        {
            AudioManager.instance.PlayOneShot("SFX_saltoSacosEnemigos", true);
        }
    }

    private IEnumerator JumpCoroutine()
    {
        float i = 0;
        while (i < jumpTime)
        {
            anim.SetTrigger("SacksJump");
            Vector3 jumpVelocity = new Vector3(jumpDistance, jumpHeight, 0);
            controller.Move(jumpVelocity * Time.deltaTime);
            i += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FallCoroutine()
    {
        falling = true;
        meshRenderer.material = fallingMaterial;
        yield return new WaitForSecondsRealtime(fallingTime);
        meshRenderer.material = baseMaterial;
        falling = false;
    }

    private IEnumerator IA_Jump()
    {
        Debug.Log("IA Jump Started");
        float firstWait = Random.Range(0.1f, 0.3f);
        yield return new WaitForSecondsRealtime(firstWait);
        while (!gameFinished)
        {
            float totalTimeToWait = jumpTime;
            int desviation = Random.Range(0,14);
            float timeToWait = 0;
            if (desviation > 0)
            {
                timeToWait = Random.Range(0.12f, 0.15f);
                totalTimeToWait += timeToWait;
            } else
            {
                timeToWait = Random.Range(0.05f, 0.1f);
                totalTimeToWait += timeToWait;
            }
            CheckJump();
            yield return new WaitForSecondsRealtime(totalTimeToWait);
        }
    }

    public void StartGame(SacksGameController controller)
    {
        gameController = controller;
        gameStarted = true;
        if (isPlayable == false)
        {
            StartCoroutine(IA_Jump());
        }
    }

    public void GameOver()
    {
        gameFinished = true;
        
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine"))
        {
            gameController.GameOver(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairsPlayerController : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] CharacterController controller = default;
    [SerializeField] Transform cam = default;
    [SerializeField] ChairsGameController gameController;

    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float aceleration;
    [SerializeField] float speed = 6;

    [SerializeField, Range(0, 2f)] float turnSmoothTime = .1f;
    float turnSmoothVelocity;

    [Header("Jump")]
    [SerializeField, Min(0)] float gravity = 15f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField, Range(0, .5f)] float jumpDelay = .5f;
    Vector3 velocity;

    [SerializeField] Transform groundCheck = default;
    [SerializeField, Range(0, .5f)] float groundDistance = .4f;
    [SerializeField] LayerMask groundMask = -1;
    bool isGrounded;

    [SerializeField, Range(0f, .3f)] float jumpPressedRememberTime = .2f;
    float jumpPressedRemember;

    [Header("Randomize Target X")]
    [SerializeField] float randomX = 0;
    [SerializeField] float randomZ = 0;


    [SerializeField] float stunDuration;
    private float actualStunCoolDown;
    [SerializeField] private float stunCoolDown;

    /*[HideInInspector]*/
    public Vector3 startPosition;

    bool randomMove = false;

    /*[HideInInspector]*/
    public bool sat = false;

    public bool paused = false;

    private float lastSped;
    private float lastHorizontal;
    private float lastVertical;
    private Vector3 lastDir;

    [SerializeField, Range(0, 1f)] float stepsInterval = .35f;
    float currentStepTime;

    Vector3 randomDirection;

    /*
    Vector3[] directions = {
        Vector3.right,
        Vector3.left,
        Vector3.back
    };
    */

    [HideInInspector] public bool stunned = false;

    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //StartCoroutine(SetRandomDirection());
        startPosition = this.transform.position;

        anim = GetComponent<CharacterIdController>().SetPlayerCustom();
    }

    /*
    IEnumerator SetRandomDirection()
    {
        randomMove = true;
        randomDirection = directions[Random.Range(0, directions.Length)];

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        
        randomMove = false;

        yield return new WaitForSeconds(Random.Range(1f, 2f));

        StartCoroutine(SetRandomDirection());
    }*/

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            if (!sat && !stunned)
            {
                isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

                if (isGrounded && velocity.y < 0)
                {
                    velocity.y = -2f;
                }

                float horizontal = Input.GetAxisRaw("Horizontal");
                float vertical = Input.GetAxisRaw("Vertical");
                

                Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

                if (direction.magnitude >= .1f)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                    //transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                    if (randomMove)
                    {
                        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * randomDirection;
                    }

                    moveDir = moveDir.normalized;
                    /*moveDir.x += randomX;
                    moveDir.z += randomZ;*/
                    if (speed < maxSpeed)
                    {
                        speed += aceleration * Time.deltaTime;
                    }

                    lastDir = moveDir;
                    lastSped = speed;

                    controller.Move(moveDir * speed * Time.deltaTime);

                    currentStepTime += Time.deltaTime;
                    if (currentStepTime >= stepsInterval){
                        AudioManager.instance.PlayOneShot("SFX_paso" + Random.Range(1, 6), true);
                        currentStepTime = 0;
                    }

                    anim.SetFloat("VelocityX", horizontal);
                    anim.SetFloat("VelocityY", vertical);
                } else
                {
                    ResetAnim();
                }

                jumpPressedRemember -= Time.deltaTime;
                if (Input.GetButtonDown("Jump"))
                {
                    jumpPressedRemember = jumpPressedRememberTime;
                }

                if (jumpPressedRemember > 0f && isGrounded)
                {
                    StartCoroutine(Jump(jumpDelay));
                    jumpPressedRemember = 0;
                }

                velocity.y -= gravity * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime);
            }

            if (actualStunCoolDown != 0)
            {
                actualStunCoolDown -= Time.deltaTime;
                if (actualStunCoolDown < 0)
                {
                    actualStunCoolDown = 0;
                }
            }
        }
    }

    IEnumerator Jump(float delay)
    {
        yield return new WaitForSeconds(delay);
        velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        jumpPressedRemember = 0;
        StopCoroutine("Jump");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chair"))
        {
            ChairsChairController chair = other.GetComponent<ChairsChairController>();
            if (!chair.occupied)
            {
                this.sat = true;
                chair.AssignSeat();
                ResetAnim();
                AudioManager.instance.PlayOneShot("SFX_campanillas");
            }
        }
        
        if (other.CompareTag("Enemy"))
        {
            ChairsEnemyController enemy = other.GetComponent<ChairsEnemyController>();
            DecideStun(enemy);
        }
        
    }

    private void DecideStun(ChairsEnemyController enemy)
    {
        float decision = Random.Range(0, 3);
        if (decision < 1)
        {
            if (actualStunCoolDown == 0 && !enemy.stunned)
            {
                Stun(enemy);
                actualStunCoolDown = stunCoolDown;
            }
        }
    }

    public void Loose()
    {
        anim.SetTrigger("DieWithoutGettingUp");
        paused = true;
    }

    public void ResetAnim()
    {
        anim.SetFloat("VelocityX", 0);
        anim.SetFloat("VelocityY", 0);
    }

    private void Stun(ChairsEnemyController enemy)
    {
        enemy.StunMyself();
    }

    public void StunMyself()
    {
        stunned = true;
        StartCoroutine(StunMyselfCoroutine());
    }

    private IEnumerator StunMyselfCoroutine()
    {
        ResetAnim();
        yield return new WaitForSecondsRealtime(stunDuration);
        stunned = false;
        //anim.SetBool("Stunned", false);
    }

    /*
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordPlayerController : MonoBehaviour
{
    [SerializeField] ScenesTransitionController scenesTransition = default;
    [SerializeField] GameTutorialControler tutorial = default;
    [SerializeField] Animator anim = default;
    [SerializeField] public Animator charAnim = default;
    [SerializeField] CharacterController controller = default;
    [SerializeField] Transform cam = default;
    [SerializeField] Transform enemy = default;

    [SerializeField] float speed = 6;

    [SerializeField, Range(0, .3f)] float turnSmoothTime = .1f;
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

    [Space]
    [SerializeField] Image healthBar = default;
    [SerializeField] float maxHp = 100;
    float hp;
    bool canMove;

    [Space]
    [SerializeField] GameObject playerHPbar;
    [SerializeField] GameObject enemyHPbar;
    [SerializeField] GameObject playerHPbarLabel;
    [SerializeField] GameObject enemyHPbarLabel;

    [SerializeField, Range(0, 1f)] float stepsInterval = .5f;
    float currentTime;

    bool scoreDistributed;

    void Awake(){
        Cursor.lockState = CursorLockMode.Locked;
        hp = maxHp;
        canMove = true;

        charAnim = GetComponent<CharacterIdController>().SetPlayerCustom();
    }

    float currentX;
    float xDirVelocity;
    float currentY;

    // Update is called once per frame
    void Update(){
        if (tutorial.gameStarted)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

            if (direction.magnitude >= .1f && canMove)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0, angle, 0);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);

                Vector3 directionX = direction.x * transform.forward;
                Vector3 directionZ = direction.z * transform.forward;
                currentX = Mathf.SmoothDamp(currentX, directionX.x, ref xDirVelocity, .1f);
                currentY = Mathf.SmoothDamp(currentY, directionZ.z, ref xDirVelocity, .1f);
                charAnim.SetFloat("VelocityY", 1);
                anim.SetBool("Moving", true);

                currentTime += Time.deltaTime;
                if (currentTime >= stepsInterval)
                {
                    AudioManager.instance.PlayOneShot("SFX_paso" + Random.Range(1, 6), true);
                    currentTime = 0;
                }
            }
            else
            {
                currentX = Mathf.SmoothDamp(currentX, 0, ref xDirVelocity, .1f);
                currentY = Mathf.SmoothDamp(currentY, 0, ref xDirVelocity, .1f);
                charAnim.SetFloat("VelocityY", 0);
                anim.SetBool("Moving", false);
            }

            velocity.y -= gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    public void StopMovement(){
        transform.LookAt(new Vector3(enemy.position.x, transform.position.y, enemy.position.z));
        canMove = false;
    }

    public void RegainMovement(){
        if(hp > 0){
            canMove = true;
        }
    }

    public void Damage(float ammount){
        if(hp > 0){
            hp -= ammount;
            healthBar.transform.localScale = new Vector3(Mathf.Clamp(hp / maxHp, 0, 1), 1, 1);

            ScreenShakeCall.instance.ShakeCamera(12.5f, .4f);

            if (hp > 0f){
                charAnim.SetTrigger("Damage");
                float randomAnim = Random.Range(0, 200);
                if(randomAnim > 100){
                    GameObject pow = ObjectPooler.instance.SpawnFromPool("Pow", transform.position + Vector3.up);
                    pow.transform.LookAt(cam);
                }
                else{
                    GameObject pam = ObjectPooler.instance.SpawnFromPool("Pam", transform.position + Vector3.up);
                    pam.transform.LookAt(cam);
                }
            }else{
                GameObject kapow = ObjectPooler.instance.SpawnFromPool("Kapow", transform.position);
                kapow.transform.LookAt(Camera.main.transform);
                charAnim.SetTrigger("Die");
                StopMovement();
                ReturnToMenu(GameObject.Find("Enemy").GetComponent<CharacterIdController>().CharacterId, gameObject.GetComponent<CharacterIdController>().CharacterId);
                
            }
        }
    }

    void OnDrawGizmos(){
        if(groundCheck != null){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    public void ShowLives()
    {
        playerHPbar.SetActive(true);
        enemyHPbar.SetActive(true);
        playerHPbarLabel.SetActive(true);
        enemyHPbarLabel.SetActive(true);

        StartCoroutine(AudioManager.instance.FadeOut("transicion", 1));
        StartCoroutine(AudioManager.instance.FadeIn("04_espadas", 1));
    }

    void ReturnToMenu(int winner, int looser)
    {
        if (!scoreDistributed)
        {
            //AudioManager.instance.PlayOneShot("SFX_booring");

            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Swords,
                winner,
                null));

            int newWinner1 = winner;
            int i = 0;
            while (i < 100)
            {
                newWinner1 = Random.Range(0, 6);
                i++;
                if (newWinner1 != winner && newWinner1 != looser)
                {
                    break;
                }
            }

            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Swords,
                newWinner1,
                null));

            int newWinner2;
            newWinner2 = winner;
            i = 0;
            while (i < 100)
            {
                newWinner2 = Random.Range(0, 6);
                i++;
                if (newWinner2 != winner && newWinner2 != looser && newWinner2 != newWinner1)
                {
                    break;
                }
            }

            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Swords,
                newWinner2,
                scenesTransition));
            scoreDistributed = true;
        }
    }
}

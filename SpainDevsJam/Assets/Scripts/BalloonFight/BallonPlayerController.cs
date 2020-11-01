using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallonPlayerController : MonoBehaviour
{
    [SerializeField] ScenesTransitionController scenesTransition;
    [SerializeField] GameTutorialControler tutorial;
    [SerializeField] LayerMask whatIsGround = -1;
    Camera cam;

    [SerializeField] float speed = 6f;
    [SerializeField] float splashSpeed = 4f;
    [SerializeField] float gravityScale = 9.8f;
    [SerializeField] Transform feetPos = default;
    [SerializeField] float feetRadius = .3f;
    float currentSpeed;
    Vector3 velocity;
    bool isGrounded;

    bool moving;
    CharacterController controller;

    [SerializeField] Animator anim = default;
    [SerializeField] float turnSmoothTime = .1f;
    float turnSmoothVelocity;
    bool canShootAgain;
    bool needABalloon;
    bool charging;
    bool shooting;
    bool isDead;

    float yVelocity;
    float velocityChangeVelocity;

    [SerializeField] Transform shootPos = default;

    [SerializeField, Range(0f, 1f)] float timeToShoot = .5f;
    float currentTime;

    [Space]
    [SerializeField, Range(0, 30)] float onSplashDecreaseSpeed = .5f;
    [SerializeField] float maxHp = 100;
    public float hp;
    [SerializeField] GameObject balloon = default;

    [SerializeField] Color splashColor = Color.red;
    [System.Serializable] public class BodyMaterials{
        public Material[] bodyMats;
    }

    [SerializeField] BodyMaterials[] characterMats;
    CharacterIdController character;

    [Space]
    [SerializeField] Canvas canvas = default;
    [SerializeField] UnityEngine.UI.Image chargingBar = default;

    bool scoreDistributed;

    private bool boolSoundActived = false;

    [SerializeField, Range(0, 1f)] float stepsInterval = .35f;
    float currentStepTime;

    // Start is called before the first frame update
    void Awake(){
        character = GetComponent<CharacterIdController>();
        hp = maxHp;
        currentSpeed = speed;
        moving = true;
        cam = Camera.main;
        controller = GetComponent<CharacterController>();

        anim = character.SetPlayerCustom();

        foreach (Material bodyMat in characterMats[character.characterId].bodyMats)
            bodyMat.SetColor("Color_9AC6EC04", splashColor);

        foreach(BodyMaterials body in characterMats)
            foreach (Material bodyMat in body.bodyMats)
                bodyMat.SetFloat("Vector1_104BE5AC", Mathf.Lerp(-1, 1.5f, 1 - hp / maxHp));

        StartCoroutine(AudioManager.instance.FadeIn("transicion", 1));
    }

    // Update is called once per frame
    void Update()
    {

        if (tutorial.gameStarted && !isDead)
        {
            if (!boolSoundActived)
            {
                boolSoundActived = true;
                StartCoroutine(AudioManager.instance.FadeOut("transicion", 1));
                StartCoroutine(AudioManager.instance.FadeIn("05_globos", 1));
            }

            balloon.SetActive(!needABalloon);

            #region Shoot
            if (Input.GetMouseButtonUp(0) && currentTime >= timeToShoot && !needABalloon && !charging && canShootAgain)
            {
                Shoot();
                canShootAgain = false;
            }

            if (Input.GetMouseButton(0) && canShootAgain && !needABalloon)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, whatIsGround))
                {
                    Vector3 pos = hit.point - transform.position;
                    float rotY = Mathf.Atan2(-pos.z, pos.x) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotY + 90, ref turnSmoothVelocity, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }

                moving = false;
                currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, timeToShoot);
            }
            else
            {
                if (!charging)
                {
                    currentTime = Mathf.Clamp(currentTime - Time.deltaTime * 2, 0, timeToShoot);
                    if (currentTime <= 0)
                    {
                        canShootAgain = true;
                    }
                }
                moving = true;
            }

            chargingBar.transform.localScale = new Vector3(Mathf.Clamp(currentTime / timeToShoot, 0, 1), 1);
            #endregion

            isGrounded = Physics.CheckSphere(feetPos.position, feetRadius, whatIsGround);

            if (moving && !shooting)
            {
                Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

                if (direction.magnitude > .1f)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    controller.Move(direction * currentSpeed * Time.deltaTime);
                    yVelocity = 1;

                    currentStepTime += Time.deltaTime;
                    if (currentStepTime >= stepsInterval){
                        AudioManager.instance.PlayOneShot("SFX_paso" + Random.Range(1, 6), true);
                        currentStepTime = 0;
                    }
                }
                else
                {
                    yVelocity = 0;
                }

                if(hp <= 0 && !isDead){
                    anim.SetTrigger("Die");
                    isDead = true;
                    ReturnToMenu(GameObject.Find("Enemy").GetComponent<CharacterIdController>().characterId, GetComponent<CharacterIdController>().characterId);
                }
            }
            else
            {
                yVelocity = 0;
            }

            anim.SetFloat("VelocityY", Mathf.SmoothDamp(anim.GetFloat("VelocityY"), yVelocity, ref velocityChangeVelocity, .1f));

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            velocity.y -= gravityScale * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            foreach (Material bodyMat in characterMats[character.characterId].bodyMats)
                bodyMat.SetFloat("Vector1_104BE5AC", Mathf.Lerp(-1, 1.5f, 1 - hp / maxHp));
        }
    }

    void LateUpdate(){
        canvas.transform.rotation = Quaternion.Euler(45, 0, 0);
    }

    void Shoot(){
        anim.SetTrigger("Throw");
        needABalloon = true;
        shooting = true;
        StartCoroutine(ShootBalloon());
    }

    IEnumerator ShootBalloon(){
        yield return new WaitForSeconds(.5f);
        ObjectPooler.instance.SpawnFromPool("OrangeBalloon", shootPos.position, shootPos.rotation);
        shooting = false;
    }

    void OnTriggerStay(Collider col){
        if (col.CompareTag("BlueSplash")){
            currentSpeed = splashSpeed;
            hp -= onSplashDecreaseSpeed * Time.deltaTime;
        }

        if (col.CompareTag("BalloonTrigger")){
            if (Input.GetMouseButtonUp(0) && currentTime >= timeToShoot && needABalloon){
                anim.SetTrigger("Lift");
                currentTime = 0;
                needABalloon = false;
                canShootAgain = false;
                charging = false;
            }

            if (Input.GetMouseButton(0) && canShootAgain && needABalloon){
                charging = true;
                moving = false;
                currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, timeToShoot);
            }
        }
    }

    void OnTriggerExit(Collider col){
        if (col.CompareTag("BlueSplash")){
            currentSpeed = speed;
        }

        if (col.CompareTag("BalloonTrigger")){
            if(currentTime >= timeToShoot && needABalloon){
                needABalloon = false;
                canShootAgain = false;
                charging = false;
            }else if (needABalloon){
                currentTime = 0;
            }
        }
    }

    void ReturnToMenu(int winner, int looser)
    {
        StartCoroutine(AudioManager.instance.FadeOut("05_globos", 1));
        if (!scoreDistributed)
        {
            StartCoroutine(BetweenScenePass.instance.DistributeScore(
                BetweenScenePass.Games.Baloons,
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
                BetweenScenePass.Games.Baloons,
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
                BetweenScenePass.Games.Baloons,
                newWinner2,
                scenesTransition,
                1.5f));
            scoreDistributed = true;
        }
    }
}

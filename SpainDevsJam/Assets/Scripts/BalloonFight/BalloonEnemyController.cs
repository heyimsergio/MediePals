using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BalloonEnemyController : MonoBehaviour
{
    [SerializeField] ScenesTransitionController scenesTransition;
    [SerializeField] GameTutorialControler tutorial;

    [SerializeField] Animator anim = default;
    [SerializeField] Transform player = default;
    [SerializeField] Transform basketPos = default;
    NavMeshAgent agent;

    [Space]
    [SerializeField] Transform shootPos = default;
    [SerializeField] Vector2 xPos = new Vector2(1, 10);
    float maxXPos;
    [SerializeField] float speed = 6f;
    [SerializeField] float splashSpeed = 3f;
    [SerializeField] Vector2 timeToShoot = new Vector2(.25f, 2f);
    float currentSpeed;
    float rotationVelocity;
    [SerializeField, Range(0, .3f)] float turnSmoothTime = .1f;
    float timeSelected;
    float currentTime;
    bool canMove;
    bool needBalloons;

    [SerializeField, Range(0, 10)] float onSplashDecreaseSpeed = .5f;
    [SerializeField] float maxHp = 100;
    [SerializeField] float hp;

    [SerializeField] Color splashColor = Color.red;
    [System.Serializable]
    public class BodyMaterials{
        public Material[] bodyMats;
    }

    [SerializeField] BodyMaterials[] characterMats;
    CharacterIdController character;

    [Space]
    [SerializeField] Canvas canvas = default;
    [SerializeField] UnityEngine.UI.Image chargingBar = default;
    [SerializeField] GameObject balloon = default;

    bool isDead;
    bool scoreDistributed;

    [SerializeField, Range(0, 1f)] float stepsInterval = .35f;
    float currentStepTime;

    // Start is called before the first frame update
    void Awake(){
        character = GetComponent<CharacterIdController>();
        agent = GetComponent<NavMeshAgent>();
        currentSpeed = speed;
        canMove = true;
        maxXPos = Random.Range(xPos.x, xPos.y);
        anim = character.SetRandomNpcCustom();

        hp = maxHp;

        foreach (Material bodyMat in characterMats[character.characterId].bodyMats)
            bodyMat.SetColor("Color_9AC6EC04", splashColor);

        foreach (BodyMaterials body in characterMats)
            foreach (Material bodyMat in body.bodyMats)
                bodyMat.SetFloat("Vector1_104BE5AC", Mathf.Lerp(-1, 1.5f, 1 - hp / maxHp));
    }

    // Update is called once per frame
    void Update(){
        if (tutorial.gameStarted) {
            balloon.SetActive(!needBalloons);

            if(canMove && !needBalloons){
                agent.SetDestination(new Vector3(maxXPos, 0, player.position.z));
                anim.SetFloat("VelocityY", 1);

                currentStepTime += Time.deltaTime;
                if (currentStepTime >= stepsInterval){
                    AudioManager.instance.PlayOneShot("SFX_paso" + Random.Range(1, 6), true);
                    currentStepTime = 0;
                }
            }
            else if (needBalloons){
                anim.SetFloat("VelocityY", 1);
                agent.SetDestination(new Vector3(10, 0, basketPos.position.z));

                currentStepTime += Time.deltaTime;
                if (currentStepTime >= stepsInterval){
                    AudioManager.instance.PlayOneShot("SFX_paso" + Random.Range(1, 6), true);
                    currentStepTime = 0;
                }
            }

            if ((new Vector3(maxXPos, transform.position.y, player.position.z) - transform.position).sqrMagnitude < agent.stoppingDistance * agent.stoppingDistance && !needBalloons || canMove == false){
                anim.SetFloat("VelocityY", 0);
                Vector3 direction = new Vector3(player.position.x - transform.position.x, transform.position.y, player.position.z - transform.position.z).normalized;
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, turnSmoothTime);

                transform.rotation = Quaternion.Euler(0, angle, 0);

                if(Mathf.Abs(transform.eulerAngles.y - angle) < .5f){
                    if(canMove == false){
                        currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, timeSelected);
                        chargingBar.transform.localScale = new Vector3(Mathf.Clamp(currentTime / timeSelected, 0, 1), 1);
                        if (currentTime >= timeSelected && !shooting){
                            StartCoroutine(ShootDelay());
                        }
                    }else{
                        timeSelected = Random.Range(timeToShoot.x, timeToShoot.y);
                        currentTime = 0;
                        chargingBar.transform.localScale = new Vector3(0, 1, 1);
                        canMove = false;
                        maxXPos = Random.Range(xPos.x, xPos.y);
                    }
                }
            }

            if ((new Vector3(10, transform.position.y, basketPos.position.z) - transform.position).sqrMagnitude < agent.stoppingDistance * agent.stoppingDistance && needBalloons && !gettingABalloon){
                StartCoroutine(GetABalloon());
                anim.SetFloat("VelocityY", 0);
            }

            agent.speed = currentSpeed;

            foreach (Material bodyMat in characterMats[character.characterId].bodyMats)
                bodyMat.SetFloat("Vector1_104BE5AC", Mathf.Lerp(-1, 1.5f, 1 - hp / maxHp));

            if (hp <= 0 && !isDead)
            {
                anim.SetTrigger("Die");
                isDead = true;
                ReturnToMenu(GameObject.Find("Player").GetComponent<CharacterIdController>().characterId, GetComponent<CharacterIdController>().characterId);
            }
        }
    }

    void LateUpdate(){
        canvas.transform.rotation = Quaternion.Euler(45, 0, 0);
    }

    bool gettingABalloon;
    IEnumerator GetABalloon(){
        gettingABalloon = true;
        anim.SetTrigger("Lift");
        yield return new WaitForSeconds(.5f);
        needBalloons = false;
        gettingABalloon = false;
        StopCoroutine(GetABalloon());
    }

    bool shooting;

    IEnumerator ShootDelay(){
        anim.SetTrigger("Throw");
        shooting = true;
        yield return new WaitForSeconds(.7f);
        chargingBar.transform.localScale = new Vector3(0, 1, 1);
        Shoot();
        canMove = true;
        shooting = false;
        StopAllCoroutines();
    }

    void Shoot(){
        needBalloons = true;
        ObjectPooler.instance.SpawnFromPool("BlueBalloon", shootPos.position, shootPos.rotation);
    }

    void OnTriggerStay(Collider col){
        if (col.CompareTag("OrangeSplash")){
            hp -= onSplashDecreaseSpeed * Time.deltaTime;
            currentSpeed = splashSpeed;
        }
    }

    void OnTriggerExit(Collider col){
        if (col.CompareTag("OrangeSplash")){
            currentSpeed = speed;
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

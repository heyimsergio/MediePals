using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SwordsEnemyController : MonoBehaviour
{
    [SerializeField] ScenesTransitionController scenesTransition = default;
    [SerializeField] GameTutorialControler tutorial = default;
    [SerializeField] Transform player = default;
    NavMeshAgent agent;
    [SerializeField] Animator charAnim = default;

    [System.Serializable] public enum Personality {Shy, Hyperactive, Balanced}
    [Space]
    public Personality personality;

    [SerializeField] float maxHp = 100;
    public float hp;

    [SerializeField] float currentRadius = 10;
    [SerializeField, Range(0, 3)] float angryLevel;
    float angryLevelIncreaseTime;
    [SerializeField] float healthAlert;

    [Space]
    [SerializeField] SwordCharacterType shyStats = default;
    [SerializeField] SwordCharacterType angryStats = default;
    [SerializeField] SwordCharacterType balancedStats = default;

    [Space]
    [SerializeField] Image healthBar = default;

    [SerializeField, Range(0, 1f)] float stepsInterval = .5f;
    float currentTime;

    bool scoreDistributed;

    // Start is called before the first frame update
    void Awake(){

        charAnim = charAnim = GetComponent<CharacterIdController>().SetRandomNpcCustom();


        agent = GetComponent<NavMeshAgent>();
        switch (personality){
            case Personality.Balanced:
                currentRadius = balancedStats.detectArea;
                healthAlert = balancedStats.healthAlert;
                angryLevelIncreaseTime = balancedStats.angryIncreaseTime;
                agent.speed = balancedStats.speed;
                break;
            case Personality.Hyperactive:
                currentRadius = angryStats.detectArea;
                healthAlert = angryStats.healthAlert;
                angryLevelIncreaseTime = angryStats.angryIncreaseTime;
                agent.speed = angryStats.speed;
                break;
            case Personality.Shy:
                currentRadius = shyStats.detectArea;
                healthAlert = shyStats.healthAlert;
                angryLevelIncreaseTime = shyStats.angryIncreaseTime;
                agent.speed = shyStats.speed;
                break;
        }

        hp = maxHp;
    }

    // Update is called once per frame
    void Update(){
        if (tutorial.gameStarted)
        {
            agent.SetDestination(new Vector3(player.position.x, 0, player.position.z));

            if ((player.position - transform.position).sqrMagnitude < agent.stoppingDistance * agent.stoppingDistance)
            {
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            }

            charAnim.SetFloat("VelocityY", 1);

            currentTime += Time.deltaTime;
            if (currentTime >= stepsInterval)
            {
                AudioManager.instance.PlayOneShot("SFX_paso" + Random.Range(1, 6), true);
                currentTime = 0;
            }

            angryLevel += angryLevelIncreaseTime * Time.deltaTime;
            angryLevel = Mathf.Clamp(angryLevel, 0, 3);
        }
    }

    public void Damage(float amount){
        if (hp > 0){
            hp -= amount;
            healthBar.transform.localScale = new Vector3(Mathf.Clamp(hp / maxHp, 0, 1), 1, 1);

            ScreenShakeCall.instance.ShakeCamera(12.5f, .4f);

            if (hp > 0f){
                float randomAnim = Random.Range(0, 200);
                if (randomAnim > 100){
                    GameObject pow = ObjectPooler.instance.SpawnFromPool("Pow", transform.position + Vector3.up);
                    pow.transform.LookAt(Camera.main.transform);
                }else{
                    GameObject pam = ObjectPooler.instance.SpawnFromPool("Pam", transform.position + Vector3.up);
                    pam.transform.LookAt(Camera.main.transform);
                }
                charAnim.SetTrigger("Damage");
            }else{
                GameObject kapow = ObjectPooler.instance.SpawnFromPool("Kapow", transform.position);
                kapow.transform.LookAt(Camera.main.transform);
                charAnim.SetTrigger("Die");
                ReturnToMenu(GameObject.Find("Player").GetComponent<CharacterIdController>().CharacterId, gameObject.GetComponent<CharacterIdController>().CharacterId);
            }
        }
    }

    void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position, currentRadius);
    }

    void ReturnToMenu(int winner, int looser)
    {
        if (!scoreDistributed)
        {
            AudioManager.instance.PlayOneShot("SFX_campanillas");

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

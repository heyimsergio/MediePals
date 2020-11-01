using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChairsEnemyController : MonoBehaviour
{
    [SerializeField] public Animator anim;

    [SerializeField] const float normalSpeed = 6f;
    [SerializeField] const float stunnedSpeed = 3f;
    //float speed = 6;
    
    [SerializeField] NavMeshAgent agent;
    [SerializeField] ChairsGameController gameController;

    [SerializeField] ChairsChairController targetChair;
    
    //[SerializeField] float gravity = -9.8f;

    [SerializeField] float findNewChairDelay;

    [SerializeField] float stunCoolDown;
    float actualStunCoolDown;
    [SerializeField] float stunDuration;

    public bool sat = false;

    /*[HideInInspector]*/ public Vector3 startPosition;

    //bool canMove = true;
    public bool stunned = false;

    public bool paused = false;

    void Start()
    {
        startPosition = this.transform.position;
        
    }

    public void InitGame()
    {
        SetInitialTargetChair();
        agent.SetDestination(targetChair.gameObject.transform.position);
        SetNewTargetChair();
    }

    public void SetInitialTargetChair()
    {
        float maxDistance = float.PositiveInfinity;
        int selectedIndex = 0;
        for (int i = 0; i < gameController.freeChairs.Count; i++)
        {
            if (!gameController.freeChairs[i].occupied)
            {
                float distance = Vector3.Distance(this.transform.position, gameController.freeChairs[i].gameObject.transform.position);
                if (distance < maxDistance)
                {
                    selectedIndex = i;
                    maxDistance = distance;
                }
            }
        }
        targetChair = gameController.freeChairs[selectedIndex];
    }

    public void SetNewTargetChair()
    {
        StartCoroutine(SetNewTargetChairCoroutine());
    }

    public IEnumerator SetNewTargetChairCoroutine()
    {
        while (!sat && gameController.freeChairs.Count> 0)
        {
            if (!this.stunned)
            {
                float maxDistance = float.PositiveInfinity;
                int selectedIndex = 0;
                for (int i = 0; i < gameController.freeChairs.Count; i++)
                {
                    if (!gameController.freeChairs[i].occupied)
                    {
                        NavMeshPath path = new NavMeshPath();
                        agent.CalculatePath(gameController.freeChairs[i].transform.position, path);
                        float distance = GetPathLength(path);/*= Vector3.Distance(this.transform.position, gameController.freeChairs[i].gameObject.transform.position)*/
                        
                        if (distance < maxDistance)
                        {
                            selectedIndex = i;
                            maxDistance = distance;
                        }
                    }
                }
                if (targetChair == null)
                {
                    ChairsChairController chair = gameController.freeChairs[selectedIndex];
                    targetChair = chair;
                    if (!this.stunned)
                    {
                        agent.SetDestination(chair.gameObject.transform.position);
                        Debug.Log("Se setea nuevo destino por silla null");
                    }
                } else if (!targetChair.Equals(gameController.freeChairs[selectedIndex]))
                {
                    ChairsChairController chair = gameController.freeChairs[selectedIndex];
                    targetChair = chair;
                    if (!this.stunned)
                    {
                        agent.SetDestination(chair.gameObject.transform.position);
                        Debug.Log("Se setea nuevo destino por nueva silla mas cercana");
                    }
                }
            }
            yield return new WaitForSecondsRealtime(findNewChairDelay);
        }
    }

    private float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }

    // Update is called once per frame
    void Update()
    {
        if (actualStunCoolDown != 0)
        {
            actualStunCoolDown -= Time.deltaTime;
            if (actualStunCoolDown < 0)
            {
                actualStunCoolDown = 0;
            }
        }
        if (stunned || paused)
        {
            agent.SetDestination(this.transform.position);
        } else
        {
            anim.SetFloat("VelocityX", agent.velocity.x);
            anim.SetFloat("VelocityY", agent.velocity.z);
        }
        /*
        if (targetChair != null && !stunned && canMove)
        {
            var targetRotation = Quaternion.LookRotation(targetChair.position - transform.position);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime);

            Vector3 targetPos = new Vector3(targetChair.position.x, 0f, targetChair.position.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }else if (stunned)
        {
            if (moveLeft)
            {
                controller.Move(Vector3.left * Time.deltaTime);
            }

            if (moveRight)
            {
                controller.Move(Vector3.right * Time.deltaTime);
            }

            StartCoroutine(StopStunned());
        }
        */
        

    }

    public void ChooseNewChair()
    {
        targetChair = null;
        for (int i = 0; i < gameController.freeChairs.Count; i++)
        {
            if (!gameController.freeChairs[i].GetComponent<ChairsChairController>().occupied)
            {
                targetChair = gameController.freeChairs[i];
            }
        }
    }

    public void ResetEnemy()
    {
        //agent.enabled = false;
        agent.Warp(startPosition);
        transform.rotation = new Quaternion(0, 0, 0, 0);
        //agent.enabled = true;
        //enemy.gameObject.SetActive(true);
        //this.sat = false;
        this.stunned = false;
        this.targetChair = null;
        SetNewTargetChair();
    }

    private void ResetAnimation()
    {
        anim.SetFloat("VelocityX", 0);
        anim.SetFloat("VelocityY", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            ChairsEnemyController enemy = other.GetComponent<ChairsEnemyController>();
            DecideStun(enemy);
        }

        else if (other.CompareTag("Player"))
        {
            ChairsPlayerController enemy = other.GetComponent<ChairsPlayerController>();
            DecideStun(enemy);
        }
        
        if (other.CompareTag("Chair"))
        {
            ChairsChairController chair = other.GetComponent<ChairsChairController>();
            if (!chair.occupied)
            {
                this.sat = true;
                chair.AssignSeat();
                ResetAnimation();
                
            }
        }

    }

    private void DecideStun(ChairsEnemyController enemy)
    {
        float decision = Random.Range(0, 3);
        if (decision < 1)
        {
            if (actualStunCoolDown == 0 && !enemy.stunned) { 
                Stun(enemy);
                actualStunCoolDown = stunCoolDown;
            }
        }
    }
    private void DecideStun(ChairsPlayerController enemy)
    {
        float decision = Random.Range(0, 5);
        if (decision < 1)
        {
            if (actualStunCoolDown == 0 && !enemy.stunned)
            {
                Stun(enemy);
                actualStunCoolDown = stunCoolDown;
            }
        }
    }

    private void Stun(ChairsEnemyController enemy)
    {
        enemy.StunMyself();
    }

    private void Stun(ChairsPlayerController enemy)
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
        
        Debug.Log("Me stuneo " + this.gameObject.name);
        agent.isStopped = true;
        ResetAnimation();
        yield return new WaitForSecondsRealtime(stunDuration);
        Debug.Log("Me termino el stuneo " + this.gameObject.name);
        agent.isStopped = false;
        stunned = false;
        this.targetChair = null;
        SetNewTargetChair();
    }
    /*
    public void ResetAnim()
    {
        anim.SetFloat("VelocityX", 0);
        anim.SetFloat("VelocityY", 0);
    }*/

    public void Loose()
    {
        anim.SetTrigger("DieWithoutGettingUp");
    }


}

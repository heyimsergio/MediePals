using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwordEnemyAttack : MonoBehaviour{

    [SerializeField] Animator anim = default;
    [SerializeField] float attackDamage = 20f;
    [SerializeField, Range(0, .5f)] float attackMaxDelay = .1f;
    bool attacking;

    [SerializeField] NavMeshAgent agent;

    [Space]
    [SerializeField] Transform attackPos = default;
    [SerializeField, Range(0, 3)] float attackRange = 2;
    [SerializeField] LayerMask whatIsEnemy = -1;

    // Start is called before the first frame update
    void Update(){
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos.position, attackRange, whatIsEnemy);
        if (hitEnemies.Length > 0 && !attacking)
        {
            if (GetComponentInParent<SwordsEnemyController>().hp > 0f) {
                attacking = true;
                anim.SetTrigger("Attack");
            }
        }

        if (attacking)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    public void Attack(){
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos.position, attackRange, whatIsEnemy);

        if (hitEnemies.Length == 0)
            AudioManager.instance.PlayOneShot("SFX_falloEspada" + Random.Range(1, 5));
        else
            AudioManager.instance.PlayOneShot("SFX_Espada" + Random.Range(1, 3));

        foreach (Collider enemy in hitEnemies){
            enemy.GetComponent<SwordPlayerController>().Damage(attackDamage);
        }
    }

    public void EndAttack(){
        attacking = false;
    }

    void OnDrawGizmos(){
        if (attackPos == null)
            return;

        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}

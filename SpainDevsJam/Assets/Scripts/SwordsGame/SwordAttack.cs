using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour{
    [SerializeField] GameTutorialControler tutorial;
    [SerializeField] float attackDamage = 20f;
    Animator anim;

    [Space]
    [SerializeField] Transform attackPos = default;
    [SerializeField, Range(0, 3)] float attackRange = 2;
    [SerializeField] LayerMask whatIsEnemy = -1;

    SwordPlayerController player;
    bool attacking;

    // Start is called before the first frame update
    void Awake(){
        anim = GetComponent<Animator>();
        player = FindObjectOfType<SwordPlayerController>();
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetMouseButtonDown(0) && !attacking && tutorial.gameStarted)
        {
            anim.SetTrigger("Attack");
            attacking = true;
            player.StopMovement();
        }
    }

    public void Attack(){
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos.position, attackRange, whatIsEnemy);

        if(hitEnemies.Length == 0)
            AudioManager.instance.PlayOneShot("SFX_falloEspada" + Random.Range(1, 5), true);
        else
            AudioManager.instance.PlayOneShot("SFX_Espada" + Random.Range(1, 3), true);

        foreach (Collider enemy in hitEnemies){
            enemy.GetComponent<SwordsEnemyController>().Damage(attackDamage);
        }
    }

    public void EndAttack(){
        attacking = false;
        player.RegainMovement();
    }

    void OnDrawGizmos(){
        if (attackPos == null)
            return;

        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}

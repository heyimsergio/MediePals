using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour, IPooledObject{

    [SerializeField] float shootForce = 10f;
    [SerializeField] string nameOfTheSplash = "OrangeSplash";
    [SerializeField] string enemyTag = "Enemy";
    Rigidbody rb;
    [SerializeField, Range(1, 3)] int jumpsToExplode = 3;
    int currentJump;

    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    public void OnObjectSpawn(){
        currentJump = 1;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col){
        if(col.gameObject.CompareTag("Ground")){
            if(currentJump < jumpsToExplode){
                GameObject splash = ObjectPooler.instance.SpawnFromPool(nameOfTheSplash, new Vector3(transform.position.x, .15f, transform.position.z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
                splash.transform.localScale = new Vector3(1, .15f, 1);
                AudioManager.instance.PlayOneShot("SFX_globoFalla", true);
                currentJump++;
            }else{
                GameObject splash = ObjectPooler.instance.SpawnFromPool(nameOfTheSplash, new Vector3(transform.position.x, .15f, transform.position.z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
                splash.transform.localScale = new Vector3(2, .15f, 2);
                AudioManager.instance.PlayOneShot("SFX_globoImpacta", true);
                gameObject.SetActive(false);
            }
        }

        if (col.gameObject.CompareTag(enemyTag)){
            GameObject splash = ObjectPooler.instance.SpawnFromPool(nameOfTheSplash, new Vector3(transform.position.x, .15f, transform.position.z), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            splash.transform.localScale = new Vector3(2, .15f, 2);
            gameObject.SetActive(false);
        }
    }
}

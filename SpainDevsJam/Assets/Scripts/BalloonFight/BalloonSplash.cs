using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSplash : MonoBehaviour, IPooledObject{

    [SerializeField] float timeToDisappear = 10f;
    [SerializeField] float disappearDuration = 1f;
    float currentTime;

    Animator anim;

    void Awake(){
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    public void OnObjectSpawn(){
        currentTime = 0;
    }

    // Update is called once per frame
    void Update(){
        currentTime += Time.deltaTime;
        if (currentTime >= timeToDisappear - disappearDuration){
            anim.SetTrigger("Destroy");
        }
    }

    public void Disable(){
        gameObject.SetActive(false);
    }
}

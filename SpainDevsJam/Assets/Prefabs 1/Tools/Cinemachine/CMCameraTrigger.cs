using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CMCameraTrigger : MonoBehaviour{

    CinemachineVirtualCamera vcam;

    // Start is called before the first frame update
    void Awake(){
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        vcam.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col){
        if (col.CompareTag("Player")){
            vcam.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D col){
        if (col.CompareTag("Player")){
            vcam.gameObject.SetActive(false);
        }
    }
}

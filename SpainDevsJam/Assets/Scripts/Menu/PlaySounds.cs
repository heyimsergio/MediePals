using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySounds : MonoBehaviour{

    [SerializeField] bool playIntro = true;

    void Start(){
        if (playIntro){
            AudioManager.instance.Play("Intro");
        }
    }

    public void PlayMusic(string name){
        AudioManager.instance.Play(name);
    }

    public void StopMusic(string name){
        AudioManager.instance.Play(name);
    }

    public void PlayOneShot(string name){
        AudioManager.instance.PlayOneShot(name);
    }
}

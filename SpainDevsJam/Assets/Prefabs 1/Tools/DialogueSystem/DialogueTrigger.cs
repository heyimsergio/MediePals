using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour{

    public Dialogue dialogue;

    /// <summary> Start the dialogue specified on the inspector </summary>
    public void TriggerDialogue(){
        FindObjectOfType<DialogueSystem>().StartDialogue(dialogue);
    }
}

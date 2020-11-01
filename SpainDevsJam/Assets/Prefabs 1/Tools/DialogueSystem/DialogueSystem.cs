using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public TMP_Animated dialogue;

    public Queue<string> sentences;

    // Start is called before the first frame update
    void Start(){
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue){
        nameText.text = dialogue.characterName;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences){
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence(){
        if(sentences.Count == 0){
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogue.ReadText(sentence);
    }

    void EndDialogue(){
        Debug.Log("End of conversation.");
    }
}

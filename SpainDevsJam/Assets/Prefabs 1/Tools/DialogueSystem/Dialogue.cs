using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Dialogue : ScriptableObject{

    public string characterName;
    [TextArea] public string[] sentences;
}

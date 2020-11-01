using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class JustaTargetComboObjects : MonoBehaviour
{
    public enum KeyType { W, A, S, D};

    public KeyType key;

    [SerializeField] TextMeshPro text;
    
    public void SetRandomKey()
    {
        int num = UnityEngine.Random.Range(0, Enum.GetValues(typeof(KeyType)).Length);
        switch (num)
        {
            case 0:
                key = KeyType.W;
                text.text = "W";
                break;
            case 1:
                key = KeyType.A;
                text.text = "A";
                break;
            case 2:
                key = KeyType.S;
                text.text = "S";
                break;
            case 3:
                key = KeyType.D;
                text.text = "D";
                break;
            default:
                key = KeyType.W;
                text.text = "W";
                break;
        }
    }

}

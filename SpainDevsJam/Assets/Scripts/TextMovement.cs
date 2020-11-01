using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMovement : MonoBehaviour
{
    enum movementType { ThreeSecondsCountdown };
    [SerializeField] private movementType type;
    [SerializeField] private float velocity;
    [SerializeField] private float desiredValue;

    private float originalFontSize;

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = this.gameObject.GetComponent<TextMeshProUGUI>();
        originalFontSize = text.fontSize;
    }

    private void OnEnable()
    {
        //text = this.gameObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = originalFontSize;
        switch (type)
        {
            case movementType.ThreeSecondsCountdown:
                StartCoroutine(ThreeSecondsCountDown(velocity, desiredValue));
                break;
            default:
                break;
        }
    }

    private IEnumerator ThreeSecondsCountDown(float velocity, float desiredValue)
    {
        float original = text.fontSize;
        float limit = 1.5f;
        for (int a = 3; a > 0; a--) {
            text.fontSize = original;
            text.text = a.ToString();
            float i = 0;
            bool increasing = true;
            while (i < limit)
            {
                if (increasing)
                {
                    text.fontSize += velocity * Time.deltaTime;
                    if (text.fontSize >= desiredValue)
                    {
                        text.fontSize = desiredValue;
                        increasing = false;
                    }
                }
                else
                {
                    text.fontSize -= velocity * Time.deltaTime;
                    if (text.fontSize <= original)
                    {
                        text.fontSize = original;
                        increasing = true;
                    }
                }
                i+= Time.smoothDeltaTime;
                yield return null;
            }
        }
        text.fontSize = original;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustaPlayerController : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float aceleration;
    [SerializeField] float timeOfRunning;
    [SerializeField] float timeOfBracking;

    [SerializeField] float timeBetweenRounds; 

    [SerializeField] public Animator anim;

    Vector3 initialPos;

    [SerializeField] int maxLives;
    int currentLife;

    [SerializeField] float roundPoints = 0;

    [SerializeField] JustaTrigger trigger;
    [SerializeField] float multiplier;
    [SerializeField] float mistakePoints;

    [SerializeField] GameObject feedbackError;
    [SerializeField] GameObject feedbackGreat;
    [SerializeField] GameObject feedbackPerfect;

    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Player"))
            anim = GetComponent<CharacterIdController>().SetPlayerCustom();

        initialPos = this.transform.position;
        currentLife = maxLives;
        //StartRunning();
    }

    public int GetCurrentLives()
    {
        return this.currentLife;
    }

    public float GetRoundPoints()
    {
        return this.roundPoints;
    }

    public void SetRoundPoints(float points)
    {
        this.roundPoints = points;
    }

    public void Mistake()
    {
        //Mal activar
        StartCoroutine(ActivateFeedBack(feedbackError));
        roundPoints -= mistakePoints;
        if (roundPoints < 0)
        {
            roundPoints = 0;
        }
        //Debug.Log("Fallo. Puntos actual: " + roundPoints);
    }

    public void WinPoints(float distance)
    {
        //Max distance = 0.5  -> 0,5 puntos
        //Min distance = 0      -> 2 puntos
        float points = 0;
        if (distance <= 0.2f)
        {
            points = 2;
            //Genial activar
            StartCoroutine(ActivateFeedBack(feedbackPerfect));
        } else if (distance > 0.2f && distance < 0.5f)
        {
            points = 1;
            //Bien activar
            StartCoroutine(ActivateFeedBack(feedbackGreat));
        }
        else
        {
            points = 0.5f;
            //Bien activar
            StartCoroutine(ActivateFeedBack(feedbackGreat));
        }
        roundPoints += points;
        //Debug.Log("Puntos ganados: " + points + " puntos totales: " + roundPoints);
    }

    private IEnumerator ActivateFeedBack(GameObject feedback)
    {
        feedback.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        feedback.SetActive(false);
    }

    public void LooseLife()
    {
        currentLife--;
    }

    public void StartRunning()
    {
        StartCoroutine(StartRunningCoroutine());
    }

    private IEnumerator PasosCaballo(float delay)
    {
        float i = 0;
        while (true && i < 10000)
        {
            i += 0.1f;
            AudioManager.instance.PlayOneShot("SFX_pasosCaballo"+Random.Range(1,5), true);
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    private IEnumerator StartRunningCoroutine()
    {
        float i = 0;
        Coroutine corutina = StartCoroutine(PasosCaballo(0.45f));
        while (i < timeOfRunning)
        {
            i += Time.deltaTime;
            if (speed < maxSpeed)
            {
                speed += Time.deltaTime * aceleration;
            }
            anim.SetFloat("VelocityY", speed);
            this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            yield return null;
        }
        StopCoroutine(corutina);
        corutina = StartCoroutine(PasosCaballo(0.65f));
        i = 0;
        while (i < timeOfBracking)
        {
            i += Time.deltaTime;
            if (speed > 0)
            {
                speed -= Time.deltaTime * aceleration * 2;
            }
            anim.SetFloat("VelocityY", speed);
            this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            yield return null;
        }
        StopCoroutine(corutina);
        yield return new WaitForSecondsRealtime(timeBetweenRounds);
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        this.transform.position = initialPos;
        this.roundPoints = 0;
    }
}

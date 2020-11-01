using UnityEngine;
using TMPro;


public class CharSelCharacterController : MonoBehaviour
{
    [SerializeField] public string charName;
    public string phrase;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;


    [SerializeField] Transform spawnPointLeft;
    [SerializeField] Transform spawnPointRight;
    [SerializeField] Transform spawnPointCenter;

    [SerializeField] TextMeshProUGUI phraseText;
    [SerializeField] GameObject phrasePanel;


    [SerializeField] public Animator anim;

    bool canWalk;
    bool moveLeft;
    bool moveRight;

    bool stopAtCenter;
    bool restoreRotation;


    private void OnEnable()
    {
        canWalk = false;
        moveLeft = false;
        moveRight = false;
        moveRight = false;
        transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (canWalk)
        {

            anim.SetFloat("Dance", 0f);

            Quaternion targetRotation = Quaternion.identity;
            Vector3 targetPosition = Vector3.zero;

            if (moveLeft)
            {
                targetRotation = Quaternion.LookRotation(spawnPointLeft.localPosition - transform.localPosition);
                targetPosition = new Vector3(spawnPointLeft.localPosition.x, 0f, 0f);
            }
            else if (moveRight)
            {
                targetRotation = Quaternion.LookRotation(spawnPointRight.localPosition - transform.localPosition);
                targetPosition = new Vector3(spawnPointRight.localPosition.x, 0f, 0f);
            }

            if (stopAtCenter)
            {
                targetPosition = new Vector3(spawnPointCenter.localPosition.x, 0f, 0f);
            }

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, speed * Time.deltaTime);

            anim.SetFloat("VelocityY", 1f);


            if (stopAtCenter && (Mathf.Round(transform.localPosition.x * 10f) / 10f) == spawnPointCenter.localPosition.x)
            {
                canWalk = false;
                moveLeft = false;
                moveRight = false;
                stopAtCenter = false;
                restoreRotation = true;

                phraseText.text = phrase;
                phrasePanel.GetComponent<Animator>().SetBool("visible", true);
                
                anim.SetFloat("VelocityY", 0f);
                anim.SetFloat("Dance", GetComponent<CharacterIdController>().Dance);
            }
            else if(!stopAtCenter && (transform.localPosition.x == spawnPointRight.localPosition.x || transform.localPosition.x == spawnPointLeft.localPosition.x))
            {
                canWalk = false;
                moveLeft = false;
                moveRight = false;
                gameObject.SetActive(false);

                anim.SetFloat("VelocityY", 0f);
            }

        }

        if (restoreRotation)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 180, 0), rotationSpeed * Time.deltaTime);

            if (transform.localRotation.eulerAngles.y > 160 && transform.localRotation.eulerAngles.y < 200)
            {
                
            }

            if (transform.localRotation == Quaternion.Euler(0, 180, 0))
            {
                restoreRotation = false;
            }
        }
    }


    public void WalkLeft(bool stopCharAtCenter)
    {
        restoreRotation = false;

        canWalk = true;
        moveLeft = true;

        stopAtCenter = stopCharAtCenter;
    }

    public void WalkRight(bool stopCharAtCenter)
    {
        restoreRotation = false;

        canWalk = true;
        moveRight = true;

        stopAtCenter = stopCharAtCenter;
    }

    void StopRestoringRotation()
    {
        restoreRotation = false;
    }
}

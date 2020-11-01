using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairsChairController : MonoBehaviour
{

    public bool occupied;
    public bool assigned;

    [SerializeField] ChairsGameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignSeat()
    {
        this.occupied = true;
        gameController.OccupyChair(this);
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (occupied)
            {
                other.GetComponent<ChairsEnemyController>().ChooseNewChair();
            }
            else
            {
                occupied = true;
            }
        }
    }*/
}

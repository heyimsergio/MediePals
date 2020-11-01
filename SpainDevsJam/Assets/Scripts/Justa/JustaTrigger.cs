using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustaTrigger : MonoBehaviour
{

    [SerializeField] JustaPlayerController player;

    JustaTargetComboObjects targetCollisioning = null;

    [HideInInspector] public bool paused = true;

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            if (Input.anyKeyDown)
            {
                if (!Input.GetKeyDown(KeyCode.Escape))
                {
                    AudioManager.instance.PlayOneShot("SFX_caballoBufido" + UnityEngine.Random.Range(1, 3), true);
                                        
                    if (targetCollisioning == null)
                    {
                        LoosePoints();
                    }
                    else
                    {
                        JustaTargetComboObjects.KeyType key = targetCollisioning.key;
                        switch (key)
                        {
                            case JustaTargetComboObjects.KeyType.W:
                                if (Input.GetKeyDown(KeyCode.W))
                                {
                                    WinPoints(this.transform.position, targetCollisioning.gameObject.transform.position);
                                }
                                else
                                {
                                    LoosePoints();
                                }
                                break;
                            case JustaTargetComboObjects.KeyType.A:
                                if (Input.GetKeyDown(KeyCode.A))
                                {
                                    WinPoints(this.transform.position, targetCollisioning.gameObject.transform.position);
                                }
                                else
                                {
                                    LoosePoints();
                                }
                                break;
                            case JustaTargetComboObjects.KeyType.S:
                                if (Input.GetKeyDown(KeyCode.S))
                                {
                                    WinPoints(this.transform.position, targetCollisioning.gameObject.transform.position);
                                }
                                else
                                {
                                    LoosePoints();
                                }
                                break;
                            case JustaTargetComboObjects.KeyType.D:
                                if (Input.GetKeyDown(KeyCode.D))
                                {
                                    WinPoints(this.transform.position, targetCollisioning.gameObject.transform.position);
                                }
                                else
                                {
                                    LoosePoints();
                                }
                                break;
                        }
                    }
                }
            }
        }

    }

    private void LoosePoints()
    {
        player.Mistake();
    }

    private void WinPoints(Vector3 triggerPos, Vector3 targetPos)
    {
        float distance = Vector3.Distance(triggerPos, targetPos);
        player.WinPoints(distance);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TargetJusta"))
        {
            if (targetCollisioning == null)
            {
                targetCollisioning = other.GetComponent<JustaTargetComboObjects>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TargetJusta"))
        {
            if (targetCollisioning != null)
            {
                targetCollisioning = null;
            }
        }
    }
}

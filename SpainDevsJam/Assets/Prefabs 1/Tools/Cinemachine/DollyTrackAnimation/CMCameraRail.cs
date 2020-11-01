using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class EndCutscene : UnityEvent { }

public class CMCameraRail : MonoBehaviour{

    [SerializeField] CinemachineVirtualCamera dollyTrackCamera = default;
    CinemachineTrackedDolly cameraTrack;

    [Space]
    [SerializeField] Waypoint[] waypoints = default;
    int totalCount;
    int currentCount;

    bool finished;
    bool paused;

    float easedPercentBetweenWaypoints;
    float speed;
    float currentEase;
    float percentBetweenWaypoints;
    float nextPos;
    float lastFieldOfView;

    [Space]
    public EndCutscene onCutsceneEnd;

    void Awake(){
        paused = true;
        lastFieldOfView = dollyTrackCamera.m_Lens.FieldOfView;
        totalCount = waypoints.Length - 1;
        if(dollyTrackCamera != null){
            cameraTrack = dollyTrackCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        }
    }

    // Update is called once per frame
    void Update(){
        if(!finished && !paused){
            Waypoint waypoint = waypoints[currentCount];

            if(currentCount == 0){
                speed = Time.deltaTime / waypoint.timeBetweenWaypoints * waypoint.endWaypoint;
                percentBetweenWaypoints += speed / waypoint.endWaypoint;
                percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
                easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

                nextPos = easedPercentBetweenWaypoints * waypoint.endWaypoint;
            }else{
                speed = Time.deltaTime / waypoint.timeBetweenWaypoints * (waypoint.endWaypoint - waypoints[currentCount - 1].endWaypoint);
                percentBetweenWaypoints += speed / (waypoint.endWaypoint - waypoints[currentCount - 1].endWaypoint);
                percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
                easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

                nextPos = waypoints[currentCount - 1].endWaypoint + easedPercentBetweenWaypoints * (waypoint.endWaypoint - waypoints[currentCount - 1].endWaypoint);
            }

            dollyTrackCamera.m_Lens.FieldOfView = Mathf.Lerp(lastFieldOfView, waypoint.fieldOfView, easedPercentBetweenWaypoints);

            if (cameraTrack.m_PathPosition < waypoint.endWaypoint){
                cameraTrack.m_PathPosition = nextPos;
            }else{
                NextWaypoint();
            }
        }
    }

    float Ease(float x){
        if (waypoints[currentCount].ease){
            float a = currentEase + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }else{
            return x;
        }
    }

    /// <summary> Starts playing the cutscene. </summary>
    public void StartRail(){
        paused = false;
        lastFieldOfView = dollyTrackCamera.m_Lens.FieldOfView;
        Waypoint waypoint = waypoints[currentCount];
        if (waypoint.startDelay >= 0)
        {
            paused = true;
            StartCoroutine(DelayMovement(waypoint.startDelay));
        }
        currentEase = waypoint.ease ? waypoint.easeAmount : 1;
    }

    void NextWaypoint(){
        lastFieldOfView = dollyTrackCamera.m_Lens.FieldOfView;
        if (currentCount >= totalCount){
            Debug.Log("Finish");
            finished = true;

            if (onCutsceneEnd.GetPersistentEventCount() > 0)
            {
                onCutsceneEnd.Invoke();
            }
        }
        else{
            percentBetweenWaypoints = 0;
            currentCount++;
            Waypoint waypoint = waypoints[currentCount];
            if(waypoint.startDelay > 0){
                paused = true;
                StartCoroutine(DelayMovement(waypoint.startDelay));
            }
            currentEase = waypoint.ease ? waypoint.easeAmount : 1;
        }
    }

    IEnumerator DelayMovement(float delay){
        yield return new WaitForSeconds(delay);
        paused = false;
    }
}

using UnityEngine;

[System.Serializable]
public class Waypoint{

    [Min(0)] public float startDelay;

    [Header("Easing")]
    public bool ease;
    [Range(0f, 2f)] public float easeAmount;

    [Header("Waypoints")]
    [Min(0)] public float timeBetweenWaypoints;
    [Min(0)] public int endWaypoint;

    [Header("Field of View")]
    [Range(1, 179)] public float fieldOfView = 60;
}

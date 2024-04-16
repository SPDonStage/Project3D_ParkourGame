using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Parkour Menu/Create New Parkour Action")]
public class NewParkourAction : ScriptableObject
{
    [Header("---Check Height Obstacle---")]
    [SerializeField] string animationName;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] bool isLookAtObstacle = true; //optional
    private Quaternion rotatingToObstacle;
    public float checkHeight;
    [Header("---Target Matching---")]
    [SerializeField] bool isMatching = true; //optional
    Vector3 matchPosition = Vector3.zero;
    [SerializeField] AvatarTarget avatarTarget;
    [SerializeField] float startTimeMatching;
    [SerializeField] float endTimeMatching;
    public bool checkIfAvailable(EnvironmentChecker.ObjectData objectData, Transform player)
    {
        checkHeight = objectData.Downward_Ray_Hit.point.y + player.position.y < 0 ? -player.position.y : + player.position.y ;
        if (objectData.Downward_Ray_Hit.point.y == 0) 
            return false;
        if (checkHeight < minHeight || checkHeight > maxHeight)
            return false;

        if (isLookAtObstacle)
        {
            rotatingToObstacle = Quaternion.LookRotation(-objectData.Yoffset_Ray_Hit.normal);
        }
        if (isMatching)
        {
            matchPosition = objectData.Yoffset_Ray_Hit.point;
        }
        return true;
    }

    public string AnimationName => animationName;
    public Quaternion RotatingToObstacle => rotatingToObstacle;
    public bool IsLookAtObstacle => isLookAtObstacle;
    public bool IsMatching => isMatching;
    public Vector3 MatchPosition => matchPosition;  
    public AvatarTarget AvatarTarget => avatarTarget;
    public float StartTimeMatching => startTimeMatching;
    public float EndTimeMatching => endTimeMatching;
}

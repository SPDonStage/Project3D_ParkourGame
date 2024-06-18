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
    //  [SerializeField] bool isLookAtObstacle = true; //optional
    // private Quaternion rotatingToObstacle;
    [SerializeField] bool isUseSpineHeight;
    public float checkHeight;
    [Header("---Target Matching---")]
    [SerializeField] bool isMatching = true; //optional
    Vector3 matchPosition = Vector3.zero;
    [SerializeField] AvatarTarget avatarTarget;
    [SerializeField] float startTimeMatching;
    [SerializeField] float endTimeMatching;
    [SerializeField] private Vector3 positionHeight;
    public bool checkIfAvailable(EnvironmentChecker.ObjectData objectData, Transform player, Transform spineHeight)
    {
        //      Debug.Log("ray:" + objectData.Downward_Ray_Hit.point.y);
        //    Debug.Log("player:" + player.position.y); Debug.Log("height:" + checkHeight);
        // checkHeight = player.position.y < 0 ? Mathf.Abs(objectData.Downward_Ray_Hit.point.y + player.position.y) : objectData.Downward_Ray_Hit.point.y - player.position.y ;Debug.Log("height:"+checkHeight);
        if (isUseSpineHeight)
        {
            checkHeight = player.position.y < 0 ? Mathf.Abs(objectData.Downward_Ray_Hit.point.y - spineHeight.position.y)
                : Mathf.Abs(-objectData.Downward_Ray_Hit.point.y + spineHeight.position.y);
        }
        else
        {
            checkHeight = player.position.y < 0 ? Mathf.Abs(objectData.Downward_Ray_Hit.point.y - player.position.y)
                : Mathf.Abs(-objectData.Downward_Ray_Hit.point.y + player.position.y);
        }
        if (objectData.Downward_Ray_Hit.point.y == 0) 
            return false;
        if (checkHeight < minHeight || checkHeight > maxHeight)
            return false;

     //   if (isLookAtObstacle)
     //   {
  //          rotatingToObstacle = Quaternion.LookRotation(objectData.rotationToRotate);
      //  }
        if (isMatching)
        {
            matchPosition = objectData.Downward_Ray_Hit.point;
        }
        return true;
    }

    public string AnimationName => animationName;
  //  public Quaternion RotatingToObstacle => rotatingToObstacle;
   // public bool IsLookAtObstacle => isLookAtObstacle;
    public bool IsMatching => isMatching;
    public Vector3 MatchPosition => matchPosition;  
    public AvatarTarget AvatarTarget => avatarTarget;
    public float StartTimeMatching => startTimeMatching;
    public float EndTimeMatching => endTimeMatching;
    public  Vector3 PositionHeight => positionHeight;
}

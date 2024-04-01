using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Parkour Menu/Create New Parkour Action")]
public class NewParkourAction : ScriptableObject
{
    [SerializeField] string animationName;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    public bool checkIfAvailable(EnvironmentChecker.ObjectData objectData, Transform player)
    {
        float checkHeight = objectData.Downward_Ray_Hit.point.y - player.position.y;
        if (checkHeight < minHeight || checkHeight > maxHeight) 
            return false;
        
        return true;
    }

    public string AnimationName => animationName;
}

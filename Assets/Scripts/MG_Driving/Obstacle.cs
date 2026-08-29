
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleType _obstacleType;

    public ObstacleType GetObstacleType()
    {
        return _obstacleType;
    }
}

public enum ObstacleType
{
    CONE, CAR, TRUCK, RED_CAR, CONSTRUCTION_BARRIER, POTHOLE, TREE, FALLEN_TREE, ROCK, OTHERS
}
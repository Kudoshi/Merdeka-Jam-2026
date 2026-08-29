
using System.Collections;
using UnityEngine;

public class ItemCarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _carPf;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Vector2 _timeSpawn;
    [SerializeField] private float _laneClearTime = 2;
    [SerializeField] private float _laneClearTimeMultiplier = 2.5f;
    [SerializeField] private float _minSpawnTimeMultiplier = 0.3f;
    [SerializeField] private float _maxSpawnTimeMultiplier = 0.75f;

    private int _clearLane;
    private float _nextClearLane;
    private void Start()
    {
        Util.WaitNextFrame(this, () =>
        {
            StartCoroutine(StartSpawner());

        });
    }

    private void Update()
    {
        if (Time.time >= _nextClearLane)
        {

            float speed = Mathf.Clamp01(CarDrivingSpeed.CurrentSpeed / CarDrivingSpeed.MaxSpeed);
            float multiplier = Mathf.Lerp(_maxSpawnTimeMultiplier, _minSpawnTimeMultiplier, speed);

            float clearTime = _laneClearTime * multiplier * _laneClearTimeMultiplier;

            _nextClearLane = Time.time + clearTime;

            int clearLane = UnityEngine.Random.Range(0, _spawnPoints.Length);
            _clearLane = clearLane;
        }
    }

    private IEnumerator StartSpawner()
    {
        int nextCarDecision = UnityEngine.Random.Range(0, _carPf.Length);

        while (true)
        {
            if (nextCarDecision == 0) // Lorry
                yield return new WaitForSeconds(0.1f);
            int lane;
            
            // Get lane
            while (true)
            {
                lane = UnityEngine.Random.Range(0, _spawnPoints.Length);
                if (lane != _clearLane) break;
            }

            GameObject car = _carPf[nextCarDecision];
            Instantiate(car, _spawnPoints[lane].position, _spawnPoints[lane].rotation);



            float speed = Mathf.Clamp01(CarDrivingSpeed.CurrentSpeed / CarDrivingSpeed.MaxSpeed);
            float multiplier = Mathf.Lerp(_maxSpawnTimeMultiplier, _minSpawnTimeMultiplier, speed);

            Vector2 timeSpawn = _timeSpawn * multiplier;
            float spawnDuration = UnityEngine.Random.Range(timeSpawn.x, timeSpawn.y);

            nextCarDecision = UnityEngine.Random.Range(0, _carPf.Length);

            if (nextCarDecision == 0) // Lorry
                yield return new WaitForSeconds(0.1f);


            yield return new WaitForSeconds(spawnDuration);
        }
    }

}

public enum DrivingItemType
{
    CAR, ROAD, POTHOLE
}
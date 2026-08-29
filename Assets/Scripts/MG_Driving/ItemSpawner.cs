
using System;
using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _objectPfs;
    [SerializeField] private Vector2 _spawnTime;

    private void Start()
    {
        Util.WaitNextFrame(this, () =>
        {
            StartCoroutine(StartSpawner());

        });
    }

    private IEnumerator StartSpawner()
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(_spawnTime.x, _spawnTime.y);

            yield return new WaitForSeconds(waitTime);

            int itemDecision = UnityEngine.Random.Range(0, _objectPfs.Length);

            Instantiate(_objectPfs[itemDecision], transform.position, transform.rotation);
        }
    }
}
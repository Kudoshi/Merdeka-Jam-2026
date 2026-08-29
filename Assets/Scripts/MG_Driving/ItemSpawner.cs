
using System;
using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _objectPf;
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

            Instantiate(_objectPf, transform.position, transform.rotation);
        }
    }
}
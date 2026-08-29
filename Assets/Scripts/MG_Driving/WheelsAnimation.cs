using UnityEngine;
using DG.Tweening;

public class WheelsAnimation : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;


    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, _speed * Time.deltaTime));
    }
}
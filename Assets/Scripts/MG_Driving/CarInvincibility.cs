using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInvincibility : MonoBehaviour
{
    [SerializeField] private float _flashTime;

    private Dictionary<Material, Color> _materials = new Dictionary<Material, Color>();
    private bool _flash;
    private Coroutine _flashCr;
    private float _stopFlashTime;
    private void Awake()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach(MeshRenderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            foreach(Material mat in materials)
            {
                _materials.Add(mat, mat.color);
                mat.SetColor("_EmissionColor", Color.white * 1.0f);

            }
        }
    }

    private void Update()
    {
        if (!_flash) return;

        if (Time.time >= _stopFlashTime)
        {
            _flash = false;

            foreach (var material in _materials)
            {
                material.Key.color = material.Value;
            }
        }
    }

    public void TriggerInvincibility(float invincibilityTime)
    {
        if (_flashCr != null)
        {
            StopCoroutine(_flashCr);
        }

        _flash = true;
        _flashCr = StartCoroutine(FlashCr());
        _stopFlashTime = Time.time + invincibilityTime;
    }

    private IEnumerator FlashCr()
    {
        while (_flash)
        {
            foreach (Material material in _materials.Keys)
            {
                material.EnableKeyword("_EMISSION");
            }

            yield return new WaitForSeconds(_flashTime);

            foreach (var material in _materials)
            {
                material.Key.DisableKeyword("_EMISSION");
            }

            yield return new WaitForSeconds(_flashTime);

        }

        _flashCr = null;
    }

}
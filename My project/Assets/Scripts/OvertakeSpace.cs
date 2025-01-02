using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvertakeSpace : MonoBehaviour
{
    public bool IsEmpty { get; private set; } = true;

    public Material green;
    public Material red;

    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material = green;

        IsEmpty = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Car"))
            return;

        _renderer.material = red;
        IsEmpty = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Car"))
            return;

        _renderer.material = green;
        IsEmpty = true;
    }
}
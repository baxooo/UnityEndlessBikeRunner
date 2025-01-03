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
        if (_renderer == null)
        {
            Debug.LogError("Renderer component not found! Component Id:" + GetInstanceID());
            return;
        }

        _renderer.material = green;
        IsEmpty = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other || !other.CompareTag("Car"))
            return;

        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        _renderer.material = red;
        IsEmpty = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other || !other.CompareTag("Car"))
            return;

        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        _renderer.material = green;
        IsEmpty = true;
    }
}
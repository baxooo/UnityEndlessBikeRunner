using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnSpace : MonoBehaviour
    {
        public bool IsEmpty { get; private set; } = true;

        private Renderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.materials[0].color = new Color(0, 1, 0, 0.3f);
            IsEmpty = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            _renderer.materials[0].color = new Color(1, 0, 0, 0.3f);
            IsEmpty = false;
        }

        private void OnTriggerExit(Collider other)
        {
            _renderer.materials[0].color = new Color(0, 1, 0, 0.3f);
            IsEmpty = true;
        }
    }
}
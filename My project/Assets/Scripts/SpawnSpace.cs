using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnSpace : MonoBehaviour
    {
        public Material Green;
        public Material Red;
        public bool IsEmpty { get; private set; } = true;

        private Renderer _renderer;

        private void Start()
        {
            while (!TryGetComponent(out _renderer))
            {
                Debug.Log("Waiting for renderer component... Component Id:" + GetInstanceID());
            }
            
            _renderer.material = Green;
            IsEmpty = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            _renderer.material = Red;
            IsEmpty = false;
        }

        private void OnTriggerExit(Collider other)
        {
            _renderer.material = Green;
            IsEmpty = true;
        }
    }
}
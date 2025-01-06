using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnSpace : MonoBehaviour
    {
#if UNITY_EDITOR
        public Material Green;
        public Material Red;
        
        private Renderer _renderer;
#endif
        public bool IsEmpty { get; private set; } = true;


        private void Start()
        {
#if UNITY_EDITOR
            while (!TryGetComponent(out _renderer))
            {
                Debug.Log("Waiting for renderer component... Component Id:" + GetInstanceID());
            }

            _renderer.material = Green;
#endif
            IsEmpty = true;
        }

        private void OnTriggerEnter(Collider other)
        {
#if UNITY_EDITOR
            _renderer.material = Red;
#endif
            IsEmpty = false;
        }

        private void OnTriggerExit(Collider other)
        {
#if UNITY_EDITOR
            _renderer.material = Green;
#endif
            IsEmpty = true;
        }
    }
}
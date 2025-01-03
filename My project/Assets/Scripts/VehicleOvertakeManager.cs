using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class VehicleOvertakeManager : MonoBehaviour
    {
        public GameObject ColliderSquarePrefab;

        public bool IsLeftFree { get; private set; } = true;
        public bool IsRightFree { get; private set; } = true;

        private List<OvertakeSpace> _leftOvertakeSpaces = new();
        private List<OvertakeSpace> _rightOvertakeSpaces = new();

        private void Start()
        {
            InitializeOvertakeSpaces();
        }

        private void Update()
        {
            IsLeftFree = _leftOvertakeSpaces.TrueForAll(space => space.IsEmpty);
            IsRightFree = _rightOvertakeSpaces.TrueForAll(space => space.IsEmpty);
        }

        private void InitializeOvertakeSpaces()
        {
            CreateOvertakeSpaces(_rightOvertakeSpaces, 3.2f, "RightOvertakeSpace");
            CreateOvertakeSpaces(_leftOvertakeSpaces, -3.2f, "LeftOvertakeSpace");
        }

        private void CreateOvertakeSpaces(List<OvertakeSpace> overtakeSpaces, float xOffset, string tag)
        {
            for (var i = 0; i < 10; i++)
            {
                var position = new Vector3(transform.position.x + xOffset, 0.6f, transform.position.z - 8.5f + i * 3);
                var square = Instantiate(ColliderSquarePrefab, position, Quaternion.identity);
                square.transform.SetParent(transform);
                square.tag = tag;
                overtakeSpaces.Add(square.GetComponent<OvertakeSpace>());
            }
        }
    }
}
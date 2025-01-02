using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class VehiclesSpawnGrid : MonoBehaviour
{
    private float[] _possiblePositions = { -4.9f, -1.65f, 1.65f, 4.9f };
    private GameObject _player;

    public GameObject spawnSpace;
    public float baseMinSpawnDistance = 150f;
    public byte rows;

    private readonly byte _columns = 4;

    public List<SpawnSpace> SpawnedSpaces { get; private set; }


    // Start is called before the first frame update
    private void Start()
    {
        _player = GameObject.FindWithTag("Player");

        var initialPosition = new Vector3(0, 0, baseMinSpawnDistance);

        SpawnedSpaces = new List<SpawnSpace>();
        for (var i = 0; i < rows; i++)
        for (var j = 0; j < _columns; j++)
        {
            Vector3 position = new(_possiblePositions[j], 0, initialPosition.z + i * 21);
            var space = Instantiate(spawnSpace, position, Quaternion.identity);

            space.transform.SetParent(transform);

            SpawnedSpaces.Add(space.GetComponent<SpawnSpace>());
        }
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = new Vector3(0, 0, _player.transform.position.z);
    }
}
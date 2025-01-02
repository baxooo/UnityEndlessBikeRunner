using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrafficGenerator : MonoBehaviour
{
    private readonly Dictionary<int, float> _possiblePositions = new()
    {
        { 1, -4.9f },
        { 2, -1.65f },
        { 3, 1.65f },
        { 4, 4.9f }
    };

    [SerializeField] private int minCarsAtOnce = 8;
    [SerializeField] private GameObject[] vehicles = new GameObject[6];
    [SerializeField] private LayerMask carLayer;

    private List<GameObject> _spawnedVehicles = new();
    private GameManager _gameManager;
    private GameObject _player;
    private VehiclesSpawnGrid _spawnGrid;

    // Start is called before the first frame update
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _spawnGrid = GameObject.Find("SpawnGrid(Clone)").GetComponent<VehiclesSpawnGrid>();
        _player = _gameManager.Player;

        for (var i = 0; i < minCarsAtOnce / 2; i++) SpawnFirstVehicles(Random.Range(20, 100));

        for (var i = minCarsAtOnce / 2; i < minCarsAtOnce; i++) SpawnVehicleInGrid();
    }

    private void FixedUpdate()
    {
        // per evitare errori mentre si itera sulla lista e rimuovendo oggetti dalla lista,
        // meglio procedere all'indietro così da evitare problemi con gli indici
        for (var i = _spawnedVehicles.Count - 1; i >= 0; i--)
        {
            var vehicle = _spawnedVehicles[i];
            if (vehicle.transform.position.z + 40 < _player.transform.position.z) DestroyVehicle(vehicle);
        }

        if (_spawnedVehicles.Count < minCarsAtOnce)
        {
            var length = Random.Range(4, 9);
            for (var i = 0; i < length; i++) SpawnVehicleInGrid();
        }
    }

    private void SpawnVehicleInGrid()
    {
        var emptySpace = false;
        while (!emptySpace)
        {
            var spacePick = Random.Range(0, _spawnGrid.SpawnedSpaces.Count);
            var vehiclePick = Random.Range(0, vehicles.Length);

            emptySpace = _spawnGrid.SpawnedSpaces[spacePick].IsEmpty;

            if (emptySpace)
            {
                var instance = Instantiate(vehicles[vehiclePick],
                    _spawnGrid.SpawnedSpaces[spacePick].transform.position, new Quaternion());
                _spawnedVehicles.Add(instance);
            }
        }
    }

    private void DestroyVehicle(GameObject vehicle)
    {
        _spawnedVehicles.Remove(vehicle);
        Destroy(vehicle);
    }

    private void SpawnFirstVehicles(int range)
    {
        float zAxisSpawn = range;
        var vehiclePick = Random.Range(0, vehicles.Length);
        var positionPick = Random.Range(1, _possiblePositions.Count + 1);

        var instance = Instantiate(vehicles[vehiclePick], new Vector3(_possiblePositions[positionPick], 0, zAxisSpawn),
            new Quaternion());

        CheckForObstacleInFrontOrBehind(instance);
        _spawnedVehicles.Add(instance);
    }

    private void CheckForObstacleInFrontOrBehind(GameObject carObject)
    {
        var carPosition = new Vector3(carObject.transform.position.x, 1, carObject.transform.position.z);

        for (var i = 0; i < 10; i++)
        {
            var hitInFront = Physics.Raycast(carPosition, transform.TransformDirection(Vector3.forward), 15, carLayer);
            var hitBehind = Physics.Raycast(carPosition, transform.TransformDirection(Vector3.back), 12, carLayer);

            if (hitBehind && hitInFront)
            {
                var newPosition = _possiblePositions.Where(f => !(Mathf.Abs(transform.position.x - f.Value) < 0.1f));
                var position = Random.Range(0, newPosition.Count());

                carObject.transform.position = new Vector3(
                    newPosition.ToList()[position].Value,
                    carObject.transform.position.y,
                    carObject.transform.position.z
                );
            }
            else if (hitInFront)
            {
                carObject.transform.position = new Vector3(
                    carObject.transform.position.x,
                    carObject.transform.position.y,
                    carObject.transform.position.z - Random.Range(10, 20)
                );
            }
            else if (hitBehind)
            {
                carObject.transform.position = new Vector3(
                    carObject.transform.position.x,
                    carObject.transform.position.y,
                    carObject.transform.position.z + Random.Range(5, 10)
                );
            }
        }
    }

    public void DestroyAllVehicles()
    {
        for (var i = _spawnedVehicles.Count - 1; i >= 0; i--) DestroyVehicle(_spawnedVehicles[i]);
    }
}
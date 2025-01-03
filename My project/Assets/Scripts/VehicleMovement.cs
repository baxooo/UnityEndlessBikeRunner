using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [SerializeField] private float minSpeed = 7f;
    [SerializeField] private List<GameObject> wheels = new();
    [SerializeField] private LayerMask carLayer;


    private readonly Dictionary<int, float> _possiblePositions = new()
    {
        { 1, -4.85000f },
        { 2, -1.75000f },
        { 3, 1.75000f },
        { 4, 4.85000f }
    };

    public float Speed { get; private set; }
    private float _originalSpeed;
    private float _currentSpeed;
    private CharacterController _player;

    //adding this because there are too many overtakes,
    //for a smoother experience it is better to have a random limit
    //to vehicles that can overtake and perhaps a cooldown
    private bool _canOvertake;

    private float _targetLaneX;
    private bool _changingLane;

    private Vector3 _bounds;
    private GameObject _cube;

    //overtake manager
    public GameObject overtakeManagerPrefab;
    private VehicleOvertakeManager _overtakeManager;


    // Start is called before the first frame update
    private void Start()
    {
        InitializeVehicle();
        if (_canOvertake)
            InitializeOvertakeManager();
    }

    private void InitializeOvertakeManager()
    {
        var manager = Instantiate(overtakeManagerPrefab, transform, false);

        _overtakeManager = manager.GetComponent<VehicleOvertakeManager>();
    }

    private void InitializeVehicle()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
        _canOvertake = UnityEngine.Random.Range(0, 100) < 50;

        Speed = Mathf.Max(minSpeed + UnityEngine.Random.Range(0f, 2f),
            _player.velocity.z * 0.6f + UnityEngine.Random.Range(-2f, 2f));
        _originalSpeed = Speed;
        _targetLaneX = transform.position.x;
        _bounds = GetComponent<BoxCollider>().bounds.size;
    }


    private void Update()
    {
        CheckForVehicleInFront();

        MoveVehicleForward();

        RotateWheels();

        if (!_changingLane)
            return;

        HandleLaneChange();
    }

    private void MoveVehicleForward()
    {
        _currentSpeed = Mathf.Lerp(_currentSpeed, Speed, Time.fixedDeltaTime * 2f);
        transform.Translate(_currentSpeed * Time.deltaTime * Vector3.forward);
    }

    private void HandleLaneChange()
    {
        var newXPosition =
            Mathf.MoveTowards(transform.position.x, _targetLaneX, Time.deltaTime * (_currentSpeed * 0.1f));
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

        if (Mathf.Abs(transform.position.x - _targetLaneX) < 0.01f)
        {
            _changingLane = false;
            Speed = _originalSpeed;
        }
    }

    private void RotateWheels()
    {
        foreach (var wheel in wheels)
            wheel.transform.Rotate(Vector3.right * Speed);
    }


    private void CheckForVehicleInFront()
    {
        var forward = transform.TransformDirection(Vector3.forward) * 19;
        Vector3 leftVehicleSide = new(transform.position.x - 1.5f, 1, transform.position.z);
        Vector3 rightVehicleSide = new(transform.position.x + 1.5f, 1, transform.position.z);

        var position = new Vector3(transform.position.x, 1, transform.position.z);
        Debug.DrawRay(position, forward, Color.green);
        Debug.DrawRay(leftVehicleSide, forward, Color.green);
        Debug.DrawRay(rightVehicleSide, forward, Color.green);

        var hasObstacleInFront = Physics.Raycast(position, forward, out var hit, 19, carLayer) ||
                                 Physics.Raycast(leftVehicleSide, forward, out hit, 19, carLayer) ||
                                 Physics.Raycast(rightVehicleSide, forward, out hit, 19, carLayer);

        if (!hasObstacleInFront)
        {
            Speed = _originalSpeed;
            return;
        }


        if (!hit.transform.gameObject.CompareTag("Car"))
            return;

        var otherVehicle = hit.transform.gameObject.GetComponent<VehicleMovement>();

        //slowdown if the other vehicle is slower
        if (Speed > otherVehicle.Speed)
            Speed = Mathf.Lerp(Speed, otherVehicle.Speed, 0.15f);

        if (_canOvertake && !_changingLane)
            ChangeLane();
    }

    private void ChangeLane()
    {
        var currentPosition =
            _possiblePositions.FirstOrDefault(v => Math.Abs(v.Value - transform.position.x) < 0.1).Key;

        var leftLaneExists = _possiblePositions.ContainsKey(currentPosition - 1);
        var rightLaneExists = _possiblePositions.ContainsKey(currentPosition + 1);

        var canMoveLeft = leftLaneExists && _overtakeManager.IsLeftFree;

        var canMoveRight = rightLaneExists && _overtakeManager.IsRightFree;

        if (canMoveLeft && canMoveRight)
        {
            int[] options = { 1, -1 };
            _targetLaneX = _possiblePositions[currentPosition + options[UnityEngine.Random.Range(0, 2)]];
            _changingLane = true;
        }
        else if (canMoveLeft)
        {
            _targetLaneX = _possiblePositions[currentPosition - 1];
            _changingLane = true;
        }
        else if (canMoveRight)
        {
            _targetLaneX = _possiblePositions[currentPosition + 1];
            _changingLane = true;
        }
    }
}
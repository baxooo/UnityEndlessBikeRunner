using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 7f;
    [SerializeField] private List<GameObject> _wheels = new();
    [SerializeField] private LayerMask _carLayer;

    private readonly Dictionary<int, float> _possiblePositions = new()
    {
        { 1, -4.85000f },
        { 2, -1.75000f },
        { 3, 1.75000f },
        { 4, 4.85000f }
    };

    private float _speed;
    private float _originalSpeed;
    private float _currentSpeed;
    private bool _slowedDown;
    private CharacterController _player;
    public float Speed => _speed;
    private float _targetLaneX;
    private bool _changingLane;

    private bool _canOvertake; //adding this because there are too many overtakes,
    //for a smoother experience it is better to have a random limit
    //to vehicles that can overtake and perhaps a cooldown

    private Vector3 _bounds;
    private GameObject _cube;

    // Start is called before the first frame update
    private void Start()
    {
        var charController = GameObject.FindWithTag("Player");
        _player = charController.GetComponent<CharacterController>();

        _canOvertake = UnityEngine.Random.Range(0, 100) < 50;

        // this is a test, remove it when done ##########
        _cube = _canOvertake ? GameObject.CreatePrimitive(PrimitiveType.Cube) : new GameObject("empty brother");
        _cube.transform.SetParent(transform);
        // this is a test, remove it when done ##########

        _speed = Mathf.Max(_minSpeed + UnityEngine.Random.Range(0f, 2f),
            _player.velocity.z * 0.6f + UnityEngine.Random.Range(-2f, 2f));
        _originalSpeed = _speed;
        _targetLaneX = transform.position.x;
        _bounds = GetComponent<BoxCollider>().bounds.size;
    }

    // Update is called once per frame
    private void Update()
    {
        // this is a test, remove it when done ##########
        _cube.transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        // this is a test, remove it when done ##########

        _currentSpeed = Mathf.Lerp(_currentSpeed, _speed, Time.deltaTime * 2f);
        transform.Translate(_currentSpeed * Time.deltaTime * Vector3.forward);

        foreach (var wheel in _wheels) wheel.transform.Rotate(Vector3.right * _speed);

        if (_changingLane)
        {
            var newXPosition = Mathf.MoveTowards(transform.position.x, _targetLaneX,
                Time.deltaTime * (_currentSpeed * 0.1f));
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);

            if (Mathf.Abs(transform.position.x - _targetLaneX) < 0.01f)
            {
                _changingLane = false;
                _speed = _originalSpeed;
            }
        }
    }

    private void FixedUpdate()
    {
        CheckForVehicleInFront();
    }

    private void CheckForVehicleInFront()
    {
        //check for other vehicle in front

        var forward = transform.TransformDirection(Vector3.forward) * 19;
        Vector3 leftVehicleSide = new(transform.position.x - 1.5f, 1, transform.position.z);
        Vector3 rightVehicleSide = new(transform.position.x + 1.5f, 1, transform.position.z);

        var position = new Vector3(transform.position.x, 1, transform.position.z);
        Debug.DrawRay(position, forward, Color.green);
        Debug.DrawRay(leftVehicleSide, forward, Color.green);
        Debug.DrawRay(rightVehicleSide, forward, Color.green);

        var hasObstacleInFront = Physics.Raycast(position, forward, out var hit, 19, _carLayer) ||
                                 Physics.Raycast(leftVehicleSide, forward, out hit, 19, _carLayer) ||
                                 Physics.Raycast(rightVehicleSide, forward, out hit, 19, _carLayer);

        if (!hasObstacleInFront)
        {
            if (_slowedDown)
                _speed = _originalSpeed;
            return;
        }

        var otherVehicle = hit.transform.gameObject.GetComponent<VehicleMovement>();

        //slowdown if the other vehicle is slower
        if (_speed > otherVehicle.Speed)
            _speed = Mathf.Lerp(_speed, otherVehicle.Speed, 0.15f);

        if (_canOvertake)
            ChangeLane();
    }

    private void OnDrawGizmos()
    {
        var rayDistanceForward = 20f;
        var rayDistanceBack = 10f;

        var raySideDistance = 5f;

        Vector3 leftSide = new(transform.position.x - 3.2f, 1, transform.position.z);
        Vector3 middleLeftSide = new(transform.position.x - 1.7f, 1, transform.position.z);
        Vector3 middleRightSide = new(transform.position.x + 1.7f, 1, transform.position.z);
        Vector3 rightSide = new(transform.position.x + 3.2f, 1, transform.position.z);
        Vector3 center = new(transform.position.x, 1, transform.position.z);

        Vector3 backLateralPosition = new(transform.position.x, 1, transform.position.z - _bounds.z / 2);
        Vector3 frontLateralPosition = new(transform.position.x, 1, transform.position.z + _bounds.z / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(leftSide, Vector3.forward * rayDistanceForward);
        Gizmos.DrawRay(middleLeftSide, Vector3.forward * rayDistanceForward);
        Gizmos.DrawRay(leftSide, Vector3.back * rayDistanceBack);

        Gizmos.DrawRay(frontLateralPosition, Vector3.left * raySideDistance);
        Gizmos.DrawRay(backLateralPosition, Vector3.left * raySideDistance);
        Gizmos.DrawRay(center, Vector3.left * raySideDistance);
        Gizmos.DrawRay(frontLateralPosition, Vector3.right * raySideDistance);
        Gizmos.DrawRay(backLateralPosition, Vector3.right * raySideDistance);
        Gizmos.DrawRay(center, Vector3.right * raySideDistance);

        Gizmos.DrawRay(rightSide, Vector3.forward * rayDistanceForward);
        Gizmos.DrawRay(middleRightSide, Vector3.forward * rayDistanceForward);
        Gizmos.DrawRay(rightSide, Vector3.back * rayDistanceBack);
    }

    private void ChangeLane()
    {
        if (_changingLane) return;

        Vector3 leftSide = new(transform.position.x - 3, 1, transform.position.z);
        Vector3 middleLeftSide = new(transform.position.x - 1.7f, 1, transform.position.z);
        Vector3 rightSide = new(transform.position.x + 3, 1, transform.position.z);
        Vector3 middleRightSide = new(transform.position.x + 1.7f, 1, transform.position.z);
        Vector3 backLateralPosition = new(transform.position.x, 1, transform.position.z - _bounds.z / 2);
        Vector3 frontLateralPosition = new(transform.position.x, 1, transform.position.z + _bounds.z / 2);
        Vector3 center = new(transform.position.x, 1, transform.position.z);


        var currentPosition =
            _possiblePositions.FirstOrDefault(v => Math.Abs(v.Value - transform.position.x) < 0.1).Key;

        var leftLaneExists = _possiblePositions.ContainsKey(currentPosition - 1);
        var rightLaneExists = _possiblePositions.ContainsKey(currentPosition + 1);

        var rayDistance = 20f;

        var canMoveLeft = leftLaneExists && !Physics.Raycast(leftSide, Vector3.forward, rayDistance + 1, _carLayer) &&
                          !Physics.Raycast(leftSide, Vector3.back, rayDistance - 10, _carLayer) &&
                          !Physics.Raycast(middleLeftSide, Vector3.back, rayDistance + 1, _carLayer);

        var canMoveRight = rightLaneExists &&
                           !Physics.Raycast(rightSide, Vector3.forward, rayDistance + 1, _carLayer) &&
                           !Physics.Raycast(rightSide, Vector3.back, rayDistance - 10, _carLayer) &&
                           !Physics.Raycast(middleRightSide, Vector3.back, rayDistance + 1, _carLayer);

        var hasCarOnLeftSide = Physics.Raycast(frontLateralPosition, Vector3.left, 5, _carLayer) ||
                               Physics.Raycast(center, Vector3.left, 5, _carLayer) ||
                               Physics.Raycast(backLateralPosition, Vector3.left, 5, _carLayer);

        var hasCarOnRightSide = Physics.Raycast(frontLateralPosition, Vector3.right, 5, _carLayer) ||
                                Physics.Raycast(center, Vector3.right, 5, _carLayer) ||
                                Physics.Raycast(backLateralPosition, Vector3.right, 5, _carLayer);


        if (canMoveLeft && canMoveRight && !hasCarOnLeftSide && !hasCarOnRightSide)
        {
            int[] options = { 1, -1 };
            _targetLaneX = _possiblePositions[currentPosition + options[UnityEngine.Random.Range(0, 2)]];
            _changingLane = true;
        }
        else if (canMoveLeft && !hasCarOnLeftSide)
        {
            _targetLaneX = _possiblePositions[currentPosition - 1];
            _changingLane = true;
        }
        else if (canMoveRight && !hasCarOnRightSide)
        {
            _targetLaneX = _possiblePositions[currentPosition + 1];
            _changingLane = true;
        }
    }
}
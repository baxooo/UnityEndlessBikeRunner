using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BikeController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;
    [SerializeField] private float _breakPower;
    [SerializeField] private float _minSpeed = 7;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _leaningSpeed;
    [SerializeField] private float _maxLeaningAngle;
    [SerializeField] private GameObject _bikeBody;
    [SerializeField] private GameObject _steeringColumn;
    [SerializeField] private GameObject _cameraPosition;

    private Camera _camera;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _currentSpeed = 0f;
    private float _speedSmoothVelocity;
    private float _speedSmoothLean;
    private float _currentLeanSpeed;
    private float _currentLeanAngle;
    private float _targetLeanAngle;
    private float _targetLeanSpeed;


    // Start is called before the first frame update
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _camera = Camera.main;
        _camera.transform.SetParent(_cameraPosition.transform);
        _camera.transform.position = _cameraPosition.transform.position;
        _camera.transform.rotation = _cameraPosition.transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        _targetLeanSpeed = horizontalInput * _leaningSpeed;
        _targetLeanAngle = horizontalInput * _maxLeaningAngle;

        var verticalInput = Input.GetAxis("Vertical");
        float targetSpeed;
        float smoothTime;

        switch (verticalInput)
        {
            case > 0:
                targetSpeed = _maxSpeed;
                smoothTime = _acceleration;
                break;
            case < 0:
                targetSpeed = _minSpeed;
                smoothTime = _breakPower;
                break;
            default:
                targetSpeed = _minSpeed;
                smoothTime = _deceleration;
                break;
        }

        _currentSpeed = Mathf.SmoothDamp(
            _currentSpeed,
            targetSpeed,
            ref _speedSmoothVelocity,
            smoothTime
        );

        _currentLeanSpeed = Mathf.Lerp(_targetLeanSpeed, _currentLeanSpeed, 0.14f);
        _currentLeanAngle = Mathf.Lerp(_targetLeanAngle, _currentLeanAngle, 0.14f);

        _velocity.z = _currentSpeed;
        _velocity.x = _currentLeanSpeed;
        _characterController.Move(_velocity * Time.deltaTime);

        var leanAngle = _bikeBody.transform.rotation.z - _currentLeanAngle;
        _bikeBody.transform.eulerAngles =
            new Vector3(_bikeBody.transform.eulerAngles.x, _bikeBody.transform.eulerAngles.y, leanAngle);

        //this counters the lean and points straight, or counter steers
        var steerAngle = _steeringColumn.transform.rotation.z + _currentLeanAngle * 2 + 180;

        //this actually does the steering effect
        _steeringColumn.transform.eulerAngles =
            new Vector3(_steeringColumn.transform.eulerAngles.x, _steeringColumn.transform.eulerAngles.y,
                steerAngle - leanAngle * 2.4f);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] _levelPartArray;

    private GameObject _player;
    private Vector3 _lastEndPosition;
    private int _currentRoad;
    private GameManager _gameManager;


    // Start is called before the first frame update
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _player = _gameManager.Player;

        _currentRoad = 0;
        _lastEndPosition = _levelPartArray[_currentRoad].Find("End").position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_lastEndPosition.z < _player.transform.position.z)
        {
            _levelPartArray[_currentRoad].position = new Vector3(0, 0,
                _levelPartArray[_currentRoad].position.z + 90 * _levelPartArray.Length);

            _currentRoad++;
            if (_currentRoad == _levelPartArray.Length)
                _currentRoad = 0;

            _lastEndPosition = _levelPartArray[_currentRoad].Find("End").position;
        }
    }
}
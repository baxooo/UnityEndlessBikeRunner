using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _environmentPrefab;
    public GameObject StaticBikePrefab;
    [SerializeField] private GameObject _showRoomPrefab;
    [SerializeField] private GameObject SpawnGridPrefab;
    public GameObject SpawnGrid { get; private set; }
    public Text Score;
    public Text Speed;
    public Text OvertakeCombo;

    public GameObject panelMenu;
    public GameObject panelPause;
    public GameObject panelGameOver;
    public GameObject panelPlay;

    public GameObject Player { get; private set; }
    private CharacterController _playerController;
    private GameObject _environment;
    private GameObject _showRoom;
    private StateEnum _currentState;

    private Camera _camera;

    // Start is called before the first frame update
    private void Start()
    {
        _showRoom = Instantiate(_showRoomPrefab);
        SwitchState(StateEnum.MENU);
        _camera = Camera.main;
    }

    public void SwitchState(StateEnum newState)
    {
        EndState();
        BeginState(newState);
    }

    private void BeginState(StateEnum state)
    {
        switch (state)
        {
            case StateEnum.MENU:
                panelMenu.SetActive(true);
                _showRoom.SetActive(true);
                _currentState = state;
                break;
            case StateEnum.PLAY:
                panelPlay.SetActive(true);
                _currentState = state;
                break;
            case StateEnum.PAUSE:
                panelPause.SetActive(true);
                _currentState = state;
                break;
            case StateEnum.GAMEOVER:
                panelGameOver.SetActive(true);
                Time.timeScale = 0;
                _currentState = state;
                break;
        }
    }

    private void EndState()
    {
        switch (_currentState)
        {
            case StateEnum.MENU:
                panelMenu.SetActive(false);
                _showRoom.SetActive(false);
                break;
            case StateEnum.PLAY:
                panelPlay.SetActive(false);
                break;
            case StateEnum.PAUSE:
                panelPause.SetActive(false);
                break;
            case StateEnum.GAMEOVER:
                panelGameOver.SetActive(false);
                Time.timeScale = 1;
                break;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case StateEnum.MENU:
                break;
            case StateEnum.PLAY:

                if (!Player) // checks if player is null
                {
                    Player = Instantiate(_playerPrefab);
                    SpawnGrid = Instantiate(SpawnGridPrefab);
                    Player.TryGetComponent(out _playerController);
                }

                if (!_environment)
                    _environment = Instantiate(_environmentPrefab);
                Speed.text = "Speed: " + _playerController.velocity.z.ToString("F0") + " km/h";

                break;
            case StateEnum.GAMEOVER:
                break;
        }
    }

    public void PlayClicked()
    {
        SwitchState(StateEnum.PLAY);
    }

    public void ResumeClicked()
    {
        SwitchState(StateEnum.PLAY);
    }

    public void RestartClicked()
    {
        _camera.transform.parent = null;

        var tg = _environment.GetComponent<TrafficGenerator>();
        tg.DestroyAllVehicles();

        Destroy(Player);
        Destroy(_environment);
        Destroy(SpawnGrid);

        PlayClicked();
    }

    public void MainMenuClicked()
    {
        _camera.transform.parent = null;
        _camera.transform.SetPositionAndRotation(new Vector3(3f, 2.15f, 3f),
            Quaternion.Euler(new Vector3(13, 225, 0))); // to be changed

        var tg = _environment.GetComponent<TrafficGenerator>();
        tg.DestroyAllVehicles();

        Destroy(Player);
        Destroy(_environment);
        Destroy(SpawnGrid);

        SwitchState(StateEnum.MENU);
    }

    public void ExitGameClicked()
    {
        Application.Quit();
    }
}
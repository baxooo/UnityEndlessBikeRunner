using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public GameObject PanelMenu;
    public GameObject PanelPause;
    public GameObject PanelGameOver;

    public GameObject Player;
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
                PanelMenu.SetActive(true);
                _showRoom.SetActive(true);
                _currentState = state;
                break;
            case StateEnum.PLAY:
                _currentState = state;
                break;
            case StateEnum.PAUSE:
                PanelPause.SetActive(true);
                _currentState = state;
                break;
            case StateEnum.GAMEOVER:
                PanelGameOver.SetActive(true);
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
                PanelMenu.SetActive(false);
                _showRoom.SetActive(false);
                break;
            case StateEnum.PLAY:
                break;
            case StateEnum.PAUSE:
                PanelPause.SetActive(false);
                break;
            case StateEnum.GAMEOVER:
                PanelGameOver.SetActive(false);
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
                }

                if (!_environment)
                    _environment = Instantiate(_environmentPrefab);
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
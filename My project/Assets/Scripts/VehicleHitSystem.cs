using Assets.Scripts.Enums;
using UnityEngine;

public class VehicleHitSystem : MonoBehaviour
{
    private GameManager _gameManager;

    // Start is called before the first frame update
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _gameManager.SwitchState(StateEnum.GAMEOVER);
    }
}
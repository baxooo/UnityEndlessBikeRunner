using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowRoomRotation : MonoBehaviour
{
    [SerializeField] private GameObject _cylinder;
    private Transform _center;
    private GameObject _bike;
    private GameObject _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager");
        var script = _gameManager.GetComponent<GameManager>();
        _center = _cylinder.transform.Find("CylinderTopCenter");

        _bike = Instantiate(script.StaticBikePrefab);
        _bike.transform.position = _center.position;

        _bike.transform.SetParent(_cylinder.transform);
    }

    // Update is called once per frame
    void Update()
    {
        _cylinder.transform.Rotate(new Vector3(0,0.01f,0));
    }
}

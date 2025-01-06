using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckRandomCarSpawner : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject center;

    // Start is called before the first frame update
    private void Start()
    {
        if (Random.Range(0, 100) < 75)
            return;
        var selectedCar = cars[Random.Range(0, cars.Length)];

        Instantiate(selectedCar, center.transform, false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject platformPrefab;
    public int count = 300;
    
    public float minGapY = -0.4f;  // min vertical distance between platforms
    public float maxGapY = 0.1f; 

    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPosition = new Vector3();

        for (int i = 0; i < count; i++) {
            float gapY = Random.Range(minGapY, maxGapY);
            spawnPosition.y += gapY;
            spawnPosition.x = Random.Range(-0.1f, 0.6f);
            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        }
        
    }
}

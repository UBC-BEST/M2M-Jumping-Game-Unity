using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Platform Generation")]
    public GameObject platformPrefab;
    public int count = 300;
    public float minGapY = -0.4f;
    public float maxGapY = -0.3f;
    
    [Header("Death Settings")]
    public float deathZoneOffset = 8f; // Distance below camera where player dies
    public GameObject gameOverUI;
    
    private bool isGameOver = false;
    private Camera mainCamera;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        mainCamera = Camera.main;
    }
    
    void Start()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        
        // Generate platforms
        Vector3 spawnPosition = new Vector3();
        for (int i = 0; i < count; i++) {
            float gapY = Random.Range(minGapY, maxGapY);
            spawnPosition.y += gapY;
            spawnPosition.x = Random.Range(-0.1f, 0.6f);
            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        }
    }
    
    public void TriggerGameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Time.timeScale = 0f; // Pause the game
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        
        Debug.Log("Game Over!");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume normal time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
    
    public float GetDeathZone()
    {
        if (mainCamera != null)
        {
            return mainCamera.transform.position.y - deathZoneOffset;
        }
        return -10f;
    }
}
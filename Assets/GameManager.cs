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

        // Max Reachable Height
        // H = v^2 / (2g)
        float maxJumpHeight = 5f; // Default fallback
        float jumpVelocity = 0f;
        
        if (platformPrefab != null)
        {
            Platformer platformScript = platformPrefab.GetComponent<Platformer>();
            if (platformScript != null)
            {
                jumpVelocity = platformScript.jumpForce;
            }
        }

        float gravity = Mathf.Abs(Physics2D.gravity.y); 
        if (gravity > 0 && jumpVelocity > 0)
        {
            maxJumpHeight = (jumpVelocity * jumpVelocity) / (2 * gravity);
        }

        Debug.Log($"Calculated Max Jump Height: {maxJumpHeight}");

        float minSafeGap = 0.3f; 
        
        // Maximum gap strictly less than max jump height
        float maxSafeGap = maxJumpHeight * 0.75f;

        if (minGapY < 0) minGapY = Mathf.Abs(minGapY);
        if (maxGapY < 0) maxGapY = Mathf.Abs(maxGapY);

        if (minGapY < minSafeGap) 
        {
            minGapY = minSafeGap;
            Debug.Log($"Adjusted minGapY to {minGapY} to prevent overlaps.");
        }

        if (maxGapY > maxSafeGap) 
        {
            maxGapY = maxSafeGap;
            Debug.Log($"Adjusted maxGapY to {maxGapY} to ensure reachability.");
        }

        if (maxGapY < minGapY)
        {
            maxGapY = minGapY;
        }

        
        // Generate platforms
        Vector3 spawnPosition = new Vector3();
        for (int i = 0; i < count; i++) {
            float gapY = Random.Range(minGapY, maxGapY);
            spawnPosition.y += gapY;
            spawnPosition.x = Random.Range(-0.2f, 0.9f);
            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        }
    }
    
    public void TriggerGameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Time.timeScale = 0f; 
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        
        Debug.Log("Game Over!");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f; 
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public Rigidbody2D rb;
    private float moveX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveX = Input.GetAxis("Horizontal") * moveSpeed;
        
        // Check if player has fallen below death zone
        CheckDeathZone();
    }

    private void FixedUpdate() 
    {
        Vector2 velocity = rb.velocity;
        velocity.x = moveX;
        rb.velocity = velocity;
    }
    
    void CheckDeathZone()
    {
        if (GameManager.Instance != null)
        {
            if (transform.position.y < GameManager.Instance.GetDeathZone())
            {
                Die();
            }
        }
    }
    
    public void Die()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver())
        {
            Debug.Log("You died");
            
            GameManager.Instance.TriggerGameOver();
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            Die();
        }
    }
}
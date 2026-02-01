using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        // Check if collision is with the player
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Die();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPlatform : Platformer
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        // Only trigger if player is falling down onto the platform
        if (collision.relativeVelocity.y <= 0f)
        {
            base.OnCollisionEnter2D(collision);
            Destroy(gameObject, 0.1f); // Brief delay before disappearing
        }
    }
}

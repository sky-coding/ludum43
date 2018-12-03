using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    public Player player;
    private Vector3 _velocity;
    private float _yOffset = 75f;
    private float _xOffset = 20f;
    private float _speedMultiplier = 5f;

    void Update()
    {
        var playerPosition = player.transform.position;
        var targetY = playerPosition.y + _yOffset;
        var vectorToPlayer = playerPosition - transform.position;
        var targetX = transform.position.x;
        if (vectorToPlayer.x > _xOffset)
        {
            targetX = playerPosition.x - _xOffset;
        }
        else if (vectorToPlayer.x < -_xOffset)
        {
            targetX = playerPosition.x + _xOffset;
        }

        var vectorToTarget = new Vector3(targetX, targetY) - transform.position;
        _velocity = vectorToTarget * _speedMultiplier;

        if (_velocity.x > 0.1f && transform.localScale.x < 0f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (_velocity.x < -0.1f && transform.localScale.x > 0f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        transform.position += _velocity * Time.deltaTime;
    }
}

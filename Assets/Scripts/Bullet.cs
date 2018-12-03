using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _damage = 10;
    public Vector3 velocity;
    private float _distanceTraveled = 0f;
    public Sounds sounds;

    void Update()
    {
        if (_distanceTraveled > 500f)
        {
            Destroy(gameObject);
            return;
        }

        var movement = velocity * Time.deltaTime;
        _distanceTraveled += movement.magnitude;
        transform.position += movement;
    }

    void OnTriggerEnter2D(Collider2D triggeredCollider)
    {
        var villager = triggeredCollider.gameObject.GetComponent<Villager>();
        if (villager != null)
        {
            if (villager.isDead) return;

            villager.Damage(_damage);
            sounds.impactAudioSources[Random.Range(0, sounds.impactAudioSources.Count)].Play();
        }

        Destroy(gameObject);
    }
}

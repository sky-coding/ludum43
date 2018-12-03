using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class House : MonoBehaviour
{
    public GameObject villagerPrefab;

    public GameObject doorOpen;
    public GameObject doorClosed;

    // private Vector3 _villagerSpawnPoint;
    private Vector3 _villagerSpawnOffset;
    public int villagersInside = 5;
    private GameObject _player;
    private List<House> _otherHouses = new List<House>();
    private float evacuateThreshold = 300f;
    private float evacuateCooldown = 0.2f;
    private float _evacuateCooldownRemaining = 0f;
    private float _villagerRespawnRate = 10f;
    private float _villagerRespawnRateRemaining = 10f;
    private int _villagerRespawnCap = 100;

    private PolygonCollider2D _levelBoundsPolygonCollider2D;
    private Sounds _sounds;

    void Awake()
    {
        _levelBoundsPolygonCollider2D = GameObject.FindGameObjectWithTag("LevelBounds").GetComponent<PolygonCollider2D>();
        _sounds = GameObject.FindGameObjectWithTag("Sounds").GetComponent<Sounds>();
        _player = GameObject.FindGameObjectWithTag("Player");
        var houses = GameObject.FindGameObjectsWithTag("House");
        foreach (var house in houses)
        {
            if (house == this.gameObject) continue;
            _otherHouses.Add(house.GetComponent<House>());
        }

        var doorCollider = GetComponent<BoxCollider2D>();
        var hit = Physics2D.Raycast(doorCollider.bounds.center, new Vector2(0f, -1f), 100f, LayerMask.GetMask("Ground"));
        var villagerSpawnPoint = new Vector3(doorCollider.bounds.center.x, hit.point.y + 19f, 0f);
        _villagerSpawnOffset = villagerSpawnPoint - transform.position;
    }

    void Update()
    {
        var distanceToPlayer = (_player.transform.position - transform.position).magnitude;
        var shouldEvacuate = distanceToPlayer < evacuateThreshold;

        if (shouldEvacuate && doorClosed.activeSelf)
        {
            doorClosed.SetActive(false);
            doorOpen.SetActive(true);
            _sounds.doorOpenAudioSources[Random.Range(0, _sounds.doorOpenAudioSources.Count)].Play();
        }
        else if (!shouldEvacuate && doorOpen.activeSelf)
        {
            doorClosed.SetActive(true);
            doorOpen.SetActive(false);
            _sounds.doorCloseAudioSources[Random.Range(0, _sounds.doorCloseAudioSources.Count)].Play();
        }

        _villagerRespawnRateRemaining -= Time.deltaTime;
        if (_villagerRespawnRateRemaining <= 0f)
        {
            var villagersInHouses = _otherHouses.Sum(house => house.villagersInside) + villagersInside;
            if (villagersInHouses < _villagerRespawnCap && !shouldEvacuate)
            {
                villagersInside++;
            }
            _villagerRespawnRateRemaining = _villagerRespawnRate;
        }
        if (!shouldEvacuate) return;
        if (villagersInside < 1) return;
        _evacuateCooldownRemaining -= Time.deltaTime;
        if (_evacuateCooldownRemaining > 0f) return;
        _evacuateCooldownRemaining = evacuateCooldown;
        villagersInside--;
        var villagerGameObject = Instantiate(villagerPrefab, transform.position + _villagerSpawnOffset, Quaternion.identity);
        var villager = villagerGameObject.GetComponent<Villager>();
        villager.sourceHouse = this;
        villager.destinationHouse = _otherHouses[Random.Range(0, _otherHouses.Count)];
        villager.runSpeed = Random.Range(villager.runSpeedMin, villager.runSpeedMax);
        villager.levelBoundsPolygonCollider2D = _levelBoundsPolygonCollider2D;
    }

    void OnTriggerEnter2D(Collider2D triggeredCollider)
    {
        var villager = triggeredCollider.gameObject.GetComponent<Villager>();
        if (villager == null) return;
        if (villager.destinationHouse.gameObject != gameObject) return;
        if (villager.sourceHouse.gameObject == gameObject) return;

        Destroy(villager.gameObject);
        villagersInside++;
    }
}

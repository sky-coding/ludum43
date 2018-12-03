using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public Player player;
    public GameObject ground1;
    public GameObject ground2;
    public GameObject ground3;
    private float groundChunkSize = 2048f;
    private float slightBuffer = 50f;

    public PolygonCollider2D levelBoundsPolygonCollider;

    public CinemachineConfiner confiner;

    private GameObject previousGround;
    private GameObject activeGround;
    private GameObject nextGround;

    private List<GameObject> _villagersBuffer = new List<GameObject>();

    // Use this for initialization
    void Awake()
    {
        previousGround = ground1;
        activeGround = ground2;
        nextGround = ground3;
    }

    void Update()
    {
        if (player.transform.position.x > activeGround.transform.position.x + groundChunkSize + slightBuffer)
        {
            ClearGroundOfVillagers(previousGround);

            var temp = previousGround;

            previousGround = activeGround;
            activeGround = nextGround;
            
            var position = temp.transform.position;
            position.x = activeGround.transform.position.x + groundChunkSize;
            temp.transform.position = position;
            nextGround = temp;
        }
        else if (player.transform.position.x < activeGround.transform.position.x - slightBuffer)
        {
            ClearGroundOfVillagers(nextGround);

            var temp = nextGround;

            nextGround = activeGround;
            activeGround = previousGround;

            var position = temp.transform.position;
            position.x = activeGround.transform.position.x - groundChunkSize;
            temp.transform.position = position;
            previousGround = temp;
        }
        else
        {
            return;
        }

        var updatedBounds = new Vector2[4];
        updatedBounds[0] = new Vector2(previousGround.transform.position.x, previousGround.transform.position.y);
        updatedBounds[1] = new Vector2(previousGround.transform.position.x, previousGround.transform.position.y + groundChunkSize);
        updatedBounds[2] = new Vector2(nextGround.transform.position.x + groundChunkSize, nextGround.transform.position.y + groundChunkSize);
        updatedBounds[3] = new Vector2(nextGround.transform.position.x + groundChunkSize, nextGround.transform.position.y);
        levelBoundsPolygonCollider.points = updatedBounds;

        confiner.InvalidatePathCache();
    }

    void ClearGroundOfVillagers(GameObject ground)
    {
        var villagers = GameObject.FindGameObjectsWithTag("Villager");
        foreach (var villager in villagers)
        {
            if (villager.transform.position.x > ground.transform.position.x &&
                villager.transform.position.x < ground.transform.position.x + groundChunkSize)
            {
                _villagersBuffer.Add(villager);
            }
        }

        var houses = ground.GetComponentsInChildren<House>();

        foreach (var villager in _villagersBuffer)
        {
            var house = houses[Random.Range(0, houses.Length)];
            house.villagersInside++;
            Destroy(villager);
        }
    }
}

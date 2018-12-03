using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Player player;
    public Camera mainCamera;

    private Vector3 _previousCameraPosition;

    public float parallaxSpeed = 0.25f;

    public GameObject background1;
    public GameObject background2;
    public GameObject background3;

    private float chunkSize = 2048f;
    private float slightBuffer = 50f;

    private GameObject previousBackground;
    private GameObject middleBackground;
    private GameObject nextBackground;

    void Start()
    {
        previousBackground = background1;
        middleBackground = background2;
        nextBackground = background3;

        _previousCameraPosition = mainCamera.transform.position;
    }

    void Update()
    {
        if (player.transform.position.x > nextBackground.transform.position.x + slightBuffer)
        {
            var temp = previousBackground;

            previousBackground = middleBackground;
            middleBackground = nextBackground;

            var position = temp.transform.position;
            position.x = middleBackground.transform.position.x + chunkSize;
            temp.transform.position = position;
            nextBackground = temp;
        }
        else if (player.transform.position.x < middleBackground.transform.position.x - slightBuffer)
        {
            var temp = nextBackground;

            nextBackground = middleBackground;
            middleBackground = previousBackground;

            var position = temp.transform.position;
            position.x = middleBackground.transform.position.x - chunkSize;
            temp.transform.position = position;
            previousBackground = temp;
        }

        var cameraMovement = mainCamera.transform.position - _previousCameraPosition;
        transform.position += cameraMovement * parallaxSpeed;
        _previousCameraPosition = mainCamera.transform.position;
    }
}

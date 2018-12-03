using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f;
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;

    [HideInInspector]
    private float _normalizedHorizontalSpeed = 0;

    public CharacterController2D characterController;
    public Animator animator;
    public GameObject arm;
    public GameObject bulletOrigin;
    public GameObject bulletPrefab;
    
    private Vector3 _velocity;
    private float _armAngle;

    private float _bulletSpeed = 1000f;
    private float _rapidShootThreshold = 1f;
    private float _slowShootThreshold = 3f;
    private float _timeSpentShooting = 0f;
    private float _rapidShootCooldown = 0.075f;
    private float _shootCooldown = 0.25f;
    private float _slowShootCooldown = 0.5f;
    private float _shootCooldownRemaining = 0f;

    private Sounds _sounds;

    void Awake()
    {
        _sounds = GameObject.FindGameObjectWithTag("Sounds").GetComponent<Sounds>();
    }

    void Update()
    {
        if (characterController.isGrounded) _velocity.y = 0;

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0.01f) _normalizedHorizontalSpeed = 1;
        else if (horizontalInput < -0.01f) _normalizedHorizontalSpeed = -1;
        else _normalizedHorizontalSpeed = 0;

        if (characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
        }

        var smoothedMovementFactor = characterController.isGrounded ? groundDamping : inAirDamping;
        _velocity.x = Mathf.Lerp(_velocity.x, _normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
        _velocity.y += gravity * Time.deltaTime;
        characterController.move(_velocity * Time.deltaTime);
        _velocity = characterController.velocity;
        animator.SetFloat("absoluteXVelocity", Mathf.Abs(_velocity.x));
        animator.SetBool("isGrounded", characterController.isGrounded);

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var vectorToMouse = mousePosition - transform.position;

        if (vectorToMouse.x > 0.01f && transform.localScale.x < 0f) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (vectorToMouse.x < -0.01f && transform.localScale.x > 0f) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        arm.transform.rotation = CalculateArmAngle(mousePosition);

        _shootCooldownRemaining -= Time.deltaTime;

        var isShooting = Input.GetButton("Fire");
        if (isShooting && _shootCooldownRemaining <= 0f)
        {
            if (_timeSpentShooting < _rapidShootThreshold)
            {
                _shootCooldownRemaining = _rapidShootCooldown;
            }
            else if (_timeSpentShooting > _slowShootThreshold)
            {
                _shootCooldownRemaining = _slowShootCooldown;
            }
            else
            {
                _shootCooldownRemaining = _shootCooldown;
            }

            var bulletVector = (Vector2)(Quaternion.Euler(0, 0, _armAngle) * Vector2.right);

            if (transform.localScale.x < 0f)
            {
                bulletVector *= -1;
            }

            var bulletGameObject = Instantiate(bulletPrefab, bulletOrigin.transform.position, Quaternion.identity);
            var bullet = bulletGameObject.GetComponent<Bullet>();
            bullet.velocity = bulletVector * _bulletSpeed;
            bullet.sounds = _sounds;

            _sounds.shootAudioSources[Random.Range(0, _sounds.shootAudioSources.Count)].Play();
        }

        if (isShooting) _timeSpentShooting += Time.deltaTime;
        else _timeSpentShooting = 0f;
    }

    Quaternion CalculateArmAngle(Vector3 mousePosition)
    {
        // var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var vector = (transform.localScale.x < 0f)
            ? arm.transform.position - mousePosition
            : mousePosition - arm.transform.position;

        vector.x = Mathf.Abs(vector.x);

        _armAngle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        if (_armAngle > 45f) _armAngle = 45f;
        else if (_armAngle < -45f) _armAngle = -45f;

        return Quaternion.Euler(new Vector3(0, 0, _armAngle));
    }
}

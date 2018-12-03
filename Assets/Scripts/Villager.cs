using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public float gravity = -25f;
    public float runSpeedMin = 8f;
    public float runSpeedMax = 15f;
    public float runSpeed = 8f;
    public float groundDamping = 20f;
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;

    [HideInInspector]
    private float _normalizedHorizontalSpeed = 0;

    public CharacterController2D characterController;
    public Animator animator;
    public SpriteRenderer bodySpriteRenderer;
    public SpriteRenderer hairSpriteRenderer;
    public SpriteRenderer shirtSpriteRenderer;
    public SpriteRenderer pantsSpriteRenderer;

    private Vector3 _velocity;

    public House sourceHouse;
    public House destinationHouse;

    public ParticleSystem shotParticleSystem;
    public ParticleSystem deathParticleSystem;

    private int _health = 30;

    [HideInInspector]
    public bool isDead = false;

    public PolygonCollider2D levelBoundsPolygonCollider2D;

    private Sounds _sounds;

    private static List<Color> hairColors = new List<Color> { new Color32(20, 16, 19, 255), new Color32(160, 134, 98, 255), new Color32(115, 23, 45, 255), new Color32(254, 243, 192, 255) };
    private static List<Color> shirtColors = new List<Color> { new Color32(20, 16, 19, 255), new Color32(115, 23, 45, 255), new Color32(166, 252, 219, 255), new Color32(188, 74, 155, 255) };
    private static List<Color> pantsColors = new List<Color> { new Color32(20, 16, 19, 255), new Color32(66, 57, 52, 255) };

    void Awake()
    {
        _sounds = GameObject.FindGameObjectWithTag("Sounds").GetComponent<Sounds>();

        hairSpriteRenderer.color = hairColors[Random.Range(0, hairColors.Count)];
        shirtSpriteRenderer.color = shirtColors[Random.Range(0, shirtColors.Count)];
        pantsSpriteRenderer.color = pantsColors[Random.Range(0, pantsColors.Count)];
    }

    void Start()
    {
        var vectorToDestination = destinationHouse.gameObject.transform.position - transform.position;
        _normalizedHorizontalSpeed = vectorToDestination.x < 0 ? -1f : 1f;
        if (vectorToDestination.x > 0f)
        {
            _normalizedHorizontalSpeed = 1;
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (vectorToDestination.x < 0f)
        {
            _normalizedHorizontalSpeed = -1;
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (characterController.isGrounded) _velocity.y = 0;

        var smoothedMovementFactor = characterController.isGrounded ? groundDamping : inAirDamping;
        _velocity.x = Mathf.Lerp(_velocity.x, _normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
        _velocity.y += gravity * Time.deltaTime;
        characterController.move(_velocity * Time.deltaTime);
        _velocity = characterController.velocity;

        var levelBoundsMin = levelBoundsPolygonCollider2D.bounds.min;
        var levelBoundsMax = levelBoundsPolygonCollider2D.bounds.max;
        if (transform.position.x < levelBoundsMin.x || transform.position.x > levelBoundsMax.x ||
            transform.position.y < levelBoundsMin.y || transform.position.y > levelBoundsMax.y)
        {
            Destroy(gameObject);
        }
    }

    public void Damage(int amount)
    {
        _health -= amount;

        shotParticleSystem.Play();

        if (_health <= 0)
        {
            isDead = true;
            deathParticleSystem.Play();
            _sounds.deathAudioSources[Random.Range(0, _sounds.deathAudioSources.Count)].Play();
            GetComponent<BoxCollider2D>().enabled = false;

            StartCoroutine(Sacrifice());
            StartCoroutine(FadeAndDeactivate());
            StartCoroutine(DestroyAfterDelay());
        }
    }

    IEnumerator Sacrifice()
    {
        deathParticleSystem.Play();

        yield return new WaitForSeconds(1.5f);

        var particlesBuffer = new ParticleSystem.Particle[deathParticleSystem.main.maxParticles];
        var liveParticlesBuffer = new ParticleSystem.Particle[deathParticleSystem.main.maxParticles];

        var collisionModule = deathParticleSystem.collision;
        collisionModule.enabled = false;

        var velocity = 100f;
        var maxVelocity = 1000f;
        var percentVelocityGainPerSecond = 5f;

        var spiritGameObject = GameObject.FindGameObjectWithTag("Spirit");
        var gameManagerGameObject = GameObject.FindGameObjectWithTag("GameManager");
        var gameManager = gameManagerGameObject.GetComponent<GameManager>();

        while (deathParticleSystem.particleCount > 0)
        {
            velocity = Mathf.Min(velocity + (velocity * percentVelocityGainPerSecond * Time.deltaTime), maxVelocity);

            var targetPosition = spiritGameObject.transform.position;

            var particleCount = deathParticleSystem.particleCount;
            var liveParticleCount = 0;

            deathParticleSystem.GetParticles(particlesBuffer);

            for (var i = 0; i < particleCount; i++)
            {
                var vectorToTarget = targetPosition - particlesBuffer[i].position;

                // is less than one unit away, or will reach target next frame(ish)
                if (vectorToTarget.magnitude < 1f || velocity * Time.deltaTime > vectorToTarget.magnitude)
                {
                    // implicitly kill particle by not adding it to liveParticlesBuffer

                    gameManager.SacrificeCollected();
                    // sacrificeCollectedAudioSource.Play();
                }
                else
                {
                    var velocityToTarget = Vector3.Normalize(vectorToTarget) * velocity;

                    liveParticlesBuffer[liveParticleCount] = particlesBuffer[i];
                    liveParticlesBuffer[liveParticleCount].velocity = velocityToTarget;
                    liveParticleCount++;
                }
            }

            deathParticleSystem.SetParticles(liveParticlesBuffer, liveParticleCount);

            yield return null;
        }
    }

    IEnumerator FadeAndDeactivate()
    {
        var bodySpriteRendererColor = bodySpriteRenderer.color;
        var hairSpriteRendererColor = hairSpriteRenderer.color;
        var shirtSpriteRendererColor = shirtSpriteRenderer.color;
        var pantsSpriteRendererColor = pantsSpriteRenderer.color;

        var fadeRate = 2f;
        var alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= fadeRate * Time.deltaTime;

            bodySpriteRendererColor.a = alpha;
            bodySpriteRenderer.color = bodySpriteRendererColor;

            hairSpriteRendererColor.a = alpha;
            hairSpriteRenderer.color = hairSpriteRendererColor;

            shirtSpriteRendererColor.a = alpha;
            shirtSpriteRenderer.color = shirtSpriteRendererColor;

            pantsSpriteRendererColor.a = alpha;
            pantsSpriteRenderer.color = pantsSpriteRendererColor;

            yield return null;
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject sacrificeBarFill;

    private float _timeElapsed = 0f;
    private float _initialTimeBeforeDecay = 10f;

    private float _sacrificePoints = 50f;
    private float _maxSacrificePoints = 500f;
    private float _sacrificeDecayRate = 5f;

    private bool _isGameOver = false;

    private Sounds _sounds;

    void Awake()
    {
        _sounds = GameObject.FindGameObjectWithTag("Sounds").GetComponent<Sounds>();
    }

    void Update()
    {
        if (_isGameOver) return;

        _timeElapsed += Time.deltaTime;

        if (_timeElapsed > _initialTimeBeforeDecay)
        {
            _sacrificePoints -= _sacrificeDecayRate * Time.deltaTime;
        }

        if (_sacrificePoints <= 0f)
        {
            // lose game
            BlackOverlay.Instance.FadeToScene("Failure");
            _isGameOver = true;
        }
        else if (_sacrificePoints >= _maxSacrificePoints)
        {
            // win game
            BlackOverlay.Instance.FadeToScene("Success");
            _isGameOver = true;
        }

        var percentage = _sacrificePoints / _maxSacrificePoints;
        if (percentage < 0f) percentage = 0f;
        if (percentage > 1f) percentage = 1f;
        var scale = sacrificeBarFill.transform.localScale;
        scale.y = percentage;
        sacrificeBarFill.transform.localScale = scale;
    }

    public void SacrificeCollected()
    {
        _sacrificePoints += 2f;
        _sounds.sacrificeAudioSources[Random.Range(0, _sounds.sacrificeAudioSources.Count)].Play();
    }
}

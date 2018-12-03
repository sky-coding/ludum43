using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlackOverlay : Singleton<BlackOverlay>
{
    public SpriteRenderer spriteRenderer;
    private bool _fadingToScene = false;

    public void FadeToScene(String scene)
    {
        StartCoroutine(FadeToSceneCoroutine(scene));
    }

    private IEnumerator FadeToSceneCoroutine(String scene)
    {
        _fadingToScene = true;

        var fadeRate = 0.1f;

        // fade out
        {
            var alpha = 0f;
            var color = spriteRenderer.color;

            while (alpha < 1f)
            {
                color.a = alpha;
                spriteRenderer.color = color;
                alpha += fadeRate;
                yield return new WaitForSeconds(0.02f);
            }

            alpha = 1f;
            color.a = alpha;
            spriteRenderer.color = color;

            yield return null;
        }

        var asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        // fade in
        {
            var alpha = 1f;
            var color = spriteRenderer.color;

            while (alpha > 0f)
            {
                color.a = alpha;
                spriteRenderer.color = color;
                alpha -= fadeRate;
                yield return new WaitForSeconds(0.02f);
            }

            alpha = 0f;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        _fadingToScene = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameController : MonoBehaviour
{
    private bool _clicked = false;

    public void HandlePlayAgainButton()
    {
        if (_clicked) return;

        BlackOverlay.Instance.FadeToScene("Main Menu");

        _clicked = true;
    }
}

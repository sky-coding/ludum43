using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private bool _clicked = false;

    public void HandleContinueButton()
    {
        if (_clicked) return;

        BlackOverlay.Instance.FadeToScene("Main");

        _clicked = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    void OnFart();
    void OnLeftPressed();
    void OnLeftReleased();
    void OnRightPressed();
    void OnRightReleased();
    void OnInteract();
    void OnJump();
}

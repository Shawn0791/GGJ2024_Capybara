using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public interface IMountable : IInteractible
{
    void Dismount();
    void OnLeftPressed();
    void OnLeftReleased();
    void OnRightPressed();
    void OnRightReleased();
}

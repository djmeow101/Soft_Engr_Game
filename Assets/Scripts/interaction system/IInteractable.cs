using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public interface IInteractable
{
    public string InteractionPrompt{get;}
    public bool Interact (Interactor interactor);
}

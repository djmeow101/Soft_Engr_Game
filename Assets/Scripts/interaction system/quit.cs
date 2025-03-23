using UnityEngine;  // <- This is required
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class quit : MonoBehaviour, IInteractable
{   
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    public bool Interact(Interactor interactor)
    {
        Cursor.visible = true;      // Makes the cursor visible
        Cursor.lockState = CursorLockMode.None;
       SceneManager.LoadScene(2);
       return true;
         // Unlocks the cursor
    }
    
}
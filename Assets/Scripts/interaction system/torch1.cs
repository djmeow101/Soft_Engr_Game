using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class torch1 : MonoBehaviour, IInteractable
{   
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private string _prompt;
    [SerializeField] private Transform wall;
    [SerializeField] private PuzzleData puzzleData;
    private bool isPulled =true;
    public string InteractionPrompt => _prompt;
    public bool Interact (Interactor interactor)
    {
        if(isPulled){
            if(puzzleData.turn1 && puzzleData.turn2 && puzzleData.turn3){
                Debug.Log("sdf");
                transform.Rotate(_rotation);
                wall.position = wall.position+ new Vector3(0,100f,0);
                isPulled = false;
            }
        }
        return true;
    }
}

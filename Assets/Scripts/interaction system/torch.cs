using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class torch : MonoBehaviour, IInteractable
{   
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private string _prompt;
    [SerializeField] private Transform wall;
    private bool isPulled =true;
    public string InteractionPrompt => _prompt;
    public bool Interact (Interactor interactor)
    {
        if(isPulled){
        Debug.Log("sdf");
        transform.Rotate(_rotation*Time.deltaTime);
        wall.position = wall.position+ new Vector3(0,100f,0);
        isPulled = false;
        }
        return true;
    }
}

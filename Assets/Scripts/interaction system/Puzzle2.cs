using UnityEngine;  // <- This is required
using System.Collections;
using System.Collections.Generic;

public class Puzzle2 : MonoBehaviour, IInteractable
{   
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private string _prompt;
    [SerializeField] private PuzzleData puzzleData;
    [SerializeField] private int column_num;
    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor interactor)
    {
        transform.Rotate(_rotation);
        if(column_num == 1){
            if (puzzleData.turnNum1 < 3)
                puzzleData.turnNum1++;
            else
                puzzleData.turnNum1 = 0;

            puzzleData.turn1 = puzzleData.turnNum1 == 3;

            Debug.Log("Turn number1: " + puzzleData.turnNum1);
        }
        if(column_num == 2){
            if (puzzleData.turnNum2 < 3)
                puzzleData.turnNum2++;
            else
                puzzleData.turnNum2 = 0;

            puzzleData.turn2 = puzzleData.turnNum2 == 2;

            Debug.Log("Turn number2: " + puzzleData.turnNum2);
        }
        if(column_num == 3){
            if (puzzleData.turnNum3 < 3)
                puzzleData.turnNum3++;
            else
                puzzleData.turnNum3 = 0;

            puzzleData.turn3 = puzzleData.turnNum3 == 3;

            Debug.Log("Turn number3: " + puzzleData.turnNum3);
        }
        return true;
    }
}

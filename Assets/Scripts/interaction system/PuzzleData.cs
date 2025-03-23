using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleData", menuName = "Puzzle/PuzzleData")]
public class PuzzleData : ScriptableObject
{
    public int turnNum1;
    public int turnNum2;
    public int turnNum3;
    public bool turn1;
    public bool turn2;
    public bool turn3;

    private void OnEnable()
    {
        ResetData();
    }

    public void ResetData()
    {
        turnNum1 = 0;
        turnNum2 = 0;
        turnNum3 = 0;
        turn1 = false;
        turn2 = false;
        turn3 = false;
    }
}

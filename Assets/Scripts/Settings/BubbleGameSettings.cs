using UnityEngine;

[CreateAssetMenu(fileName = "BubbleGameSettings", menuName = "Bubble Game Settings", order = 1)]
public class BubbleGameSettings : ScriptableObject
{
    [Tooltip ("X size of the grid")]
    public int GridSizeX;

    [Tooltip("Y size of the grid")]
    public int GridSizeY;

    [Tooltip ("How many rows should be generated at start?")]
    public int StartingRowCount;

    [Tooltip ("How many rows should be generated at per turn?")]
    public int RowsAtPerTurn;

    [Tooltip ("How much crowdness rows should have. It's a randomizer between 0 - 100")]
    [Range(0,100)]
    public int RowCrowdness;
}

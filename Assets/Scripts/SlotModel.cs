using UnityEngine;

public class SlotModel : MonoBehaviour
{   
    private int rowIndex = 0;
    private int colIndex = 0;
    private int winIndex = 0;

    public int RowIndex{get{return rowIndex;} set{rowIndex = value;}}
    public int ColIndex{get{return colIndex;} set{colIndex = value;}}
    public int WinIndex{get{return winIndex;} set{winIndex = value;}}
}

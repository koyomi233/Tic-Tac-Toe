using UnityEngine;
using UnityEngine.UI;

public class ChessboardModel : MonoBehaviour
{
    [SerializeField] private Button slotButton;
    [SerializeField] private Button player1Button;
    [SerializeField] private Button player2Button;

    public Button SlotButton{get{return slotButton;}}
    public Button Player1Button{get{return player1Button;}}
    public Button Player2Button{get{return player2Button;}}
}

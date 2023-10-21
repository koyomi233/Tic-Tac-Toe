using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{   
    ChessboardController chessboardController;
    SlotModel m_slotModel;
    private void Start() {
        GameObject chessboard = GameObject.Find("Chessboard");
        m_slotModel = GetComponent<SlotModel>();
        if(chessboard){
            chessboardController = chessboard.GetComponent<ChessboardController>();
        }
        GetComponent<Button>().onClick.AddListener(OnEmptySlotButtonClicked);
        
    }
    
    public void OnEmptySlotButtonClicked(){
        if(chessboardController != null && m_slotModel != null){
            chessboardController.PlayerMove(m_slotModel.RowIndex, m_slotModel.ColIndex, m_slotModel.WinIndex);
        }
    }
}

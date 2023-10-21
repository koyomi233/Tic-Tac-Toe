using UnityEngine;
using UnityEngine.UI;

public class ChessboardView : MonoBehaviour
{
    private ChessboardModel m_ChessboardModel;
    private GameObject redTurnImg;
    private GameObject blueTurnImg;

    private void Awake() {
        m_ChessboardModel = gameObject.GetComponent<ChessboardModel>();
        redTurnImg = GameObject.Find("RedTurn");
        blueTurnImg = GameObject.Find("BlueTurn");
    }

    //刷新整个棋盘
    public void RefreshChessboard(){
        for(int i = 0; i < transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }

        int winindex = 0;
        for(int i = 0; i < 3; i++){
            for(int j = 0; j < 3; j++){
                if(m_ChessboardModel != null){
                    Button slot = Instantiate(m_ChessboardModel.SlotButton);
                    if(slot){
                        SlotModel slotModel = slot.GetComponent<SlotModel>();
                        if(slotModel){
                            slotModel.RowIndex = i;
                            slotModel.ColIndex = j;
                            slotModel.WinIndex = winindex;
                            slot.transform.SetParent(transform);
                            slot.transform.localScale = Vector3.one;
                            winindex++;
                        }
                    }
                }
            }
        }
    }

    // 设置对应玩家的棋子
    public void ReplaceChessButton(int playerIndex, int location){
        Destroy(transform.GetChild(location).gameObject);
        Button slot = null;
        if(playerIndex == -1){
            slot = Instantiate(m_ChessboardModel.Player1Button);
        }
        else if(playerIndex == 1){
            slot = Instantiate(m_ChessboardModel.Player2Button);
        }
        if(slot){
            slot.transform.SetParent(transform);
            slot.transform.localScale = Vector3.one;
            slot.transform.SetSiblingIndex(location);
        }
    }
    
    public void ShowCurrentTurnImage(int playerIndex){
        if(playerIndex == -1){
            redTurnImg.SetActive(false);
            blueTurnImg.SetActive(true);
        }
        else{
            redTurnImg.SetActive(true);
            blueTurnImg.SetActive(false);
        }
    }
}

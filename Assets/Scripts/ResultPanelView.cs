using UnityEngine;

public class ResultPanelView : MonoBehaviour
{
    private GameObject m_ResultTitleRed;
    private GameObject m_ResultTitleBlue;
    private GameObject m_ResultTitleTied;

    private void Awake() {
        m_ResultTitleRed = GameObject.Find("ResultTitleRed");
        m_ResultTitleBlue = GameObject.Find("ResultTitleBlue");
        m_ResultTitleTied = GameObject.Find("ResultTitleTied");
    }
    
    public void SetResultPanelVisible(bool visible){
        gameObject.SetActive(visible);
    }

    public void ShowPlayerTitle(int playerIndex, bool isTied){
        if(isTied){
            m_ResultTitleBlue.SetActive(false);
            m_ResultTitleRed.SetActive(false);
            m_ResultTitleTied.SetActive(true);
        }
        else if(playerIndex == -1){
            m_ResultTitleBlue.SetActive(true);
            m_ResultTitleRed.SetActive(false);
            m_ResultTitleTied.SetActive(false);
        }
        else if(playerIndex == 1){
            m_ResultTitleBlue.SetActive(false);
            m_ResultTitleRed.SetActive(true);
            m_ResultTitleTied.SetActive(false);
        }
    }
}

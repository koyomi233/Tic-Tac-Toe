using System;
using System.Collections;
using UnityEngine;

public class ChessboardController : MonoBehaviour
{
    private int gameMode = 0;                                           // 当前游戏模式；0-单人，1-双人
    private bool locker = false;
    private int currentPlayer = 1;                                      // 当前即将下棋的玩家
    private int[,] chessRecord;                                         // 当局棋盘的棋子记录
    private ChessboardView m_ChessboardView;

    private ResultPanelView m_ResultPanelView;

    private AudioSource m_PressSound;

    void Start()
    {
        m_ChessboardView = gameObject.GetComponent<ChessboardView>();
        m_ResultPanelView = GameObject.Find("ResultPanel").GetComponent<ResultPanelView>();
        m_PressSound = GetComponent<AudioSource>();

        chessRecord = new int[3, 3];

        RequestStartPlay();
    }

    void Update() {
        if(gameMode == 0) ComputerMove();
    }

    // 开始游戏
    public void RequestStartPlay(){
        if(gameMode == 0) StopCoroutine("DoComputerMove");
        m_ResultPanelView.SetResultPanelVisible(false);
        Array.Clear(chessRecord, 0, 9);
        // 先刷新棋盘
        if(m_ChessboardView){
            
            m_ChessboardView.RefreshChessboard();
        }

        // 做个随机看谁先手；更新下当前玩家
        int rand = UnityEngine.Random.Range(0, 2);
        if(rand == 0) {
            currentPlayer = -1;
            locker = false;
        }
        else {
            currentPlayer = 1;
        }
        m_ChessboardView.ShowCurrentTurnImage(currentPlayer);
        locker = false;
    }

    // 玩家走一步
    public void PlayerMove(int row, int col, int winIndex){
        if(gameMode == 0 && currentPlayer == 1){
            return;
        }
        if(locker) return;
        locker = true;
        
        // 如果目标棋位已经被下过了，则不做处理，等待下次有效执行
        if(chessRecord[row, col] != 0){
            return;
        }
        // 添加记录，并且更新View
        chessRecord[row, col] = currentPlayer;
        m_ChessboardView.ReplaceChessButton(currentPlayer, winIndex);
        m_PressSound.Play();

        OnMoveFinished();
    }

    // 电脑走一步
    private void ComputerMove(){
        if(locker || currentPlayer == -1) return;
        locker = true;
        // 用协程等1s再执行，模拟电脑思考，顺带解决窗口刷新冲突问题
        StartCoroutine("DoComputerMove");
    }

    IEnumerator DoComputerMove(){
        yield return new WaitForSeconds(1.0f);
        int row = 0;
        int col = 0;
        int bestScore = -1000;
        int score = 0;
        for(int i = 0; i < 3; i++){
            for(int j = 0; j < 3; j++){
                if(chessRecord[i, j] != 0) continue;

                chessRecord[i, j] = 1;

                // alpha beta剪枝，上下界任意取一个比较大的数
                score = BestInput(-1, 1, -1000, 1000);
                if(score >= bestScore){
                    bestScore = score;
                    row = i;
                    col = j;
                }
                chessRecord[i, j] = 0;
            }
        }

        if(chessRecord[row, col] == 0){
            chessRecord[row, col] = 1;
            m_ChessboardView.ReplaceChessButton(currentPlayer, row * 3 + col);
            m_PressSound.Play();
            OnMoveFinished();
        }        
    }

    // Alpha-Beta剪枝MinMax获取最优解
    private int BestInput(int currentPlayer, int nextPlayer, int a, int b){
        int res = CheckResult();
        if(res == -1) return -1;
        if(res == 1) return 1;
        if(res == 0) return 0;
        if(res == 2){
            int score = 0;
            for(int i = 0; i < 3; i++){
                for(int j = 0; j < 3; j++){
                    if(chessRecord[i, j] != 0) continue;
                    // 模拟玩家下在（i,j）位置，并记录对应的得分
                    chessRecord[i, j] = currentPlayer;
                    score = BestInput(nextPlayer, currentPlayer, a, b);
                    // 还原棋位
                    chessRecord[i, j] = 0;

                    // Max
                    if(currentPlayer == 1){
                        if(score >= a) a = score;
                        if(a > b) return b;
                    }
                    // Min
                    else{
                        if(score < b) b = score;
                        if(b <= a) return a;
                    }
                }
            }
        }
        if(currentPlayer == 1) return a;
        else return b;
    }

    // 检查对局是否结束，-1-玩家1胜利，1-玩家2(电脑)胜利，0-平局，2-未结束
    private int CheckResult(){
        // 判断行和列
        for(int i = 0; i < 3; i++){
            int sumRow = 0;
            int sumCol = 0;
            for(int j = 0; j < 3; j++){
                sumRow += chessRecord[i, j];
                sumCol += chessRecord[j, i];
            }
            if(sumRow == -3 || sumCol == -3){
                return -1;
            }
            if(sumRow == 3 || sumCol == 3){
                return 1;
            }
        }

        // 判断对角线
        int first = 0;
        int second = 0;
        for(int i = 0; i < 3; i++){
            first += chessRecord[i, i];
            second += chessRecord[i, 2 - i];
        }
        if(first == -3 || second == -3){
            return -1;
        }
        if(first == 3 || second == 3){
            return 1;
        }

        // 判断是否平局
        if(chessRecord.Length == 9){
            foreach(int elem in chessRecord){
                if(elem == 0){
                    return 2;
                }
            }
        } 
        return 0;
    }

    // 任意玩家移动完毕后判断游戏是否结束
    private void OnMoveFinished(){
        if(CheckResult() == -1){
            // 玩家1胜利
            m_ResultPanelView.SetResultPanelVisible(true);
            m_ResultPanelView.ShowPlayerTitle(-1, false);
        }else if(CheckResult() == 1){
            // 玩家2胜利
            m_ResultPanelView.SetResultPanelVisible(true);
            m_ResultPanelView.ShowPlayerTitle(1, false);
        }else if(CheckResult() == 0){
            // 平局
            m_ResultPanelView.SetResultPanelVisible(true);
            m_ResultPanelView.ShowPlayerTitle(1, true);
        }else{
            // 对局未结束，换人并等待下一次移动
            currentPlayer *= -1;
            m_ChessboardView.ShowCurrentTurnImage(currentPlayer);
            locker = false;
        }  
    }
}

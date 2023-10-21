using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private AudioSource bgAudio;
    private void Start() {
        GameObject[] BGSound = GameObject.FindGameObjectsWithTag("BGSound");
        if(BGSound.Length > 1){
            Destroy(GameObject.FindGameObjectsWithTag("BGSound")[1]);
        }
        DontDestroyOnLoad(BGSound[0]);
        bgAudio = BGSound[0].GetComponent<AudioSource>();
    }

    public void OnButtonGameStart(){
        SceneManager.LoadScene("MainLevel");
    }

    public void OnButtonGameQuit(){
        Application.Quit();
    }

    public void OnButtonHome(){
        SceneManager.LoadScene("MenuLevel");
    }

    public void OnButtonMusicPressed(){
        if(bgAudio){
            if(bgAudio.isPlaying){
                bgAudio.Pause();
            }else{
                bgAudio.UnPause();
            }
        }
    }
}

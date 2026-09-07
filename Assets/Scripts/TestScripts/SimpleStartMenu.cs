using UnityEngine;

public class SimpleStartMenu : MonoBehaviour
{
    public GameObject RetryButton;
    public GameObject StartButton;

    public GameObject Panel;


    private void Start()
    {
        RetryButton.SetActive(false);
        Panel.SetActive(false);
        StartButton.SetActive(false);
    }
    
    public void ShowMenu(bool win){
        if(win){
            RetryButton.SetActive(false);
            StartButton.SetActive(true);
        } else {
            RetryButton.SetActive(true);
            StartButton.SetActive(false);
        }
        Panel.SetActive(true);
    }

    public void HideMenu(){
        RetryButton.SetActive(false);
        StartButton.SetActive(false);
        Panel.SetActive(false);
    }
}
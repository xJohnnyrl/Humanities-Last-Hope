using UnityEngine;


public class OpenShop : MonoBehaviour
{
    [SerializeField] GameObject shop;
    [SerializeField] GameObject pause;
    [SerializeField] GameObject darkbackground;

    public void ActiveShop(){
        shop.SetActive(true);
        darkbackground.SetActive(true);
        Time.timeScale = 0;
    }
    public void CloseShop() {
        shop.SetActive(false);
        darkbackground.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenPauseMenu() {
        pause.SetActive(true);
        darkbackground.SetActive(true);
        Time.timeScale = 0;
    }
    public void ClosePauseMenu() {
        pause.SetActive(false);
        darkbackground.SetActive(false);
        Time.timeScale = 1;
    }
}

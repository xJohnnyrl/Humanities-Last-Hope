using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenShop : MonoBehaviour
{
    [SerializeField] GameObject shop;
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

}

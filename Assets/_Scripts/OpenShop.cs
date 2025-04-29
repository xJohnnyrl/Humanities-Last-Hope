using UnityEngine;


public class OpenShop : MonoBehaviour
{

    [SerializeField] private AudioClip pauseSFX;
    [SerializeField] private AudioClip unpauseSFX;
    [SerializeField] private AudioClip ShopSFX;
    [SerializeField] private AudioClip closeshopSFX;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }


    [SerializeField] GameObject shop;
    [SerializeField] GameObject pause;
    [SerializeField] GameObject darkbackground;

    public void ActiveShop()
    {
        shop.SetActive(true);
        darkbackground.SetActive(true);
        Time.timeScale = 0;

        if (ShopSFX != null && audioSource != null)
            audioSource.PlayOneShot(ShopSFX);
    }
    public void CloseShop()
    {
        shop.SetActive(false);
        darkbackground.SetActive(false);
        Time.timeScale = 1;

        if (closeshopSFX != null && audioSource != null)
            audioSource.PlayOneShot(closeshopSFX);
    }

    public void OpenPauseMenu()
    {
        pause.SetActive(true);
        darkbackground.SetActive(true);
        Time.timeScale = 0;

        if (pauseSFX != null && audioSource != null)
            audioSource.PlayOneShot(pauseSFX);
    }
    public void ClosePauseMenu()
    {

        pause.SetActive(false);
        darkbackground.SetActive(false);
        Time.timeScale = 1;

        if (unpauseSFX != null && audioSource != null)
            audioSource.PlayOneShot(unpauseSFX);

    }
}

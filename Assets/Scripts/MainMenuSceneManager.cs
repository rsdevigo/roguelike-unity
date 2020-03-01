using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{
    public int boostUpFoodAmount;
    public Text boostUpNumberText;
    public Button boostUpButton;

    public int screenActive = 0;

    public GameObject[] screensObjects;

    void Awake () {
    }

    void Update () {
        if (screenActive == 1) {
            if (PlayerPrefs.GetInt("FoodBoostUp", 0) <= 0) {
                boostUpButton.interactable = false;
            } else {
                boostUpButton.interactable = true;
            }
            boostUpNumberText.text = "Você possui "+ PlayerPrefs.GetInt("FoodBoostUp", 0)+" BoostUp sobrando.";
        }
    }

    void changeScreen(int screenIndex) {
        screensObjects[screenActive].SetActive(false);
        screensObjects[screenIndex].SetActive(true);
        screenActive = screenIndex;
    }

    public void PlayButtonAction () {
        changeScreen(1);
        boostUpNumberText = GameObject.Find("BoostUpText").GetComponent<Text>();
        boostUpNumberText.text = "Você possui "+ PlayerPrefs.GetInt("FoodBoostUp", 0)+" BoostUp sobrando.";
    }

    public void CloseBoostUpChoice () {
        changeScreen(0);
        
    }

    public void SpendBoostUpAndPlay () {
        if (PlayerPrefs.GetInt("FoodBoostUp", 0) > 0) {
            PlayerPrefs.SetInt("FoodBoostUp", PlayerPrefs.GetInt("FoodBoostUp") -1);
            GameManager.instance.playerFoodPoints += boostUpFoodAmount;
        }
        PlayGame();
    }

    public void PlayGame () {
        GameManager.instance.gameStarted = true;
        SceneManager.LoadScene("MinhaCena");
    }

}

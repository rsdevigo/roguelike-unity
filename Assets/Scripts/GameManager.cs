using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float turnDelay = 0.1f;
    public float levelStartDelay = 2f;
    private BoardManager boardScript;
    public static GameManager instance = null;
    private int level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    public int playerFoodPoints = 50;
    [HideInInspector] public bool playersTurn = true;
    private Text levelText;
    private GameObject levelImage;
    private bool doingSetup;
    public bool gameStarted = true;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();

        boardScript = GetComponent<BoardManager>();
    }

    async void Start()
    {
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        levelImage.transform.Find("ResetGameButton").gameObject.SetActive(true);
        gameStarted = false;
        enabled = false;
    }

    public void ResetGame()
    {
        level = 0;
        playerFoodPoints = 50;
        enabled = true;
        Invoke("HideLevelImage", levelStartDelay);
        gameStarted = true;
        SceneManager.LoadScene("MinhaCena");
    }

    void InitGame()
    {
        doingSetup = true;
        Debug.Log("AQUIII");
        if (!SoundManager.instance.musicSource.isPlaying)
        {
            SoundManager.instance.musicSource.Play();
        }
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        levelImage.transform.Find("ResetGameButton").gameObject.SetActive(false);
        doingSetup = false;
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].isActiveAndEnabled)
            {
                enemies[i].MoveEnemy();
                yield return new WaitForSeconds(turnDelay);
            }
        }
        playersTurn = true;
        enemiesMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (gameStarted)
        {
            level++;
            InitGame();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
}

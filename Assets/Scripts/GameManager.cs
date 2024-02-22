using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public float turnDelay = 0.1f;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playerTurn = true;
    public BoardManager boardManager;
    public float levelStartDelay = 2f;
    public bool doingSetup;

    private List<Enemy> enemies = new List<Enemy>();
    private bool enemiesMoving;
    private int level = 0;
    private GameObject levelImage;
    private Text levelText;
    private Text foodText;


    private void Awake() {
        ConfigureLikeSingleton();

        boardManager = GetComponent<BoardManager>();
    }

    private void ConfigureLikeSingleton() {
        if (GameManager.instance != null && GameManager.instance != this) {
            Destroy(gameObject);
            return;
        }
        GameManager.instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        //InitGame();
    }

    private void InitGame() {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        UpdateFoodText(playerFoodPoints);
        levelImage.SetActive(true);

        enemies.Clear();
        boardManager.SetupScene(level);

        Invoke("HideLevelImage", levelStartDelay);

    }

    private void HideLevelImage() {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver() {
        levelText.text = "After " + level + " days, you starved...";
        levelImage.SetActive(true);
        enabled = false;
    }

    IEnumerator MoveEnemies() {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0) yield return new WaitForSeconds(turnDelay); // for levels without enemies

        foreach (Enemy enemy in enemies) {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playerTurn = true;
        enemiesMoving = false;
    }

    private void Update() {
        if (playerTurn || enemiesMoving || doingSetup) return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy enemy) {
        enemies.Add(enemy);
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;

    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        level++;
        InitGame();
    }

    public void UpdateFoodText(int food) {
        foodText.text = "Food: " + food;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

    public int wallDmg = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDealy = 1f;
    public GameObject foodAnimation;

    public AudioClip moveSound1, moveSound2, eatSound1, eatSound2, drinkSound1, drinkSound2, chopSound1, chopSound2, gameOverSound;

    private Animator animator;
    private int food;

    protected override void Awake() {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start() {
        food = GameManager.instance.playerFoodPoints;
        base.Start();
    }

    private void OnDisable() {
        GameManager.instance.playerFoodPoints = food;
    }

    void CheckIfGameOver() {
        if (food <= 0) {
            GameManager.instance.GameOver();
            AudioManager.instance.music.Stop();
            AudioManager.instance.PlaySingle(gameOverSound, 1f);
        }
    }

    protected override bool AttemptMove(int xDir, int yDir) {
        UpdateFood(-1); // 1 point per attempt movement
        bool canMove = base.AttemptMove(xDir, yDir);
        if (canMove) AudioManager.instance.PlayRandomizeSFX(moveSound1, moveSound2);
        CheckIfGameOver();
        GameManager.instance.playerTurn = false;

        return canMove;
    }

    private void Update() {
        if (!GameManager.instance.playerTurn || GameManager.instance.doingSetup) return;

        int horizontal;
        int vertical;
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0) vertical = 0; // diagonal movement is not allowed

        if (horizontal != 0 || vertical != 0) AttemptMove(horizontal, vertical); // attemp move
    }

    protected override void OnCantMove(GameObject go) {
        Wall hitWall = go.GetComponent<Wall>();

        if (hitWall != null) {
            hitWall.DamageWall(wallDmg);
            AudioManager.instance.PlayRandomizeSFX(chopSound1, chopSound2);
            animator.SetTrigger("playerChop");
        }
    }

    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseFood(int loss) {
        UpdateFood(-loss);
        animator.SetTrigger("playerHit");
        CheckIfGameOver(); ;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Exit")) {
            Invoke("Restart", restartLevelDealy);
            enabled = false;
        } else if (other.CompareTag("Food")) {
            AudioManager.instance.PlayRandomizeSFX(eatSound1, eatSound2);
            UpdateFood(pointsPerFood);
            other.gameObject.SetActive(false);
        } else if (other.CompareTag("Soda")) {
            AudioManager.instance.PlayRandomizeSFX(drinkSound1, drinkSound2);
            UpdateFood(pointsPerSoda);
            other.gameObject.SetActive(false);
        }
    }

    private void UpdateFood(int amount) {
        //ShowFoodUpdate(amount);
        food += amount;
        GameManager.instance.UpdateFoodText(food);
    }

    private void ShowFoodUpdate(int amount) {
        string text = amount < 0 ? amount.ToString() : "+" + amount.ToString();
        foodAnimation.GetComponent<TextMeshPro>().text = text;
        GameObject canvasGO = GameObject.Find("Canvas");
        Instantiate(foodAnimation, transform.position, Quaternion.identity).gameObject.transform.SetParent(canvasGO.transform);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDmg;

    public AudioClip enemyAttack1, enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    protected override void Awake() {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    protected override void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
        GameManager.instance.AddEnemyToList(this);
    }

    protected override bool AttemptMove(int xDir, int yDir) {
        if (skipMove) {
            skipMove = false;
            return false;
        }
        bool canMove = base.AttemptMove(xDir, yDir);
        skipMove = true;

        return canMove;
    }

    public void MoveEnemy() {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) { // same column
            // move enemy in vertical direction
            yDir = target.transform.position.y > transform.position.y ? 1 : -1;
        } else {
            // move enemy in horizontal
            xDir = target.transform.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove(xDir, yDir);
    }

    protected override void OnCantMove(GameObject go) {
        Player hitPlayer = go.GetComponent<Player>();

        if (hitPlayer != null) {
            hitPlayer.LoseFood(playerDmg);
            AudioManager.instance.PlayRandomizeSFX(enemyAttack1, enemyAttack2);
            animator.SetTrigger("enemyAttack");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime;
    public LayerMask blockingLayer;


    private float movementSpeed = 1f;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;

    protected virtual void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    protected virtual void Start() {
        movementSpeed = 1f / moveTime;
    }

    // Return true if the gameObject can be moved
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit) {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        boxCollider.enabled = false; // to skip the raycast detect the own collider
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null) {
            // move
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement(Vector2 end) {
        float remainingDistance = Vector2.Distance(rigidBody.position, end); // calculate the distance remains        
        while (remainingDistance > float.Epsilon) {
            Vector2 newPosition = Vector2.MoveTowards(rigidBody.position, end, movementSpeed * Time.deltaTime); // get a litle point forward
            rigidBody.MovePosition(newPosition);  // move
            remainingDistance = Vector2.Distance(rigidBody.position, end); // calculate the distance remains
            yield return null;
        }
    }

    protected abstract void OnCantMove(GameObject go);

    protected virtual bool AttemptMove(int xDir, int yDir) {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if (!canMove) OnCantMove(hit.transform.gameObject);

        return canMove;
    }
}

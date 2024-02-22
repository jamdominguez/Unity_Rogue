using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public Sprite dmgSprite;
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int damage) {
        spriteRenderer.sprite = dmgSprite;
        hp -= damage;
        if (hp <= 0) gameObject.SetActive(false);
    }
}

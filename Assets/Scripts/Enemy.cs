using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    public int wallDamage;
    public int hp = 4;
    private Animator animator;
    private Transform target;
    private bool skipMove;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    public AudioClip enemyChop1;
    public AudioClip enemyChop2;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        GameManager.instance.AddEnemyToList(this);
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    public void DamageEnemy(int damage) {
        hp -= damage;
        SoundManager.instance.RandomizeSfx(enemyChop1, enemyChop2);
        if (hp <= 0)
            gameObject.SetActive(false);
    }

    protected override void AttemptMove(int xDir, int yDir) {
        if (skipMove) {
            skipMove = false;
            return;
        }

        base.AttemptMove(xDir, yDir);
        skipMove = true;
        return;
    }

    public void MoveEnemy() {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        } else {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove(xDir, yDir);
    }

    protected override void OnCantMove (RaycastHit2D hit) {
        Player hitPlayer = hit.transform.GetComponent<Player>();
        Wall hitWall = hit.transform.GetComponent<Wall>();
        if (hitPlayer != null) {
            hitPlayer.LooseFood(playerDamage);
            animator.SetTrigger("enemyAttack");
            SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
            return;
        }

        if (hitWall != null) {
            hitWall.DamageWall(wallDamage);
            animator.SetTrigger("enemyAttack");
            SoundManager.instance.RandomizeSfx(enemyChop1, enemyChop2);
            return;
        }

        return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

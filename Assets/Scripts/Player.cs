﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public int enemyDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;
    private Vector2 touchOrigin = -Vector2.one;

    protected override void OnCantMove(RaycastHit2D hit)
    {
        Wall hitWall = hit.transform.GetComponent<Wall>();
        Enemy hitEnemy = hit.transform.GetComponent<Enemy>();

        if (hitWall != null)
        {
            hitWall.DamageWall(wallDamage);
            animator.SetTrigger("playerChop");
        }

        if (hitEnemy != null)
        {
            hitEnemy.DamageEnemy(enemyDamage);
            animator.SetTrigger("playerChop");
        }
        GameManager.instance.playersTurn = false;
        //isMoving = false;
        return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+ " + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+ " + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene("MinhaCena");
    }

    public void LooseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "- " + loss + " Food: " + food;
        CheckIfGameOver();
    }

    protected override void AttemptMove(int xDir, int yDir)
    {
        food--;
        base.AttemptMove(xDir, yDir);
        foodText.text = "Food: " + food;
        RaycastHit2D hit = checkCollision(xDir, yDir);
        if (hit.transform == null)
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();


    }

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;

        base.Start();
    }

    void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn) return;


        int horizontal = 0;
        int vertical = 0;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }

#else
        if (Input.touchCount > 0) {
            Touch myTouch = Input.touches[0];
            if (myTouch.phase == TouchPhase.Began) {
                touchOrigin = myTouch.position;

            } else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y)) {
                    horizontal = (x > 0)? 1 : -1;
                } else {
                    vertical = (y > 0)? 1 : -1;
                }
            }
        }
#endif
        if ((horizontal != 0 || vertical != 0) && !isMoving)
        {
            isMoving = true;
            AttemptMove(horizontal, vertical);
        }
    }

    void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            enabled = false;
            GameManager.instance.GameOver();
        }
    }
}

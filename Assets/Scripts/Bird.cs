using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JUMP_AMOUNT = 90f;
    private Rigidbody2D rb;
    private static Bird instance;
    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;
    private State state;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }

                transform.eulerAngles = new Vector3(0, 0, rb.velocity.y * 0.15f);
                break;
            case State.Dead:
                break;
        }

    }

    public static Bird GetInstance()
    {
        return instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("DEAD! " + collision.gameObject.name);
        if (state == State.Playing)
        {
            rb.bodyType = RigidbodyType2D.Static;
            state = State.Dead;
            SoundManager.PlaySound(SoundManager.Sound.Lose);
            if (OnDied != null) OnDied(this, EventArgs.Empty);
            Score.TrySetHighscore(Level.GetInstance().GetPipesPassedCount());
        }
    }

    private void Jump()
    {
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
        rb.velocity = Vector2.up * JUMP_AMOUNT;
    }
}

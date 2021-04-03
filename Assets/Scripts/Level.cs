using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 50f;
    private const float PIPE_DESTROY_POSITION_X = -100f;
    private const float PIPE_SPAWN_POSITION_X = 100f;
    private const float GROUND_DESTROY_POSITION_X = -185f;
    private const float GROUND_WIDTH = 280f;
    private const float CLOUD_DESTROY_POSITION_X = -160f;
    private const float CLOUD_SPAWN_POSITION_X = 160f;
    private const float CLOUD_SPAWN_POSITION_Y = 30f;
    private const float BIRD_POSITION_X = 0f;

    private static Level instance;
    private List<Pipe> pipeList;
    private List<Transform> groundList;
    private List<Transform> cloudsList;
    private int pipesSpawn;
    private int pipesPassedCount;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float cloudSpawnTimer;
    private float cloudSpawnTimerMax;
    private float gapSize;
    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead
    }

    void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        SpawnInitialGround();
        SpawnInitialClouds();
        state = State.WaitingToStart;
        pipesPassedCount = 0;
        cloudSpawnTimerMax = 6f;
        SetDifficulty(Difficulty.Easy);
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    void Update()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlingPipeSpawning();
            HandleGround();
            HandleClouds();
        }
    }

    public static Level GetInstance()
    {
        return instance;
    }

    public int GetPipesSwawned()
    {
        return pipesSpawn;
    }

    public int GetPipesPassedCount()
    {
        return pipesPassedCount;
    }

    private void Bird_OnDied(object sender, EventArgs args)
    {
        state = State.BirdDead;
    }

    private void Bird_OnStartedPlaying(object sender, EventArgs args)
    {
        state = State.Playing;
    }

    private Transform GetCloudPrefabTransform()
    {
        switch (UnityEngine.Random.Range(0, 3))
        {
            default:
            case 0: return GameAssets.GetInstance().pfCloud1.transform;
            case 1: return GameAssets.GetInstance().pfCloud2.transform;
            case 2: return GameAssets.GetInstance().pfCloud3.transform;
        }
    }

    private void SpawnInitialClouds()
    {
        cloudsList = new List<Transform>();
        Transform cloudTransform;
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, CLOUD_SPAWN_POSITION_Y, 0), Quaternion.identity);
        cloudsList.Add(cloudTransform);
    }

    private void SpawnInitialGround()
    {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundPositionY = -47f;
        float groundWidth = GROUND_WIDTH / 2;
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(0, groundPositionY, 0), Quaternion.identity);
        groundList.Add(groundTransform);

        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth, groundPositionY, 0), Quaternion.identity);
        groundList.Add(groundTransform);

        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth * 2f, groundPositionY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
    }

    private void HandleClouds()
    {
        cloudSpawnTimer -= Time.deltaTime;

        if (cloudSpawnTimer < 0)
        {
            cloudSpawnTimer = cloudSpawnTimerMax;
            Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(CLOUD_SPAWN_POSITION_X, CLOUD_SPAWN_POSITION_Y, 0), Quaternion.identity);
            cloudsList.Add(cloudTransform);
        }

        for (int i = 0; i < cloudsList.Count; i++)
        {
            Transform cloudTransform = cloudsList[i];
            cloudTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime * 0.7f;

            if (cloudTransform.position.x < CLOUD_DESTROY_POSITION_X)
            {
                Destroy(cloudTransform.gameObject);
                cloudsList.RemoveAt(i);
                i--;
            }
        }
    }

    private void HandleGround()
    {
        foreach (var groundTransform in groundList)
        {
            groundTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;

            if (groundTransform.position.x < GROUND_DESTROY_POSITION_X)
            {
                float rightMostPositionX = -100f;
                for (int i = 0; i < groundList.Count; i++)
                {
                    if (groundList[i].position.x > rightMostPositionX)
                    {
                        rightMostPositionX = groundList[i].position.x;
                    }
                }

                float groundWidth = GROUND_WIDTH / 2;
                groundTransform.position = new Vector3(rightMostPositionX + groundWidth, groundTransform.position.y, groundTransform.position.z);
            }
        }
    }

    private void HandlingPipeSpawning()
    {
        pipeSpawnTimer += Time.deltaTime;

        if (pipeSpawnTimer >= pipeSpawnTimerMax)
        {
            float heightEdgeLimit = 10;
            float totalHeight = CAMERA_ORTHO_SIZE * 2;
            float minHeight = gapSize / 2 + heightEdgeLimit;
            float maxHeight = totalHeight - gapSize / 2 - heightEdgeLimit;

            float height = UnityEngine.Random.Range(minHeight, maxHeight);
            CreateGapPipe(height, gapSize, PIPE_SPAWN_POSITION_X);
            pipeSpawnTimer = 0;
        }
    }

    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];

            bool isToTheRightOfBird = pipe.GetXPosition() > BIRD_POSITION_X;

            pipe.Move();

            if (isToTheRightOfBird && pipe.IsBottom() && pipe.GetXPosition() <= BIRD_POSITION_X)
            {
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }

            if (pipe.GetXPosition() < PIPE_DESTROY_POSITION_X)
            {
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void CreateGapPipe(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize / 2, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2 - gapY - gapSize / 2, xPosition, false);
        pipesSpawn++;
        SetDifficulty(GetDifficulty());
    }

    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;

        if (createBottom)
        {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT / 2;
        }
        else
        {
            pipeHeadYPosition = CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT / 2;
        }

        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);

        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;

        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPosition = CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }

        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRender = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRender.size = new Vector2(PIPE_WIDTH, height);
        BoxCollider2D pipeBodyBoxColider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxColider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxColider.offset = new Vector2(0f, height / 2);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);
        pipeList.Add(pipe);
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimerMax = 2.5f;
                break;
            case Difficulty.Medium:
                gapSize = 45f;
                pipeSpawnTimerMax = 2.3f;
                break;
            case Difficulty.Hard:
                gapSize = 40f;
                pipeSpawnTimerMax = 2.0f;
                break;
            case Difficulty.Impossible:
                gapSize = 35f;
                pipeSpawnTimerMax = 1.5f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipesSpawn >= 30) return Difficulty.Impossible;
        if (pipesSpawn >= 20) return Difficulty.Hard;
        if (pipesSpawn >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }

    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }

        public bool IsBottom()
        {
            return isBottom;
        }
    }
}

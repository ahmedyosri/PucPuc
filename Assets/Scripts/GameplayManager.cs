using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField]
    Transform primaryBallPosition;

    [SerializeField]
    Transform secondaryBallPosition;

    [SerializeField]
    public Transform ballCreationPosition;

    Transform entitiesParent;
    Transform pooledObjsParent;

    [SerializeField]
    GameObject ballPrefab;

    [SerializeField]
    Transform zeroIndex;

    [SerializeField]
    public float cellWidth;

    [SerializeField]
    public float cellLength;

    [SerializeField]
    LineRenderer aimLine;

    [SerializeField]
    Animator gameplayUiAnimator;

    public ParticleSystem explosionPS;

    public GameObject estimatedBall;

    public float ballFiringSpeed;

    public float ballMergeSpeed;

    [SerializeField]
    ColorsDictionary colorsDictionary;

    [SerializeField]
    ScoreController scoreController;

    public int CurrentScore
    {
        get
        {
            return scoreController.CurrentScore;
        }
    }

    public GameContext gameContext;

    public Vector2 ZeroPosition
    {
        get { return zeroIndex.transform.position; }
    }

    public Transform PrimaryBallPosition
    {
        get { return primaryBallPosition; }
    }

    public Transform SecondaryBallPosition
    {
        get { return secondaryBallPosition; }
    }

    public LineRenderer AimLine
    {
        get { return aimLine; }
    }

    public ColorsDictionary ColorsDic
    {
        get { return colorsDictionary; }
    }

    bool isGamePaused;

    public static GameplayManager Instance;

    private Systems systems;

    private Contexts contexts;

    private void Awake()
    {
        Instance = this;
    }

    Queue<GameObject> pooledObjects;

    // Start is called before the first frame update
    void Start()
    {
        scoreController.AddScore(0);

        isGamePaused = false;

        entitiesParent = new GameObject("EntitiesParent").transform;
        CreatePool();

        contexts = Contexts.sharedInstance;
        gameContext = contexts.game;
        systems = CreateSystems(contexts);
        systems.Initialize();

    }

    public void OnBoardCleared()
    {
        gameplayUiAnimator.SetTrigger("OnPerfect");
        AddScore(1000);
    }

    private void CreatePool()
    {
        pooledObjsParent = new GameObject("PooledObjects").transform;
        pooledObjsParent.position = Vector3.one * -10;
        pooledObjects = new Queue<GameObject>();

        for(int i=0; i<BoardManager.width * 8; i++)
        {
            pooledObjects.Enqueue(Instantiate(ballPrefab, pooledObjsParent) as GameObject);
        }
    }

    private Systems CreateSystems(Contexts contexts)
    {
        return new Feature("Systems")
            .Add(new GameplayFeature(contexts));
    }

    // Update is called once per frame
    void Update()
    {
        if (isGamePaused)
            return;

        if(Input.GetKeyUp(KeyCode.T))
        {
            gameContext.boardManager.entities[2, 3].isExploding = true;
        }

        systems.Execute();
        systems.Cleanup();
    }

    public GameObject CreateBall()
    {
        GameObject objRef = pooledObjects.Peek();
        pooledObjects.Dequeue();
        objRef.transform.SetParent(entitiesParent);
        return objRef;
    }

    public void DeleteBall(GameObject objRef)
    {
        objRef.transform.SetParent(pooledObjsParent);
        objRef.transform.localPosition = Vector3.zero;
        objRef.transform.rotation = Quaternion.identity;
        objRef.Unlink();
        objRef.GetComponent<Collider2D>().enabled = ballPrefab.GetComponent<Collider2D>().enabled;
        pooledObjects.Enqueue(objRef);
    }

    public void AddScore(float addedScore)
    {
        int tmpScore = (int) Mathf.Ceil(addedScore);
        scoreController.AddScore(tmpScore);
    }

    public void OnGamePaused()
    {
        isGamePaused = true;
    }

    public void OnGameResumed()
    {
        isGamePaused = false;
    }

    public void OnGameQuit()
    {
        Application.Quit();
    }
}

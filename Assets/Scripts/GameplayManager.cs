using System;
using System.Collections.Generic;
using Entitas;
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

    public float ballFiringSpeed;

    public float ballMergeSpeed;

    [SerializeField]
    ColorsDictionary colorsDictionary;

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
        entitiesParent = new GameObject("EntitiesParent").transform;
        CreatePool();

        contexts = Contexts.sharedInstance;
        gameContext = contexts.game;
        systems = CreateSystems(contexts);
        systems.Initialize();

    }

    private void CreatePool()
    {
        pooledObjsParent = new GameObject("PooledObjects").transform;
        pooledObjsParent.position = Vector3.one * -10;
        pooledObjects = new Queue<GameObject>();

        for(int i=0; i<62; i++)
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
        if(Input.GetKeyUp(KeyCode.T))
        {
            GameEntity[,] ents = gameContext.boardManager.entities;
            string deb = "";
            for (int y = 0; y < BoardManager.length; y++)
            {
                for (int x = 0; x < BoardManager.width; x++)
                {
                    deb += (ents[x, y] == null) ? "X" : ents[x,y].hasBoardBall ? ents[x, y].boardBall.value.ToString() : "?";
                    deb += " ";
                }
                deb += "\n";
            }
            Debug.Log(deb);
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
        objRef.GetComponent<Collider2D>().enabled = ballPrefab.GetComponent<Collider2D>().enabled;
    }
}

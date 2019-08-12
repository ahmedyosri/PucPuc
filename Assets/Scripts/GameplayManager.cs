using System;
using Entitas;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField]
    Transform primaryBallPosition;

    [SerializeField]
    Transform secondaryBallPosition;

    [SerializeField]
    Transform entitiesParent;

    [SerializeField]
    GameObject ballPrefab;

    [SerializeField]
    Transform zeroIndex;

    [SerializeField]
    public float cellWidth;

    [SerializeField]
    public float cellLength;

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

    public static GameplayManager Instance;

    private Systems systems;

    private Contexts contexts;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        contexts = Contexts.sharedInstance;
        systems = CreateSystems(contexts);
        systems.Initialize();

    }

    private Systems CreateSystems(Contexts contexts)
    {
        return new Feature("Systems")
            .Add(new GameplayFeature(contexts));
    }

    // Update is called once per frame
    void Update()
    {
        systems.Execute();
        systems.Cleanup();
    }

    public GameObject CreateBall()
    {
        return Instantiate(ballPrefab, entitiesParent) as GameObject;
    }
}

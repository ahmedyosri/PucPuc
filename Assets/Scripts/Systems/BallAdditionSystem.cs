using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallAdditionSystem : ReactiveSystem<GameEntity>
{
    GameContext gameContext;

    List<Vector2> neighborsOfShited = new List<Vector2>{
        new Vector2(-1, 0), new Vector2(1, 0),
        new Vector2(0, -1), new Vector2(1, -1),
        new Vector2(0, 1), new Vector2(1, 1)
    };

    List<Vector2> neighborsOfNotShited = new List<Vector2>{
        new Vector2(-1, 0), new Vector2(1, 0),
        new Vector2(-1, -1), new Vector2(0, -1),
        new Vector2(-1, 1), new Vector2(0, 1)
    };

    public BallAdditionSystem(Contexts contexts) : base(contexts.game)
    {
        gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.AddToBoard, GameMatcher.ReachedTarget));
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.isAddToBoard;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (GameEntity e in entities)
        {
            e.isMoving = false;
            e.isAddToBoard = false;
            e.ReplaceBoardBall(e.boardBall.boardIdx, e.boardBall.value, e.boardBall.shifted);
            gameContext.boardManager.entities[(int)e.boardBall.boardIdx.x, (int)e.boardBall.boardIdx.y] = e;

            ApplyImpact(e);

            List<GameEntity> cluster = GetCluster(e.boardBall.boardIdx, e.boardBall.value);

            if (cluster.Count == 1)
                continue;

            int newVal = e.boardBall.value + 1;
            GameEntity maxImpactEntity = null;
            int maxImpact = 0;

            foreach(GameEntity clusterEntity in cluster)
            {
                List<GameEntity> tmpCluster = GetCluster(clusterEntity.boardBall.boardIdx, newVal);
                if (tmpCluster.Count > maxImpact)
                {
                    maxImpact = tmpCluster.Count;
                    maxImpactEntity = clusterEntity;
                }
            }

            maxImpactEntity.ReplaceBoardBall(maxImpactEntity.boardBall.boardIdx, newVal, maxImpactEntity.boardBall.shifted);
            maxImpactEntity.isAddToBoard = true;

            foreach (GameEntity clusterEntity in cluster)
            {
                if (clusterEntity == maxImpactEntity)
                    continue;

                clusterEntity.AddMergeTo(maxImpactEntity);
                gameContext.boardManager.entities[(int)clusterEntity.boardBall.boardIdx.x, (int)clusterEntity.boardBall.boardIdx.y] = null;
            }

            HashSet<GameEntity> reachableEntities = new HashSet<GameEntity>();
            for(int x=0; x<BoardManager.width; x++)
            {
                if (gameContext.boardManager.entities[x, 0] == null)
                    continue;
                List<GameEntity> tmpReachableEntities = GetCluster(new Vector2(x, 0));
                foreach (GameEntity tmpEnt in tmpReachableEntities)
                    reachableEntities.Add(tmpEnt);
            }

            for(int x=0; x<BoardManager.width; x++)
            {
                for(int y=0; y<BoardManager.length; y++)
                {
                    GameEntity tmpEnt = gameContext.boardManager.entities[x, y];
                    if (tmpEnt == null)
                        continue;
                    if (reachableEntities.Contains(tmpEnt))
                        continue;

                    if (!tmpEnt.hasFall)
                        tmpEnt.AddFall(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 3), 0));
                }
            }

        }
    }

    void ApplyImpact(GameEntity e)
    {
        List<Vector2> nodeneighbors = e.boardBall.shifted ? neighborsOfShited : neighborsOfNotShited;
        foreach (Vector2 nodeShift in nodeneighbors)
        {
            Vector2 nodeIdx = e.boardBall.boardIdx + nodeShift;
            if (nodeIdx.x < 0 || nodeIdx.y < 0 || nodeIdx.x >= BoardManager.width || nodeIdx.y >= BoardManager.length)
            {
                continue;
            }

            GameEntity neighbor = gameContext.boardManager.entities[(int)nodeIdx.x, (int)nodeIdx.y];
            if (neighbor == null)
                continue;

            Vector3 pos = neighbor.position.pos;
            Vector3 newPos = pos - (Vector3)e.position.pos;
            newPos = newPos.normalized * 0.1f;
            neighbor.AddTargetPositions(1.0f, new List<Vector3>() { pos + newPos, pos });
            neighbor.isMoving = true;
        }
    }

    private List<GameEntity> GetCluster(Vector2 boardIdx, int val = -1)
    {
        HashSet<GameEntity> visited = new HashSet<GameEntity>();
        Queue<Vector2> toVisit = new Queue<Vector2>();

        toVisit.Enqueue(boardIdx);

        while (toVisit.Count > 0)
        {
            GameEntity currEntity = GetEntity(toVisit.Peek());
            toVisit.Dequeue();

            BoardBall curr = currEntity.boardBall;

            if (visited.Contains(currEntity))
                continue;

            visited.Add(currEntity);

            var neighborsList = curr.shifted ? neighborsOfShited : neighborsOfNotShited;

            foreach(Vector2 v in neighborsList)
            {
                Vector2 neighborIdx = curr.boardIdx + v;
                if (Mathf.Min(neighborIdx.x, neighborIdx.y) < 0 || Mathf.Max(neighborIdx.x, neighborIdx.y) >= BoardManager.width)
                    continue;
                GameEntity neighborEntity = GetEntity(neighborIdx);
                if (neighborEntity == null)
                    continue;
                if (val != -1 && neighborEntity.boardBall.value != val)
                    continue;
                if (toVisit.Contains(neighborIdx))
                    continue;

                toVisit.Enqueue(neighborIdx);
            }
        }

        return visited.ToList();
    }

    GameEntity GetEntity(Vector2 boardIdx)
    {
        return gameContext.boardManager.entities[(int)boardIdx.x, (int)boardIdx.y];
    }
}
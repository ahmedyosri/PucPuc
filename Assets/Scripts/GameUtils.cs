using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameUtils
{
    public static Vector3 WorldPosForBall(GameEntity e)
    {
        Vector3 res = WorldPosForIdxPos(e.boardBall.boardIdx);
        res.x += 0.5f * (e.boardBall.shifted ? GameplayManager.Instance.cellWidth : 0.0f);
        return res;
    }

    public static Vector2 WorldPosForIdxPos(Vector2 idxPos)
    {
        idxPos.x *= GameplayManager.Instance.cellWidth;
        idxPos.y *= GameplayManager.Instance.cellLength * -1;
        return GameplayManager.Instance.ZeroPosition + idxPos;
    }

    public static Color GetColor(int val)
    {
        return GameplayManager.Instance.ColorsDic.colors[val];
    }

    static List<Vector2> neighborsOfShited = new List<Vector2>{
        new Vector2(0, -1), new Vector2(1, -1),
        new Vector2(-1, 0), new Vector2(1, 0),
        new Vector2(0, 1), new Vector2(1, 1)
    };

    static List<Vector2> neighborsOfNotShited = new List<Vector2>{
        new Vector2(-1, -1), new Vector2(0, -1),
        new Vector2(-1, 0), new Vector2(1, 0),
        new Vector2(-1, 1), new Vector2(0, 1)
    };

    public static List<GameEntity> GetCluster(Vector2 boardIdx, int val = -1)
    {
        return GetCluster(GameplayManager.Instance.gameContext.boardManager.entities, boardIdx, val);
    }

    private static List<GameEntity> GetCluster(GameEntity[,] boardEntities, Vector2 boardIdx, int val = -1)
    {
        HashSet<GameEntity> visited = new HashSet<GameEntity>();
        Queue<Vector2> toVisit = new Queue<Vector2>();

        toVisit.Enqueue(boardIdx);

        while (toVisit.Count > 0)
        {
            GameEntity currEntity = GetEntity(boardEntities, toVisit.Peek());
            toVisit.Dequeue();

            BoardBall curr = currEntity.boardBall;

            if (visited.Contains(currEntity))
                continue;

            visited.Add(currEntity);

            var neighborsList = curr.shifted ? neighborsOfShited : neighborsOfNotShited;

            foreach (Vector2 v in neighborsList)
            {
                Vector2 neighborIdx = curr.boardIdx + v;
                if (Mathf.Min(neighborIdx.x, neighborIdx.y) < 0 || neighborIdx.x >= BoardManager.width || neighborIdx.y >= BoardManager.length)
                    continue;
                GameEntity neighborEntity = GetEntity(boardEntities, neighborIdx);
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

    static GameEntity GetEntity(GameEntity[,] boardEntities, Vector2 boardIdx)
    {
        return boardEntities[(int)boardIdx.x, (int)boardIdx.y];
    }

    public static List<Vector2> GetNeighborsFor(GameEntity e)
    {
        if (!e.hasBoardBall)
            return null;
        return e.boardBall.shifted ? neighborsOfShited : neighborsOfNotShited;
    }

    public static List<GameEntity> GetParentsFor(GameEntity[,] boardEntities, BoardBall b)
    {
        List<GameEntity> parents = new List<GameEntity>();
        List<Vector2> parentIdxs = b.shifted ? neighborsOfShited : neighborsOfNotShited;
        parentIdxs = parentIdxs.GetRange(0, 2);

        foreach (Vector2 v in parentIdxs)
        {
            int x = (int)(b.boardIdx.x + v.x);
            int y = (int)(b.boardIdx.y + v.y);

            if (y < 0 || y >= BoardManager.length || x < 0 || x >= BoardManager.width)
                continue;

            if (boardEntities[x, y] != null)
                parents.Add(boardEntities[x, y]);
        }

        return parents;
    }

    public static List<GameEntity> GetSiblingsFor(GameEntity[,] boardEntities, BoardBall b)
    {
        List<GameEntity> siblings = new List<GameEntity>();
        List<Vector2> siblingsIdxs = b.shifted ? neighborsOfShited : neighborsOfNotShited;
        siblingsIdxs = siblingsIdxs.GetRange(2, 2);

        foreach (Vector2 v in siblingsIdxs)
        {
            int x = (int)(b.boardIdx.x + v.x);
            int y = (int)(b.boardIdx.y + v.y);

            if (y < 0 || y >= BoardManager.length || x < 0 || x >= BoardManager.width)
                continue;

            if (boardEntities[x, y] != null)
                siblings.Add(boardEntities[x, y]);
        }

        return siblings;
    }

    public static List<GameEntity> GetChildrenFor(GameEntity[,] boardEntities, BoardBall b)
    {
        List<GameEntity> children = new List<GameEntity>();
        List<Vector2> childrenIdxs = b.shifted ? neighborsOfShited : neighborsOfNotShited;
        childrenIdxs = childrenIdxs.GetRange(4, 2);

        foreach (Vector2 v in childrenIdxs)
        {
            int x = (int)(b.boardIdx.x + v.x);
            int y = (int)(b.boardIdx.y + v.y);

            if (y < 0 || y >= BoardManager.length || x < 0 || x >= BoardManager.width)
                continue;

            if (boardEntities[x, y] != null)
                children.Add(boardEntities[x, y]);
        }

        return children;
    }

    public static void MergeNeighborsOf(GameEntity e)
    {
        // 1- Find cluster of the current ball e
        List<GameEntity> cluster = GetCluster(e.boardBall.boardIdx, e.boardBall.value);
        GameContext gameContext = GameplayManager.Instance.gameContext;

        // 2- If no cluster, return, nothing to do
        if (cluster.Count == 1)
            return;

        int newVal = e.boardBall.value + 1;
        GameEntity maxImpactEntity = null;
        int maxImpact = 0;

        List<GameEntity> qualifiedBalls = GameUtils.GetClusterHingeBalls(cluster);

        // 3- Find the entity that, if upgraded, will yield a new maximum cluster (considering the original shot ball e)
        foreach (GameEntity clusterEntity in qualifiedBalls)
        {
            List<GameEntity> tmpCluster = GetCluster(clusterEntity.boardBall.boardIdx, newVal);
            if (tmpCluster.Count > maxImpact)
            {
                maxImpact = tmpCluster.Count;
                maxImpactEntity = clusterEntity;
            }
        }

        // 4- If they are all equal, no consequtive merge, then make sure maxImpact entity is not e
        if(maxImpact == 1 && cluster.Count > 1)
        {
            foreach (GameEntity ent in cluster)
            {
                if (ent != e)
                {
                    maxImpactEntity = ent;
                    break;
                }
            }
        }

        GameplayManager.Instance.AddScore(
            cluster.Count
            * gameContext.boardManager.scoreMultiplier
            * Mathf.Pow(2, e.boardBall.value)
        );

        // 5- Update the maxImpactEntity with new value and mark it as adding to board
        maxImpactEntity.ReplaceBoardBall(maxImpactEntity.boardBall.boardIdx, newVal, maxImpactEntity.boardBall.shifted);
        maxImpactEntity.isReachedTarget = false;
        maxImpactEntity.isAddToBoard = true;

        // 6- Merge remaining balls in cluster to the maxImpactEntity 
        // 7- Remove their values from board
        foreach (GameEntity clusterEntity in cluster)
        {
            if (clusterEntity == maxImpactEntity)
                continue;

            clusterEntity.AddTargetPositions(GameplayManager.Instance.ballMergeSpeed, new List<Vector3>() { maxImpactEntity.position.pos });
            clusterEntity.AddMergeTo(maxImpactEntity);
            clusterEntity.isReachedTarget = false;
            clusterEntity.isMoving = true;
            clusterEntity.gameObject.gameobject.GetComponent<ParticleSystem>().Play();
            gameContext.boardManager.entities[(int)clusterEntity.boardBall.boardIdx.x, (int)clusterEntity.boardBall.boardIdx.y] = null;
            gameContext.boardManager.mergingEntitiesCount++;
        }

        if (newVal == 11)
        {
            maxImpactEntity.isExploding = true;
            return;
        }

        CheckForFallingEntities();

        gameContext.boardManager.scoreMultiplier++;
    }

    private static List<GameEntity> GetClusterHingeBalls(List<GameEntity> cluster)
    {
        HashSet<GameEntity> res = new HashSet<GameEntity>();

        GameEntity[,] newEntities = GameplayManager.Instance.gameContext.boardManager.entities.Clone() as GameEntity[,];
        foreach(var clusterEntity in cluster)
        {
            if (clusterEntity.boardBall.boardIdx.y == 0)
                res.Add(clusterEntity);
            newEntities[(int)clusterEntity.boardBall.boardIdx.x, (int)clusterEntity.boardBall.boardIdx.y] = null;
        }

        HashSet<GameEntity> reachableEntities = new HashSet<GameEntity>();
        for(int x=0; x<BoardManager.width; x++)
        {
            if (newEntities[x, 0] == null)
                continue;
            List<GameEntity> tmpCluster = GetCluster(newEntities, new Vector2(x, 0));
            foreach (GameEntity e in tmpCluster)
                reachableEntities.Add(e);
        }

        List<GameEntity> conns = new List<GameEntity>();
        foreach(var clusterEntity in cluster)
        {
            conns.Clear();
            conns.AddRange(GetParentsFor(newEntities, clusterEntity.boardBall));
            conns.AddRange(GetSiblingsFor(newEntities, clusterEntity.boardBall));

            foreach(var c in conns)
            {
                if (reachableEntities.Contains(c))
                {
                    res.Add(clusterEntity);
                    continue;
                }
            }
        }

        return res.ToList();
    }

    public static void CheckForFallingEntities()
    {
        GameContext gameContext = GameplayManager.Instance.gameContext;

        // 8- Find entities that are reachable from Ceiling and mark ALL OTHERS to fall
        HashSet<GameEntity> reachableEntities = new HashSet<GameEntity>();
        for (int x = 0; x < BoardManager.width; x++)
        {
            if (gameContext.boardManager.entities[x, 0] == null)
                continue;
            List<GameEntity> tmpReachableEntities = GetCluster(new Vector2(x, 0));
            foreach (GameEntity tmpEnt in tmpReachableEntities)
                reachableEntities.Add(tmpEnt);
        }

        int fallingScore = 0;

        for (int x = 0; x < BoardManager.width; x++)
        {
            for (int y = 0; y < BoardManager.length; y++)
            {
                GameEntity tmpEnt = gameContext.boardManager.entities[x, y];
                if (tmpEnt == null)
                    continue;
                if (reachableEntities.Contains(tmpEnt))
                    continue;
                if (tmpEnt.hasFall)
                    continue;
                fallingScore += (int)Mathf.Pow(2, tmpEnt.boardBall.value);
                gameContext.boardManager.entities[x, y] = null;
                tmpEnt.isAddToBoard = false;
                tmpEnt.AddFall(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 3), 0), 1);
            }
        }

        GameplayManager.Instance.AddScore(
            fallingScore
            * gameContext.boardManager.scoreMultiplier
        );
    }

    public static bool IsRowShifted(int idx)
    {
        if (idx >= BoardManager.length || idx < 0)
        {
            Debug.LogError("Invalid index");
            return false;
        }

        GameEntity[,] boardEnts = GameplayManager.Instance.gameContext.boardManager.entities;
        for (int x = 0; x < BoardManager.width; x++)
        {
            if (boardEnts[x, idx] == null)
                continue;
            return boardEnts[x, idx].boardBall.shifted;
        }
        return false;
    }
}

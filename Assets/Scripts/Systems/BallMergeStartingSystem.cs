using Entitas;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallMergeStartingSystem : IExecuteSystem
{

    GameContext gameContext;
    bool startMerge, oldStartMerge;
    IGroup<GameEntity> impactingEntities;

    public BallMergeStartingSystem(Contexts contexts)
    {
        gameContext = contexts.game;
        startMerge = oldStartMerge = false;
    }

    public void Execute()
    {
        impactingEntities = gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Impacting, GameMatcher.ReachedTarget));

        if (impactingEntities.count == 0)
        {
            return;
        }

        if (impactingEntities.count != gameContext.boardManager.impactingEntitesCount)
        {
            return;
        }

        List<GameEntity> eList = impactingEntities.GetEntities().ToList();
        foreach(var entity in eList)
        {
            entity.isImpacting = false;
        }

        gameContext.boardManager.impactingEntitesCount = 0;

        GameEntity e = gameContext.GetGroup(GameMatcher.AddToBoard).GetEntities()[0];

        e.isAddToBoard = false;

        List<GameEntity> cluster = GameUtils.GetCluster(e.boardBall.boardIdx, e.boardBall.value);

        if (cluster.Count == 1)
            return;

        int newVal = e.boardBall.value + 1;
        GameEntity maxImpactEntity = null;
        int maxImpact = 0;

        foreach (GameEntity clusterEntity in cluster)
        {
            List<GameEntity> tmpCluster = GameUtils.GetCluster(clusterEntity.boardBall.boardIdx, newVal);
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

            clusterEntity.isImpacting = false;
            clusterEntity.AddTargetPositions(30, new List<Vector3>() { maxImpactEntity.position.pos });
            //clusterEntity.AddMergeTo(maxImpactEntity);
            gameContext.boardManager.entities[(int)clusterEntity.boardBall.boardIdx.x, (int)clusterEntity.boardBall.boardIdx.y] = null;
            gameContext.boardManager.mergingEntitiesCount++;
        }

        HashSet<GameEntity> reachableEntities = new HashSet<GameEntity>();
        for (int x = 0; x < BoardManager.width; x++)
        {
            if (gameContext.boardManager.entities[x, 0] == null)
                continue;
            List<GameEntity> tmpReachableEntities = GameUtils.GetCluster(new Vector2(x, 0));
            foreach (GameEntity tmpEnt in tmpReachableEntities)
                reachableEntities.Add(tmpEnt);
        }

        for (int x = 0; x < BoardManager.width; x++)
        {
            for (int y = 0; y < BoardManager.length; y++)
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

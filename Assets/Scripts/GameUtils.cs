using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        new Vector2(-1, 0), new Vector2(1, 0),
        new Vector2(0, -1), new Vector2(1, -1),
        new Vector2(0, 1), new Vector2(1, 1)
    };

    static List<Vector2> neighborsOfNotShited = new List<Vector2>{
        new Vector2(-1, 0), new Vector2(1, 0),
        new Vector2(-1, -1), new Vector2(0, -1),
        new Vector2(-1, 1), new Vector2(0, 1)
    };

    public static List<GameEntity> GetCluster(Vector2 boardIdx, int val = -1)
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

            foreach (Vector2 v in neighborsList)
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

    static GameEntity GetEntity(Vector2 boardIdx)
    {
        return GameplayManager.Instance.gameContext.boardManager.entities[(int)boardIdx.x, (int)boardIdx.y];
    }

    public static List<Vector2> GetNeighborsFor(GameEntity e)
    {
        if (!e.hasBoardBall)
            return null;
        return e.boardBall.shifted ? neighborsOfShited : neighborsOfNotShited;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Node
{
    public Node(MapCoordinate m, int h, int g, Maybe<Node> previous)
    {
        manhattan = h;
        cost = g;
        score = h + g;

        coordinates = m;
        cameFrom = previous;
    }
    public int manhattan;
    public int cost;
    public int score;

    public MapCoordinate coordinates;
    public Maybe<Node> cameFrom;
}

public static class Pathfinding
{

    public static List<MapCoordinate> FindRoute(MapCoordinate target, MapCoordinate origin, bool canPassDestroyedTiles)
    {
        List<MapCoordinate> result = new List<MapCoordinate>();

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        int widthTo, heightTo;
        MapManager.GetAbsoluteTileCountTo(target, origin, out widthTo, out heightTo);

        openList.Add(new Node(origin, widthTo + heightTo, 0, new Maybe<Node>()));
        Node currentBest = openList[0];
        int bestScore = openList[0].score;
        bool foundPath = false;

        while(openList.Count > 0)
        {
            bool foundBetter = false;
            openList.Remove(currentBest);
            closedList.Add(currentBest);
            List<MapCoordinate> bestNeighbours = MapManager.GetMap().GetTileNeighbors(currentBest.coordinates);

            //Giving a quick shuffle so it's not biased to always going down, then across
            for (int i = 0; i < bestNeighbours.Count; i++)
            {
                MapCoordinate temp = bestNeighbours[i];
                int randomIndex = Random.Range(i, bestNeighbours.Count);
                bestNeighbours[i] = bestNeighbours[randomIndex];
                bestNeighbours[randomIndex] = temp;
            }
            //In theory, anyway... I'm not sure this is proving enough to get more unpredictable movement.

            foreach (MapCoordinate newCoord in bestNeighbours)
            {
                if(newCoord == target)
                {
                    foundPath = true;
                    break;
                }

                bool inOpen = openList.Any(node => node.coordinates == newCoord);
                bool inClosed = closedList.Any(node => node.coordinates == newCoord);
                var mtp = MapManager.GetMap().GetTileProperties(newCoord);
                bool isPassable = (mtp.tile_enitity.is_some == false
                    && (canPassDestroyedTiles || mtp.Integrity > 0));

                if (inOpen || inClosed || isPassable == false) continue;

                int distance, cost;
                MapManager.GetAbsoluteTileCountTo(origin, newCoord, out widthTo, out heightTo);
                distance = widthTo + heightTo;
                MapManager.GetAbsoluteTileCountTo(newCoord, target, out widthTo, out heightTo);
                cost = widthTo + heightTo;

                Node newNode = new Node(newCoord, distance, cost, new Maybe<Node>(currentBest));

                if(newNode.score < bestScore)
                {
                    currentBest = newNode;
                    foundBetter = true;
                }
                openList.Add(newNode);
            }
            if (foundPath) break;
            if (!foundBetter)
            {
                int nextBest = int.MaxValue;
                foreach(Node node in openList)
                {
                    if(node.score < nextBest)
                    {
                        currentBest = node;
                        nextBest = node.score;
                    }
                }
            }
        }
        Node nextNode = currentBest;
        while(nextNode.coordinates != target)
        {
            result.Add(nextNode.coordinates);
            if(nextNode.cameFrom.is_some)
            {
                nextNode = nextNode.cameFrom.value;
                if (nextNode.coordinates == origin) break;
            }
            else
            {
                throw new System.Exception("There was a null cameFrom in the path back from the pathfinding target! THIS SHOULD NOT HAPPEN");
            }
        }


        return result;
    }
}
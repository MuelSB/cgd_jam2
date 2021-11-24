﻿using System.Collections;
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

    public static List<MapCoordinate> FindRoute(MapCoordinate target, MapCoordinate origin)
    {
        List<MapCoordinate> result = new List<MapCoordinate>();

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        int widthTo, heightTo;
        MapManager.GetAbsoluteTileCountTo(target, origin, out widthTo, out heightTo);

        openList.Add(new Node(target, widthTo + heightTo, 0, new Maybe<Node>()));
        Node currentBest = openList[0];
        int bestScore = openList[0].score;
        bool foundPath = false;

        while(openList.Count > 0)
        {
            bool foundBetter = false;
            openList.Remove(currentBest);
            closedList.Add(currentBest);
            List<MapCoordinate> bestNeighbours = MapManager.GetMap().GetTileNeighbors(currentBest.coordinates);
            foreach(MapCoordinate newCoord in bestNeighbours)
            {
                if(newCoord == origin)
                {
                    foundPath = true;
                    break;
                }

                bool inOpen = openList.Any(node => node.coordinates == newCoord);
                bool inClosed = closedList.Any(node => node.coordinates == newCoord);

                if (inOpen || inClosed) continue;

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

        if(foundPath)
        {
            Node nextNode = currentBest;
            while(nextNode.coordinates != target)
            {
                result.Add(nextNode.coordinates);
                if(nextNode.cameFrom.is_some)
                {
                    nextNode = nextNode.cameFrom.value;
                }
                else
                {
                    throw new System.Exception("There was a null cameFrom in the path back from the pathfinding target! THIS SHOULD NOT HAPPEN");
                }
            }
            result.Add(target);
        }

        return result;
    }
}
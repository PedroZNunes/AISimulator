﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// expands by visiting each branch in order. it is very slow
/// </summary>
public class BFS : GraphSearchAlgorithm {

	public override IEnumerator Search (List<Node> nodes, int size, Node start, Node goal, int framesPerSecond) {
        IsSearching = true;
        Debug.Log ("Searching path using Breadth First Search algorythm.");
        bool hasPath = false;

        bool[,] visited = new bool[size , size];
        cameFrom = new Dictionary<Node , Node> ();
        Queue<Node> frontier = new Queue<Node> ();
        Node current = new Node();

        frontier.Enqueue (start);
        cameFrom[start] = null;
        visited[(int) start.pos.x, (int) start.pos.y] = true;

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();

            UIUpdate (frontier.Count, CalculatePathLength (start, current));

            //visualize path to current
            GraphSearcher.VisualizePath (cameFrom, current , start);
            yield return new WaitForSeconds (1f/framesPerSecond); 

            if (current == goal) {
                hasPath = true;
                break;
            }

            for (int i = 0 ; i < current.links.Count ; i++) {
                Node neighbour = current.links[i];

                if (neighbour == current)
                    Debug.LogErrorFormat ("node {0} has link to itself.", current);

                if (!visited[(int) neighbour.pos.x , (int) neighbour.pos.y]) {
                    visited[(int) neighbour.pos.x , (int) neighbour.pos.y] = true;

                    cameFrom[neighbour] = current;
                    frontier.Enqueue (neighbour);
                    UIIncrementEnqueuings ();
                }

            }
        }

        SearchComplete(hasPath);
        yield break;
    }

}

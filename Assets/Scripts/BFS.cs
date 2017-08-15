using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class BFS : SearchAlgorythm {

	public override Queue<Node> Search (List<Node> nodes, int size, Node start, Node goal, bool trackVisitedNodes) {
        Debug.Log ("Searching path using BFS.");
        bool hasPath = false;

        bool[,] visited = new bool[size , size];
        Dictionary<Node , Node> cameFrom = new Dictionary<Node , Node> ();
        Queue<Node> frontier = new Queue<Node> ();
        Node current = new Node();

        frontier.Enqueue (start);
        cameFrom[start] = null;

        while (frontier.Count > 0) {
            current = frontier.Dequeue ();

            if (current == goal) {
                hasPath = true;
                break;
            }

            for (int i = 0 ; i < current.links.Count ; i++) {
                Node neighbour = current.links[i];
                
                if (trackVisitedNodes) {
                    if (!visited[(int) neighbour.pos.x , (int) neighbour.pos.y]) {
                        visited[(int) neighbour.pos.x , (int) neighbour.pos.y] = true;

                        cameFrom[neighbour] = current;
                        frontier.Enqueue (neighbour);
                    }
                }
                else {
                    cameFrom[neighbour] = current;
                    frontier.Enqueue (neighbour);
                }
            }
        }

        if (!hasPath) {
            Debug.Log ("Path Not Found.");
            return null;
        }

        Stack<Node> path = new Stack<Node> ();
        
        current = goal;
        path.Push (current);

        while (current != start) {
            current = cameFrom[current];
            path.Push(current);
        }

        StringBuilder sb = new StringBuilder ();
        while (path.Count > 0) {
            sb.AppendFormat ("{0} ", path.Pop ().ID);
        }
        
        Debug.LogFormat ("Returning path: \n {0}", sb);
        return null;
    }


}

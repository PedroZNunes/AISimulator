using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeSearcher : MonoBehaviour {

    public delegate void TreeUpdatedHandler (GamesNode[] leafs);
    static public event TreeUpdatedHandler TreeUpdated;

    private GamesAlgorythm algorythm;

    static private TreeSearcher instance;

    private void Awake () {
        instance = FindObjectOfType<TreeSearcher> ();
    }

    private void OnEnable () {
        UIGames.SearchEvent += StartSearching;
        GamesAlgorythm.NodeAnalyzedEvent += SetLeafStates;
    }
    private void OnDisable () {
        UIGames.SearchEvent -= StartSearching;
        GamesAlgorythm.NodeAnalyzedEvent -= SetLeafStates;
    }

    public void StartSearching (string algorythm, int branching, int depth, int framesPerSecond) {
        if (algorythm != null) {
            switch (algorythm) {
                case ("Minimax"):
                    this.algorythm = new Minimax ();
                    break;

                case ("Alpha - Beta"):
                    this.algorythm = new AlphaBeta ();
                    break;

                default:
                    this.algorythm = new AlphaBeta ();
                    break;
            }

            if (this.algorythm != null) {
                if (!GamesAlgorythm.IsSearching) {
                    Debug.LogFormat ("Reading Tree using {0}.", algorythm);
                    this.algorythm.Search (TreeGenerator.Root, branching, depth, framesPerSecond);
                }
                else {
                    Debug.LogWarning ("Another search in progress.");
                }
            }
        }
        else
            Debug.LogWarning ("Algorythm not set. Search canceled.");
    }

    public void SetAlgorythm (GamesAlgorythm algorythm) {
        this.algorythm = algorythm;
    }

    private void SetLeafStates (GamesNode node) {
        GamesNode[] leafs = GamesNode.Leafs.ToArray ();
        for (int i = 0 ; i < leafs.Length ; i++) {

            if (leafs[i].ID == node.ID) {
                leafs[i].SetState (NodeState.Active);
            }
            else if (leafs[i].nodeState == NodeState.Explored) {
                continue;
            }
            else if (leafs[i].nodeState == NodeState.Active) {
                leafs[i].SetState (NodeState.Explored);
            }
            else if (leafs[i].ID < node.ID) {
                leafs[i].SetState (NodeState.Pruned);
            }
            else {
                leafs[i].SetState (NodeState.Inactive);
            }
        }

        if (TreeUpdated != null) {
            TreeUpdated (leafs);
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeGenerator : MonoBehaviour {

    public delegate void SetCameraHandler (int depth, int branch, float stepY);
    static public event SetCameraHandler SetCameraEvent;

    [SerializeField]
    private GameObject minPrefab;

    [SerializeField]
    private GameObject maxPrefab;

    [SerializeField]
    private GameObject leafPrefab;

    [SerializeField]
    private GameObject linkPrefab;

    private float stepY;

    private int branching;
    private int treeDepth;

    static public GamesNode Root { get; private set; }

    //events
    private void OnEnable () {
        UIGames.GenerateMapEvent += Generate;
    }
    private void OnDisable () {
        UIGames.GenerateMapEvent -= Generate;
    }

    private void Generate (int branching, int depth, int grain) {
        Debug.Log ("Generating map.");
        grain = 0;
        this.branching = branching;
        this.treeDepth = depth;

        NodeType nodeType = NodeType.Max;

        Root = new GamesNode (branching, depth, 0, nodeType, null);

        VisualizeTree ();
    }

    private void VisualizeTree () {
        //spawn the first node
        GameObject go = (GameObject) Instantiate (maxPrefab, Vector2.zero, Quaternion.identity, this.transform);
        go.name = Root.nodeType.ToString () + " " + Root.ID;
        Root.GO = go;
        //spawn the nodes linked to it
        SpawnLinksOf (Root);

        if (SetCameraEvent != null) {
            SetCameraEvent (treeDepth, branching, stepY);
        }
    }

    private void SpawnLinksOf (GamesNode parentNode) {
        if (parentNode.depth == treeDepth)
            return;

        //spawn node with position relative to parent.
        for (int i = 0 ; i < branching ; i++) {
            GamesNode toSpawn = parentNode.links[i].GetOther (parentNode);
            //calculate position
            Vector2 step = new Vector2 ();
            step.x = Mathf.Pow (branching, treeDepth - toSpawn.depth);
            step.y = stepY = branching + (branching * branching / treeDepth) ;

            float initialPosX = parentNode.GO.transform.position.x - ((branching - 1) * step.x / 2);
            float posY = parentNode.GO.transform.position.y - step.y;
            Vector2 pos = new Vector2 (initialPosX + (step.x * i) , posY) ;
            //Debug.LogFormat ("initialposX: {0}. stepX: {1}. pos{2}", initialPosX, step.x, pos);

            //assign prefab according to node type (leaf, min or max)
            Object prefab;
            if (treeDepth == toSpawn.depth)
                prefab = leafPrefab;
            else if (toSpawn.nodeType == NodeType.Max)
                prefab = maxPrefab;
            else 
                prefab = minPrefab;

            //instantiate the node
            GameObject go = (GameObject) Instantiate (prefab, pos, Quaternion.identity, this.transform);
            go.name = toSpawn.nodeType.ToString () + " " + toSpawn.ID;
            toSpawn.GO = go;

            if (prefab == leafPrefab) {
                go.GetComponent<UITreeNode> ().AssignValue (toSpawn.value);
            }

            //prepare the link
            //posição
            pos = (toSpawn.GO.transform.position + parentNode.GO.transform.position) / 2;
            //angle
            double angle = Mathf.Atan2 (toSpawn.GO.transform.position.y - parentNode.GO.transform.position.y, toSpawn.GO.transform.position.x - parentNode.GO.transform.position.x) * Mathf.Rad2Deg + 90;
            //scale
            float scale = Vector2.Distance (parentNode.GO.transform.position, toSpawn.GO.transform.position);

            //spawn the link
            go = Instantiate (linkPrefab, pos, Quaternion.identity, transform);
            go.transform.localScale = new Vector3 (1f, scale * 4, 1f);
            go.transform.eulerAngles = new Vector3 (0f, 0f, (float) angle);

            parentNode.links[i].go = go;

            //then spawn the nodes linked to each linked node
            SpawnLinksOf (toSpawn);
        }
    }

}

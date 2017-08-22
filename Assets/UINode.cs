using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class UINode : MonoBehaviour {

    public delegate void NodeSelectedHandler (Node node);
    static public event NodeSelectedHandler NodeSelectedEvent;

    public Node node;

    private BoxCollider2D col;

    private void Awake () {
        col = GetComponent<BoxCollider2D> ();
        col.enabled = false;
    }

    private void OnEnable () {
        UIManager.EnableNodesEvent += EnableArea;
        UIManager.DisableNodesEvent += DisableArea;
    }
    private void OnDisable () {
        UIManager.EnableNodesEvent -= EnableArea;
        UIManager.DisableNodesEvent -= DisableArea;
    }

    private void OnMouseDown () {
        if (Input.GetKey (KeyCode.Mouse0)) {
            if (col.enabled) {
                NodeSelected ();
            } else {
                Debug.Log ("On mouse down triggered without collider");
            }
        }
    }

    public void EnableArea () {
        col.enabled = true;
    }

    private void DisableArea () {
        col.enabled = false;
    }

    public void NodeSelected () {
        if (NodeSelectedEvent != null)
            NodeSelectedEvent (node);
    }

}

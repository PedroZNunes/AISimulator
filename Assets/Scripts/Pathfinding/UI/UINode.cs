using UnityEngine;

[RequireComponent (typeof (BoxCollider2D), typeof (SpriteRenderer))]
public class UINode : MonoBehaviour {

    public delegate void NodeSelectedHandler (Node node);
    static public event NodeSelectedHandler NodeSelectedEvent;

    public Node node;

    [SerializeField]
    private Sprite inactiveNode;

    private BoxCollider2D col;
    private SpriteRenderer spriteRenderer;

    private void OnEnable () {
        UIManager.EnableNodesEvent += EnableArea;
        UIManager.DisableNodesEvent += DisableArea;
    }
    private void OnDisable () {
        UIManager.EnableNodesEvent -= EnableArea;
        UIManager.DisableNodesEvent -= DisableArea;
    }

    private void Awake () {
        col = GetComponent<BoxCollider2D> ();
        col.enabled = false;

        spriteRenderer = GetComponent<SpriteRenderer> ();
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

    public void ResetSprite () {
        spriteRenderer.sprite = inactiveNode;
    }

}

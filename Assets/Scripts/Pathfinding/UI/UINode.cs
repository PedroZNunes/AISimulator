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
        UINodeSetup.SettingUpNodesEvent += EnableArea;
        UINodeSetup.NodeSelectedEvent += DisableArea;
    }
    private void OnDisable () {
        UINodeSetup.SettingUpNodesEvent -= EnableArea;
        UINodeSetup.NodeSelectedEvent -= DisableArea;
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

    public void EnableArea (bool isStart) {
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

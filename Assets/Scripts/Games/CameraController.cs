using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float border;

    [SerializeField]
    private float minSize = 2;

    private Bounds area;

    
    private float panSensitivity = 2f;
    private bool isPanning = false;
    private Vector3 origCamPos;
    private Vector3 origMousePos;

    static private CameraController instance;

    private void OnEnable () {
        TreeGenerator.treeGenerated += SetCamera;
    }
    private void OnDisable () {
        TreeGenerator.treeGenerated -= SetCamera;
    }

    private void Awake () {
        if (instance == null)
            instance = FindObjectOfType<CameraController> ();
        if (instance != this)
            Destroy (this.gameObject);
    }

    private void SetCamera (int depth, int branching, float stepY) {
        float totalLeafs = Mathf.Pow (branching, depth);

        float sizeY = ((stepY * depth) + 1) / 2;

        float sizeX = ((totalLeafs + 1) / cam.aspect) / 2;

        cam.orthographicSize = Mathf.Max (sizeX, sizeY) + border;
        
        float posY = TreeNode.Leaves[0].GO.transform.position.y / 2;
        cam.transform.position = new Vector3 (0f, posY, cam.transform.position.z);

        area = new Bounds (cam.transform.position, new Vector3 (2 * cam.orthographicSize * cam.aspect, 2 * cam.orthographicSize));

        //setup the collider size for checking mouse interactions properly
        BoxCollider2D clickArea= this.GetComponent<BoxCollider2D>();
        if (clickArea != null) {
            clickArea.size = new Vector2(2 * cam.orthographicSize * cam.aspect, 2 * cam.orthographicSize);
            clickArea.offset = new Vector2 (0, posY);
        }

    }

    private void OnDrawGizmos () {
        Gizmos.DrawCube (area.center, area.size);
    }

    private void Update () {
        //zooming
        CheckMovement();
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            isPanning = true;
            origCamPos = cam.transform.position;
            origMousePos = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            float wheel = Mathf.Abs(Input.GetAxis("Mouse ScrollWheel"));
            //store the mouse pos in world coords.
            Vector3 origMousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            //Zoom
            if (Input.GetAxis("Mouse ScrollWheel") < 0) //zoom out
                cam.orthographicSize = Mathf.Min(cam.orthographicSize + cam.orthographicSize * wheel, area.extents.y);
            else if (Input.GetAxis("Mouse ScrollWheel") > 0) //zoom in
                cam.orthographicSize = Mathf.Max(Camera.main.orthographicSize - cam.orthographicSize * wheel, minSize);

            //Get the new Viewport coordinates for the new position of the object.
            Vector3 finalMousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            //move the camera by the difference between the two points
            Vector3 targetPos = cam.transform.position + (origMousePos - finalMousePos);

            //make sure the camera does not leave the bounds set by the first camera generation
            cam.transform.position = ClampMovement(targetPos);
        }


    }

    
    private void OnMouseUp() {
        isPanning = false;
    }


    private void CheckMovement() {

        if (Input.GetMouseButton(0)) {
            if (isPanning) {
                Vector3 dragMousePos = cam.ScreenToViewportPoint(Input.mousePosition);

                Vector3 displacement = ((dragMousePos - origMousePos) * panSensitivity * cam.orthographicSize);
                displacement.x *= cam.aspect;

                cam.transform.position = ClampMovement(origCamPos - displacement);
                print(displacement.ToString());
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            //leave
            isPanning = false;
            origCamPos = cam.transform.position;
        }


    }

    private Vector3 ClampMovement (Vector3 movePos) {
        Vector3 pos = new Vector3 ();
        //pos.x = Mathf.Clamp (movePos.x, area.min.x + cam.orthographicSize / 3 * 4, area.max.x - cam.orthographicSize / 3 * 4);
        pos.x = Mathf.Clamp (movePos.x, area.min.x + cam.orthographicSize * cam.aspect, area.max.x - cam.orthographicSize * cam.aspect);
        pos.y = Mathf.Clamp (movePos.y, area.min.y + cam.orthographicSize, area.max.y - cam.orthographicSize);
        pos.z = movePos.z;
        return pos;
    }

}

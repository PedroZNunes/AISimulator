using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private Camera cam;

    /// <summary>
    /// border around the grid in the camera space to get away from the very boundaries of the screen
    /// </summary>
    [SerializeField] private float border;

    /// <summary>
    /// minimum camera size for when zooming in
    /// </summary>
    [SerializeField] private float minSize = 2;

    /// <summary>
    /// Area wih content that the camera should stick to
    /// </summary>
    private Bounds area;

    
    private float panSensitivity = 2f;
    private bool isPanning = false;

    /// <summary>
    /// original camera position
    /// </summary>
    private Vector3 origCamPos;

    /// <summary>
    /// original mouse position
    /// </summary>
    private Vector3 origMousePos;

    /// <summary>
    /// this instance of the camera controller
    /// </summary>
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
        
    /// <summary>
    /// Sets up the camera
    /// </summary>
    /// <param name="depth">How deep goes the tree</param>
    /// <param name="branching">How wide goes the tree</param>
    private void SetCamera (int depth, int branching) {
        //Measure camera horizontal space according to the number of leafs because the spacing between them is always 1
        float totalLeafs = Mathf.Pow (branching, depth);
        float camSizeX = (totalLeafs + 1);

        //Measure camera vertical space with this formula that accounts for the variable spacing between rows according to the depth that row is in
        float camSizeY = 0;
        for (int n = 0; n < depth; n++) {
            camSizeY += 1 + ((branching * (depth - n)) * (1 / Camera.main.aspect));
        }
        
        //Positions the camera
        float posY = TreeNode.Leaves[0].GO.transform.position.y / 2;
        cam.transform.position = new Vector3 (0f, posY, cam.transform.position.z);

        //Setup a bound area so that the camera knows its limits
        Vector3 areaSize = new Vector3(camSizeX, camSizeY, 0f );
        area = new Bounds (cam.transform.position, areaSize);

        //Use the area's aspect ratio to set the camera's size
        float areaAspect = areaSize.x / areaSize.y;
        if (areaAspect > cam.aspect) {
            cam.orthographicSize = (areaSize.x / cam.aspect)/ 2;
        }
        else {
            cam.orthographicSize = (areaSize.y) / 2;
        }
            
        //Setup the collider size for checking mouse clicking and dragging
        BoxCollider2D clickArea = this.GetComponent<BoxCollider2D>();
        if (clickArea != null) {
            clickArea.size = new Vector2(2 * cam.orthographicSize * cam.aspect, 2 * cam.orthographicSize);
            clickArea.offset = new Vector2 (0, posY);
        }

    }


    private void OnDrawGizmos () {
        Gizmos.DrawCube (area.center, area.size);
    }

    private void Update () {
        //zooming and panning
        CheckMovement();
    }


    private void OnMouseOver() {
        //Panning
        if (Input.GetMouseButtonDown(0)) {
            isPanning = true;
            origCamPos = cam.transform.position;
            origMousePos = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        // Zooming
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            float wheel = Mathf.Abs(Input.GetAxis("Mouse ScrollWheel"));
            //store the mouse pos in world coords.
            Vector3 origMousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetAxis( "Mouse ScrollWheel" ) < 0) { //zoom out
                float maxCamSize = Mathf.Max( area.extents.y, area.extents.x / cam.aspect );
                cam.orthographicSize = Mathf.Min( cam.orthographicSize + cam.orthographicSize * wheel, maxCamSize);
            }
            else if (Input.GetAxis( "Mouse ScrollWheel" ) > 0) //zoom in
                cam.orthographicSize = Mathf.Max( Camera.main.orthographicSize - cam.orthographicSize * wheel, minSize );

            //Get the new World coordinates for the new mouse position.
            Vector3 finalMousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            //move the camera by the difference between the two points
            Vector3 targetPos = cam.transform.position + (origMousePos - finalMousePos);

            //make sure the camera does not leave the bounds set by the first camera generation
            cam.transform.position = ClampMovement(targetPos);
        }

    }

    
    private void OnMouseUp() {
        //Stops panning movements
        isPanning = false;
    }

    /// <summary>
    /// Drags and Zooms the camera around
    /// </summary>
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

    /// <summary>
    /// Makes sure the camera won't go beyond the boundaries when zooming and panning
    /// </summary>
    /// <param name="movePos"> Position you are trying to move the camera to </param>
    /// <returns>Camera position after being clamped to the boundaries</returns>
    private Vector3 ClampMovement (Vector3 movePos) {
        //se a camera for maior que a area, nao deve se mover em y
        Vector3 pos = new Vector3 ();
        if (cam.orthographicSize < area.extents.y) {
            pos.y = Mathf.Clamp (movePos.y, area.min.y + cam.orthographicSize, area.max.y - cam.orthographicSize);
        }
        else {
            pos.y = area.center.y;
        }

        if (cam.orthographicSize * cam.aspect < area.extents.x) {
            pos.x = Mathf.Clamp (movePos.x, area.min.x + cam.orthographicSize * cam.aspect, area.max.x - cam.orthographicSize * cam.aspect);
        }
        else {
            pos.x = area.center.x;
        }
        pos.z = movePos.z;
        return pos;
    }

}

using UnityEditorInternal;
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
        float camSizeX = (totalLeafs + 1);

        float treeSizeY = 0;
        for (int n = 0; n < depth; n++) {
            treeSizeY += 1 + ((branching * (depth - n)) * (1 / Camera.main.aspect));
        }
        float camSizeY = (treeSizeY);
        
        
        float posY = TreeNode.Leaves[0].GO.transform.position.y / 2;
        cam.transform.position = new Vector3 (0f, posY, cam.transform.position.z);

        
        //area = new Bounds (cam.transform.position, new Vector3 (2 * cam.orthographicSize * cam.aspect, 2 * cam.orthographicSize));
        Vector3 areaSize = new Vector3(camSizeX, camSizeY, 0f );
        
        area = new Bounds (cam.transform.position, areaSize);

        float areaAspect = area.size.x / area.size.y;

        if (areaAspect > cam.aspect) {
            cam.orthographicSize = (areaSize.x / cam.aspect)/ 2;
        }
        else {
            cam.orthographicSize = (areaSize.y) / 2;
        }
            //cam.orthographicSize = Mathf.Max( areaSize.x * cam.aspect / 2, areaSize.y / 2 ) + border;
            

        //setup the collider size for checking mouse interactions properly
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
            if (Input.GetAxis( "Mouse ScrollWheel" ) < 0) { //zoom out
                float maxCamSize = Mathf.Max( area.extents.y, area.extents.x / cam.aspect );
                cam.orthographicSize = Mathf.Min( cam.orthographicSize + cam.orthographicSize * wheel, maxCamSize);
            }
            else if (Input.GetAxis( "Mouse ScrollWheel" ) > 0) //zoom in
                cam.orthographicSize = Mathf.Max( Camera.main.orthographicSize - cam.orthographicSize * wheel, minSize );

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
        //se a camera for maior que a area, nao deve se mover em y
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

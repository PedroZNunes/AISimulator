using UnityEngine;

[RequireComponent (typeof (Camera))]
public class CameraController : MonoBehaviour {

    [SerializeField]
    private float border;

    private float maxSize;
    private float minSize = 2;
    private Camera cam;

    static private CameraController instance;

    private void Awake () {
        if (instance == null)
            instance = FindObjectOfType<CameraController> ();
        if (instance != this)
            Destroy (this.gameObject);

        cam = GetComponent<Camera> ();
    }

    private void OnEnable () {
        TreeGenerator.SetCameraEvent += SetCamera;
    }
    private void OnDisable () {
        TreeGenerator.SetCameraEvent -= SetCamera;
    }

    private void SetCamera (int depth, int branching, float stepY) {
        float totalLeafs = Mathf.Pow (branching, depth);

        float sizeY = ((stepY * depth) + 1) / 2;

        float sizeX = ((totalLeafs + 1) / 16 * 12) / 2;

        cam.orthographicSize = maxSize = Mathf.Max (sizeX, sizeY) + border;
        

        float posY = GamesNode.Leafs[0].GO.transform.position.y / 2;
        cam.transform.position = new Vector3 (0f, posY, cam.transform.position.z);
    }

    private void Update () {
        if (Input.GetAxis ("Mouse ScrollWheel") < 0) //forward
        {
            cam.orthographicSize = Mathf.Min (cam.orthographicSize + 1, maxSize);
        }
        else if (Input.GetAxis ("Mouse ScrollWheel") > 0) //backwards
        {
            cam.orthographicSize = Mathf.Max (Camera.main.orthographicSize - 1, minSize);
        }
    }
}

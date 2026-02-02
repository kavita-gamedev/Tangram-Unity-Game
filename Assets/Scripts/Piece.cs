using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class Piece : MonoBehaviour
{
    [Header("Movement")]
    public float dragHeight = 0.5f;
    public float snapDistance = 0.8f;

    [Header("Rotation")]
    public float rotationStep = 45f;

    [Header("Puzzle")]
    public Transform targetTransform;
    public bool locked;

    [Header("Settings")]
    public int kidAge = 10; // For desktop right-click limit

    [Header("Visuals")]
    public Material highlightMaterial;

    private Vector3 offset;
    private Camera cam;
    private bool dragging;

    private Renderer targetRenderer;
    private Material originalMaterial;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private float lastTapTime; // For mobile double-tap rotation
    private const float doubleTapDelay = 0.3f;

    void Start()
    {
        cam = Camera.main;
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (targetTransform != null)
        {
            targetRenderer = targetTransform.GetComponent<Renderer>();
            if (targetRenderer != null)
                originalMaterial = targetRenderer.material;
        }
    }

    void Update()
    {
        if (locked) return;

        #region Desktop Mouse Input
        // Dragging with mouse
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            HandleMouseInput();
        }
        #endregion

        #region Touch Input
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        #endregion
    }

    #region Mouse Input Methods
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (IsPointerOverPiece(Input.mousePosition))
            {
                dragging = true;
                offset = transform.position - GetWorldPosition(Input.mousePosition);
                Lift(true);
            }
        }

        if (Input.GetMouseButton(0) && dragging)
        {
            Vector3 pos = GetWorldPosition(Input.mousePosition) + offset;
            transform.position = new Vector3(pos.x, dragHeight, pos.z);
            HighlightTarget();
        }

        if (Input.GetMouseButtonUp(0) && dragging)
        {
            dragging = false;
            Lift(false);
            TrySnap();
        }

        // Rotate with right-click (desktop)
        if (kidAge <= 10 && Input.GetMouseButtonDown(1) && IsPointerOverPiece(Input.mousePosition))
        {
            Rotate();
        }
    }
    #endregion

    #region Touch Input Methods
    void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchWorldPos = GetWorldPosition(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (IsPointerOverPiece(touch.position))
                {
                    dragging = true;
                    offset = transform.position - touchWorldPos;

                    // Check for double-tap for rotation
                    if (Time.time - lastTapTime < doubleTapDelay)
                    {
                        Rotate();
                    }
                    lastTapTime = Time.time;

                    Lift(true);
                }
                break;

            case TouchPhase.Moved:
                if (dragging)
                {
                    Vector3 pos = touchWorldPos + offset;
                    transform.position = new Vector3(pos.x, dragHeight, pos.z);
                    HighlightTarget();
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (dragging)
                {
                    dragging = false;
                    Lift(false);
                    TrySnap();
                }
                break;
        }
    }
    #endregion

    bool IsPointerOverPiece(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }

    Vector3 GetWorldPosition(Vector2 screenPos)
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = cam.ScreenPointToRay(screenPos);
        plane.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up, rotationStep);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.rotateClip);
    }

    void Lift(bool up)
    {
        transform.localScale = up ? Vector3.one * 1.03f : Vector3.one;

        Vector3 p = transform.position;
        p.y = up ? dragHeight : 0f;
        transform.position = p;
    }

    void TrySnap()
    {
        if (targetTransform == null || locked)
        {
            ReturnToOriginalPosition();
            return;
        }

        Vector3 currentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetXZ = new Vector3(targetTransform.position.x, 0f, targetTransform.position.z);

        float distance = Vector3.Distance(currentXZ, targetXZ);

        if (distance < snapDistance)
            SnapToTarget();
        else
            ReturnToOriginalPosition();
    }

    void SnapToTarget()
    {
        StartCoroutine(SmoothMove(targetTransform.position, targetTransform.rotation, 0.2f, () =>
        {
            locked = true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.snapClip);

            if (targetRenderer)
                targetRenderer.enabled = false;

            PuzzleManager.Instance.CheckCompletion();
        }));
    }

    void ReturnToOriginalPosition()
    {
        StartCoroutine(SmoothMove(originalPosition, originalRotation, 0.3f, null));
    }

    IEnumerator SmoothMove(Vector3 targetPos, Quaternion targetRot, float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        onComplete?.Invoke();
    }

    void HighlightTarget()
    {
        if (targetRenderer == null) return;

        Vector3 currentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetXZ = new Vector3(targetTransform.position.x, 0f, targetTransform.position.z);

        float distance = Vector3.Distance(currentXZ, targetXZ);

        targetRenderer.material = (distance < snapDistance) ? highlightMaterial : originalMaterial;
    }

    void ResetTargetHighlight()
    {
        if (targetRenderer != null)
            targetRenderer.material = originalMaterial;
    }
}

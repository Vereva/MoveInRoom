using UnityEngine;

public class Movable : MonoBehaviour
{
    enum ColorsInteractive
    {
        Idle,
        CorrectPosition,
        WrongPosition
    }

    private Color[] _colorsInteractive = new Color[3];

    private Renderer _renderer;
    private Collider _collider;
    private Bounds _bounds;

    private RaycastHit _currentHit;
    private Vector3 _positionBeforeMove;
    private Vector3 _collisionWithNeighborOffset;

    private bool _canPlace;
    
    public Bounds _Bounds => _bounds;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();

        _colorsInteractive = new Color[] { new Color(225f, 90f, 200f, 255f) / 255f, new Color(90f, 225f, 100f, 255f) / 255f, new Color(225f, 90f, 90f, 255f) / 255f };
    }

    private void Start()
    {
        _collider = GetComponent<Collider>();

        _bounds = _collider.bounds;
        SavePosition();
    }

    public void OnSelect()
    {
        _canPlace = false;
        SetTransparent(true);
        SetColor(ColorsInteractive.WrongPosition);
    }

    public void OnDeselect()
    {
        if (_canPlace)
        {
            SavePosition();
            transform.SetParent(_currentHit.transform);
        }
        SetTransparent(false);
        SetColor(ColorsInteractive.Idle);
        LoadPosition();
    }
    private void SavePosition()
    {
        _positionBeforeMove = transform.position;
    }
    private void LoadPosition()
    {
        transform.position = _positionBeforeMove;
    }

    private void SetActiveColor(bool isCorrectNormal)
    {
        SetColor(ColorsInteractive.WrongPosition);

        if (isCorrectNormal)
        {
            SetColor(ColorsInteractive.CorrectPosition);
        }
    }
    public void SetTransparent(bool transparent)
    {
        SetTransparentChildren(transparent);
        _collider.enabled = !transparent;
    }
    private void SetTransparentChildren(bool isCorrectNormal)
    {
        if(transform.childCount > 0)
        {
            Movable[] childMovables = GetComponentsInChildren<Movable>();
            foreach (Movable movable in childMovables)
            {
                if (movable == this)
                    continue;
                movable.SetTransparent(isCorrectNormal);
            }
        }
    }
    private void SetColor(ColorsInteractive targetColor)
    {
        _renderer.material.color = _colorsInteractive[(int)targetColor];
    }

    private void SetIsCanPlace(RaycastHit hit, out bool needRotate, ref Vector3 colOffset)
    {
        float permissionOffset = 0.001f;

        bool correctNormal = hit.normal == Vector3.up;

        bool withoutRotate = (hit.collider.bounds.size.x - _bounds.size.x > -permissionOffset
                           && hit.collider.bounds.size.z - _bounds.size.z > -permissionOffset); 

        bool withRotate = (hit.collider.bounds.size.x - _bounds.size.z > -permissionOffset
                        && hit.collider.bounds.size.z - _bounds.size.x > -permissionOffset);

        needRotate = !withoutRotate && withRotate && correctNormal;

        bool canPlace = IsCanPlaceWithNeighborsColliders(hit, ref colOffset) && correctNormal && (withoutRotate || withRotate);

        SetActiveColor(canPlace);

        _canPlace = canPlace;
    }
    private bool IsCanPlaceWithNeighborsColliders(RaycastHit movableHit, ref Vector3 colOffset)
    {
        if (_currentHit.normal != movableHit.normal)
        {
            _collisionWithNeighborOffset = Vector3.zero;
        }

        bool[] isHitted = new bool[4];
        Vector3[] raycastDirections = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        for (int i = 0; i < raycastDirections.Length; i++)
        {
            Vector3 direction = Vector3.Scale(Vector3.one, raycastDirections[i]);
            float distance = Vector3.Scale(_bounds.size, raycastDirections[i]).magnitude / 2f;
            bool raycastHited = Physics.Raycast(new Ray(transform.position, direction), out RaycastHit hit, distance);

            if (raycastHited)
            {
                colOffset += Vector3.Scale(Vector3.one * (-distance + hit.distance), direction);
            }

            isHitted[i] = raycastHited;
        }
        return !((isHitted[0] && isHitted[1]) || (isHitted[2] && isHitted[3]));
    }
    private void SetRotated(bool needRotate)
    {
        if (needRotate)
        {
            transform.Rotate(Vector3.up, 90f);
            RecalculateColliderBounds();
        }
    }
    private void RecalculateColliderBounds()
    {
        _collider.enabled = true;
        _bounds = _collider.bounds;
        _collider.enabled = false;
    }

    private void SetPositionOnObject(RaycastHit hit)
    {
        Vector3 newPosition = _collisionWithNeighborOffset + _currentHit.point + Vector3.Scale(_currentHit.normal, _bounds.size) / 2f;

        if (hit.normal != Vector3.up)
        {
            transform.position = newPosition;
            return;
        }
        Vector3 halfSizeDifference = (hit.collider.bounds.size - _bounds.size) / 2f;

        newPosition.x = Mathf.Clamp(newPosition.x, hit.collider.gameObject.transform.position.x - halfSizeDifference.x, hit.collider.gameObject.transform.position.x + halfSizeDifference.x);
        newPosition.z = Mathf.Clamp(newPosition.z, hit.collider.gameObject.transform.position.z - halfSizeDifference.z, hit.collider.gameObject.transform.position.z + halfSizeDifference.z);

        transform.position = newPosition;
    }
    public void MoveOnCollider(RaycastHit hit)
    {
        _currentHit = hit;
        SetIsCanPlace(hit, out bool needRotate, ref _collisionWithNeighborOffset);
        SetRotated(needRotate);
        SetPositionOnObject(hit);
    }
}

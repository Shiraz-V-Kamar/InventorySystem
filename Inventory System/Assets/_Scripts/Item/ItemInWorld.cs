using System;
using UnityEngine;

public class ItemInWorld : MonoBehaviour, ICollectable
{ 
    public ItemScriptableObject item;
    private Rigidbody _rb;
    private BoxCollider _collider;

    private bool _grounded;
    [SerializeField]private float _groundedRadius;
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField]private float _groundedOffset;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
    }
    public void Collect()
    {
        bool result = InventoryManager.Instance.AddItem(item);
        if(result)
            Destroy(gameObject);
    }

    private void Update()
    {
        GroundedCheck();
        if (_grounded)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
            _collider.isTrigger = true;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Collect();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z),_groundedRadius);
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
        if(_rb.velocity.magnitude==0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}

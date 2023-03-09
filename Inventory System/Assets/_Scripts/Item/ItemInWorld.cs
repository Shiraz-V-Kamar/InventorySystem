using UnityEngine;

public class ItemInWorld : MonoBehaviour, ICollectable
{ 
    public ItemScriptableObject item;
    private Rigidbody _rb;
    private BoxCollider _collider;

    private bool _grounded;

    [SerializeField]private float _groundedRadius;
    [SerializeField]private float _groundedOffset;
    [SerializeField] private LayerMask _groundLayers;

    AudioManager audioManager;
    private void Start()
    {
        audioManager = AudioManager.instance;

        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        GroundedCheck();

        // Object is spawned with gravity effecting it
        // The values are changed when it comes in contact with ground
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
    public void Collect()
    {
        bool result = InventoryManager.Instance.AddItem(item);

        if (result)
        {
            audioManager.PlaySound(Helper.PICK_UP_ITEMS);
            Destroy(gameObject);
        }else
        {

        }
        // give visual feedback to player why its not pickable Slot full / Unique item
    }

    private void GroundedCheck()
    {
        // set sphere position to check ground collision, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);

        // When object stops moving/ not falling, Reset rotation
        if (_rb.velocity.magnitude == 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);
    }
}

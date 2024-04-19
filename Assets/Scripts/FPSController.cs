using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    // references
    CharacterController controller;
    [SerializeField] GameObject cam;
    [SerializeField] Transform gunHold;
    [SerializeField] Gun initialGun;

    // stats
    [SerializeField] float movementSpeed = 2.0f;
    [SerializeField] float lookSensitivityX = 1.0f;
    [SerializeField] float lookSensitivityY = 1.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;
    [SerializeField] float maxHealth = 20;

    // private variables
    float health;
    public float Health
    {
        get { return health; }
        set
        {
            health = (value < maxHealth) ? value : maxHealth;
            HealthChange.Invoke(health, maxHealth);
        }
    }
    Vector3 origin;
    Vector3 velocity;
    Vector3 moveAxis;
    Vector3 lookAxis;
    bool firing = false;
    bool sprinting;
    bool grounded;
    float xRotation;
    List<Gun> equippedGuns = new List<Gun>();
    int gunIndex = 0;
    Gun currentGun = null;

    // properties
    public GameObject Cam { get { return cam; } }

    [SerializeField] UnityEvent<float, float> HealthChange;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Health = maxHealth;

        // start with a gun
        if(initialGun != null)
            AddGun(initialGun);

        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Look();
        HandleSwitchGun();
        FireGun();

        // always go back to "no velocity"
        // "velocity" is for movement speed that we gain in addition to our movement (falling, knockback, etc.)
        Vector3 noVelocity = new Vector3(0, velocity.y, 0);
        velocity = Vector3.Lerp(velocity, noVelocity, 5 * Time.deltaTime);
    }

    void Move()
    {
        grounded = controller.isGrounded;

        if(grounded && velocity.y < 0)
        {
            velocity.y = -1;// -0.5f;
        }

        Vector3 move = transform.right * moveAxis.x + transform.forward * moveAxis.y; 
        controller.Move(move * movementSpeed * (sprinting ? 2 : 1) * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        float lookX = lookAxis.x * lookSensitivityX * Time.deltaTime;
        float lookY = lookAxis.y * lookSensitivityY * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookX);
    }

    void HandleSwitchGun()
    {
        if (equippedGuns.Count == 0)
            return;

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            gunIndex++;
            if (gunIndex > equippedGuns.Count - 1)
                gunIndex = 0;

            EquipGun(equippedGuns[gunIndex]);
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            gunIndex--;
            if (gunIndex < 0)
                gunIndex = equippedGuns.Count - 1;

            EquipGun(equippedGuns[gunIndex]);
        }
    }

    void FireGun()
    {
        // don't fire if we don't have a gun
        if (currentGun == null)
            return;

        // holding the fire button
        if(firing && currentGun.AttemptAutomaticFire())
        {
            currentGun?.AttemptFire();
        }

        // pressed the alt fire button
        /*if (GetPressAltFire())
        {
            currentGun?.AttemptAltFire();
        }*/
    }

    void EquipGun(Gun g)
    {
        // disable current gun, if there is one
        currentGun?.Unequip();
        currentGun?.gameObject.SetActive(false);

        // enable the new gun
        g.gameObject.SetActive(true);
        g.transform.parent = gunHold;
        g.transform.localPosition = Vector3.zero;
        currentGun = g;

        g.Equip(this);
    }

    // public methods

    public void AddGun(Gun g)
    {
        // add new gun to the list
        equippedGuns.Add(g);

        // our index is the last one/new one
        gunIndex = equippedGuns.Count - 1;

        // put gun in the right place
        EquipGun(g);
    }

    public void IncreaseAmmo(int amount)
    {
        currentGun.AddAmmo(amount);
    }

    public void Respawn()
    {
        Health = maxHealth;
        transform.position = origin;
    }

    // Input methods
    public void OnJump()
    {
        if (grounded)
        {
            velocity.y += Mathf.Sqrt(jumpForce * -1 * gravity);
        }
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.started) //called when first pressed down
        {
            currentGun?.AttemptFire();
            firing = true;
        }
        else if (ctx.canceled) //called again when released
            firing = false;
    }

    public void OnAltFire()
    {
        currentGun?.AttemptAltFire();
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        moveAxis = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookAxis = ctx.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) //called when first pressed down
            sprinting = true;
        else if (ctx.canceled) //called again when released
            sprinting = false;

    }

    public void TakeDamage(float dmg)
    {
        Health -= dmg;
    }
    // Collision methods

    // Character Controller can't use OnCollisionEnter :D thanks Unity
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.GetComponent<Damager>())
        {
            var collisionPoint = hit.collider.ClosestPoint(transform.position);
            var knockbackAngle = (transform.position - collisionPoint).normalized;
            velocity = (20 * knockbackAngle);
            Health -= 5;
        }

        if (hit.gameObject.GetComponent <KillZone>())
        {
            Respawn();
        }
    }


}

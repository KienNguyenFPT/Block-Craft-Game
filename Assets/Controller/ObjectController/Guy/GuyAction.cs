using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WaterSystem;

public class GuyAction : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 720f;
    public float jumpSpeed = 10f;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private Animator animator;
    private GuyStats stats;
    public float groundCheckDistance = 2f;
    public float lookSpeed = 2f;
    public LayerMask groundLayer;

    public Transform[] cameraTransforms;

    private GuyInventory inventory;
    private float velo = 0.0f;
    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
    private static readonly int VelocityXHash = Animator.StringToHash("X");
    private static readonly int VelocityYHash = Animator.StringToHash("Y");
    private bool isSlashing = false;
    private bool canSlash = true;
    public float slashCooldown = 5.0f;

    public GameObject WeaponsList;
    public GameObject InventoryList;

    public Weapon currentWeapon;
    public Item currentBlock;

    public float playerHeight = 2.0f;

    private Rigidbody playerRigidbody;
    private RaycastHit isGround;
    public ParticleSystem ripple;
    [SerializeField] private float VelocityXZ, VelocityY;
    private Vector3 PlayerPos;
    private bool inWater;
    private float CameraY, GravityForce, Zoom = -7;
    public float Speed, Gravity, JumpHeight;

    public bool getIsSlashing() => isSlashing;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        animator = GetComponent<Animator>();
        stats = GetComponent<GuyStats>();
        inventory = GetComponent<GuyInventory>();
        animator.SetFloat(VelocityHash, velo);
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Transform activeCamera = GetActiveCameraTransform();
        if (activeCamera != null)
        {
            HandleMovement(activeCamera);
            HandleWeaponSwitch();
            VelocityXZ = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(PlayerPos.x, 0, PlayerPos.z));
            VelocityY = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, PlayerPos.y, 0));
            PlayerPos = transform.position;

            //RippleCamera.transform.position = transform.position + Vector3.up * 10;
            if (isGround.collider) ripple.transform.position = transform.position + transform.forward;
            else ripple.transform.position = transform.position;
            Shader.SetGlobalVector("_Player", transform.position);
        }
    }
    /*
    void FixedUpdate()
    {
        if (isInWater)
        {

            HandleSwimming();
        }
    }

    void HandleSwimming()
    {
        float moveDirectionY = 0.0f;

        if (Input.GetKey(KeyCode.Space))
        {
            moveDirectionY = swimSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirectionY = -swimSpeed;
        }

        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), moveDirectionY, Input.GetAxis("Vertical"));
        playerRigidbody.AddForce(moveDirection * swimSpeed, ForceMode.Force);
    }
    */

    Transform GetActiveCameraTransform()
    {
        foreach (var camTransform in cameraTransforms)
        {
            if (camTransform.gameObject.active)
            {
                return camTransform;
            }
        }
        return null;
    }

    void HandleMovement(Transform activeCamera)
    {
        if (stats.IsDied) return;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = activeCamera.forward;
        Vector3 cameraRight = activeCamera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 movementDirection = (horizontalInput * cameraRight + verticalInput * cameraForward).normalized;

        if (horizontalInput != 0 || verticalInput != 0)
        {
            if (velo < speed) velo += Time.deltaTime * (speed / 5);
        }
        else
        {
            velo = 0.0f;
        }

        animator.SetFloat(VelocityHash, velo);
        animator.SetFloat(VelocityXHash, horizontalInput);
        animator.SetFloat(VelocityYHash, verticalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
        }

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        else
        {
            characterController.stepOffset = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;

            if ((Physics.Raycast(transform.position, Vector3.down, out hit) && hit.distance <= 0.3) || (stats.level > 20 && hit.distance <= ySpeed))
            {
                ySpeed = jumpSpeed;
            }

            animator.SetTrigger("Jump");
        }

        Vector3 velocity = movementDirection * velo;
        velocity.y = ySpeed;

        GravityForce -= Gravity * Time.deltaTime * 5;
        if (isGround.collider && GravityForce < -2) GravityForce = -2;
        else if (GravityForce < -99) GravityForce = -99;
        characterController.Move(new Vector3(0, GravityForce * Time.deltaTime, 0));

        if (inWater) ripple.gameObject.SetActive(true);
        else ripple.gameObject.SetActive(false);
        Physics.Raycast(transform.position, Vector3.down, out isGround, 2.7f, LayerMask.GetMask("Mountain"));
        Debug.DrawRay(transform.position, Vector3.down * 2.7f);

        //
        float height = characterController.height + characterController.radius;
        inWater = Physics.Raycast(transform.position + Vector3.up * height, Vector3.down, height * 2, LayerMask.GetMask("Water"));
        Debug.DrawRay(transform.position + Vector3.up * height, Vector3.down * height);

        characterController.Move(velocity * Time.deltaTime);

        HandleOtherAction();
    }

    void HandleOtherAction()
    {
        if (Input.GetButtonDown("Slash") && currentWeapon && canSlash)
        {
            animator.SetTrigger("Slash");
            StartCoroutine(SlashCoroutine());
        }

        if (Input.GetButtonDown("Grab"))
        {
            animator.SetTrigger("Grab");
        }

        if (Input.GetButtonDown("PickUp"))
        {
            animator.SetTrigger("PickUp");
        }
    }

    private IEnumerator SlashCoroutine()
    {
        isSlashing = true;
        canSlash = false;
        yield return new WaitForSeconds(slashCooldown);
        isSlashing = false;
        canSlash = true;
    }


    void HandleWeaponSwitch()
    {
        for (int i = 1; i <= 3; i++)
        {
            Image childOverlay = WeaponsList.GetComponentsInChildren<Image>()[i];
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                Weapon weapon = inventory.GetWeapon(i - 1);

                Color overlayColor = new Color(226 / 255f, 84 / 255f, 84 / 255f, 1.0f);
                Color overlayColorDis = new Color(212 / 255f, 212 / 255f, 212 / 255f, 1.0f);
                if (weapon != null)
                {

                    if (currentWeapon != weapon)
                    {
                        changeColorOverlay(childOverlay, overlayColor);
                        EquipWeapon(weapon);
                    }
                    else
                    {
                        changeColorOverlay(childOverlay, overlayColorDis);
                        DestroyWeapon();
                    }
                }
                for (int j = 1; j <= 3; j++)
                {
                    if (i != j)
                    {
                        Image childOverlayDis = WeaponsList.GetComponentsInChildren<Image>()[j];
                        changeColorOverlay(childOverlayDis, overlayColorDis);
                    }
                }
            }
        }
    }

    void changeColorOverlay(Image overlayImage, Color color)
    {
        if (overlayImage != null)
        {
            overlayImage.color = color;
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;

        UpdatePlayerWeaponModel(newWeapon);
    }

    public void DestroyWeapon()
    {
        Transform weaponSlot = FindChildTransformByName(transform, "mixamorig:RightHandMiddle4");
        if (weaponSlot != null)
        {
            foreach (Transform child in weaponSlot)
            {
                Destroy(child.gameObject);
            }
        }
        currentWeapon = null;
    }

    private void UpdatePlayerWeaponModel(Weapon weapon)
    {
        Transform weaponSlot = FindChildTransformByName(transform, "mixamorig:RightHandMiddle4");
        if (weaponSlot != null)
        {
            foreach (Transform child in weaponSlot)
            {
                Destroy(child.gameObject);
            }

            if (weapon.prefab != null)
            {
                GameObject instantiatedWeapon = Instantiate(Resources.Load<GameObject>(weapon.prefab), weaponSlot);
                RotateWeaponTowardsCharacter(instantiatedWeapon);
            }
        }
    }

    private void RotateWeaponTowardsCharacter(GameObject weaponObject)
    {
        if (weaponObject != null)
        {
            Vector3 directionToCharacter = transform.position - weaponObject.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToCharacter, Vector3.one);

            weaponObject.transform.rotation = lookRotation;
        }
    }

    private Transform FindChildTransformByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            else
            {
                Transform foundChild = FindChildTransformByName(child, name);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
        }
        return null;
    }

    public void BuildBlock(Vector3 position)
    {
        if (currentBlock != null && currentBlock.amount > 0)
        {
            GameObject blockPrefab = Resources.Load<GameObject>(currentBlock.prefab);
            GlowingSphere script = blockPrefab.GetComponent<GlowingSphere>();
            if (script != null)
            {
                Destroy(script);
            }
            if (blockPrefab != null)
            {
                Instantiate(blockPrefab, position, Quaternion.identity);
                currentBlock.amount--;
                Debug.Log("Block built. Remaining amount: " + currentBlock.amount);
            }
            else
            {
                Debug.LogWarning("Block prefab not found!");
            }
        }
        else
        {
            Debug.LogWarning("No blocks available to build!");
        }
    }

    public void DestroyBlock(GameObject block)
    {
        if (currentWeapon != null && block.CompareTag("Block"))
        {
            Destroy(block);
            Debug.Log("Block destroyed with weapon: " + currentWeapon.itemName);
        }
        else
        {
            Debug.LogWarning("No weapon equipped to destroy the block!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
        }
    }
    /*
    public float Speed, Gravity, JumpHeight;
    private CharacterController cc;
    public GameObject Cameraman, RippleCamera;
    private float CameraY, GravityForce, Zoom = -7;
    private RaycastHit isGround;
    public ParticleSystem ripple;
    [SerializeField] private float VelocityXZ, VelocityY;
    private Vector3 PlayerPos;
    private bool inWater;
    // Start is called before the first frame update yup
    void Start()
    {
        Application.targetFrameRate = 60;
        cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        CameraControl();

        if (Input.GetKeyDown(KeyCode.Space)) Jumping();
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        VelocityXZ = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(PlayerPos.x, 0, PlayerPos.z));
        VelocityY = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, PlayerPos.y, 0));
        PlayerPos = transform.position;

        //RippleCamera.transform.position = transform.position + Vector3.up * 10;
        if (isGround.collider) ripple.transform.position = transform.position + transform.forward;
        else ripple.transform.position = transform.position;
        Shader.SetGlobalVector("_Player", transform.position);
    }
    void PlayerMovement()
    {
        Vector3 camRight = Cameraman.transform.right;
        Vector3 camForward = Cameraman.transform.forward;
        camRight.y = 0;
        camForward.y = 0;

        Vector3 move = camRight.normalized * Input.GetAxis("Horizontal") + camForward.normalized * Input.GetAxis("Vertical");
        cc.Move(move.normalized * Time.deltaTime * 10 * Speed * ((Input.GetKey(KeyCode.LeftShift) ? 2 : 1)));
        if (move.magnitude > 0) transform.forward = move.normalized;

        GravityForce -= Gravity * Time.deltaTime * 5;
        if (isGround.collider && GravityForce < -2) GravityForce = -2;
        else if (GravityForce < -99) GravityForce = -99;
        cc.Move(new Vector3(0, GravityForce * Time.deltaTime, 0));

        if (inWater) ripple.gameObject.SetActive(true);
        else ripple.gameObject.SetActive(false);
        Physics.Raycast(transform.position, Vector3.down, out isGround, 2.7f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(transform.position, Vector3.down * 2.7f);

        //
        float height = cc.height + cc.radius;
        inWater = Physics.Raycast(transform.position + Vector3.up * height, Vector3.down, height * 2, LayerMask.GetMask("Water"));
        Debug.DrawRay(transform.position + Vector3.up * height, Vector3.down * height);
    }
    void Jumping()
    {
        // || !isGround.collider
        if (GravityForce > 2) return;
        float JumpMutify = 2;
        GravityForce = Mathf.Sqrt(JumpHeight * Gravity * JumpMutify);
        cc.Move(new Vector3(0, GravityForce * Time.deltaTime, 0));
    }
    void CameraControl()
    {
        Cameraman.transform.position = transform.position;
        CameraY -= Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * 300;
        CameraY = Mathf.Clamp(CameraY, -45, 45);

        Zoom += Input.mouseScrollDelta.y * Time.fixedDeltaTime * 100;
        Zoom = Mathf.Clamp(Zoom, -12, -4);

        Cameraman.transform.Rotate(0, Input.GetAxis("Mouse X") * Time.fixedDeltaTime * 150, 0);
        Cameraman.transform.eulerAngles = new Vector3(CameraY, Cameraman.transform.eulerAngles.y, 0);
        Cameraman.transform.GetChild(0).transform.localPosition = new Vector3(0, 1.15f, Zoom);
    }
    void CreateRipple(int Start, int End, int Delta, float Speed, float Size, float Lifetime)
    {
        Vector3 forward = ripple.transform.eulerAngles;
        forward.y = Start;
        ripple.transform.eulerAngles = forward;

        for (int i = Start; i < End; i += Delta)
        {
            ripple.Emit(transform.position + ripple.transform.forward * 1.15f, ripple.transform.forward * Speed, Size, Lifetime, Color.white);
            ripple.transform.Rotate(Vector3.up * Delta, Space.World);
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        // && !isGround.collider && Mathf.Abs(VelocityY) > 0.1f  && Mathf.Abs(VelocityY) > 0.1f
        if (other.gameObject.layer == 4)
        {
            //CreateRipple(-180, 180, 2, 5, 3f, 3);
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            //CreateRipple(-180, 180, 2, 5, 3f, 3);
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
        }
    }

    */
    

    /*
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 4 && VelocityXZ > 0.025f && Time.renderedFrameCount % 3 == 0)
        {
            int y = (int)transform.eulerAngles.y;
            CreateRipple(y-100, y+100, 3, 5f, 2.65f, 3f);
        }
    }
    */
}

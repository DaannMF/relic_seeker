using UnityEngine;

public class WalkState : BaseState<EPlayerStates> {
    private PlayerStateContext playerContext;
    private WalkSO walkData;

    public WalkState(PlayerStateContext context, WalkSO stateData) : base(EPlayerStates.Walk, stateData, context) {
        playerContext = context;
        walkData = stateData;
    }

    public override void Enter() {
        Debug.Log("Entering Walk State");

        // Handle walk animations
        //if (playerContext.Animator != null) {
        //    playerContext.Animator.SetFloat("MovementSpeed", 1f);
        //}
    }

    public override void Update() {
        // Handle rotation in all states
        playerContext.HandleRotation();

        // Handle controllable detection in all states
        playerContext.HandleDetectControllable();

        // Handle movement
        HandleMovement();

        // // Update animation based on movement speed
        // if (playerContext.Animator != null) {
        //     float movementMagnitude = playerContext.Input.MovementInput.magnitude;
        //     playerContext.Animator.SetFloat("MovementSpeed", movementMagnitude);
        // }
    }

    public override void FixedUpdate() {
        // Apply gravity to keep player grounded
        playerContext.ApplyGravity();
    }

    public override void Exit() {
        Debug.Log("Exiting Walk State");

        // Reset horizontal velocity when exiting walk state
        Vector3 velocity = playerContext.Rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        playerContext.Rb.velocity = velocity;
    }

    public override EPlayerStates GetNextState() {
        // Check for jump input first (highest priority)
        if (playerContext.Input.JumpPressed) {
            return EPlayerStates.Jump;
        }

        // Then check for movement
        if (playerContext.Input.IsMoving) {
            return EPlayerStates.Walk;
        }

        // No movement, go to idle
        return EPlayerStates.Idle;
    }

    private void HandleMovement() {
        Vector2 input = playerContext.Input.MovementInput;

        float horizontal = input.x;
        float vertical = input.y;

        Vector3 camForward = playerContext.CameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = playerContext.CameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
        Vector3 velocity = new Vector3(moveDir.x * walkData.MoveSpeed, playerContext.Rb.velocity.y, moveDir.z * walkData.MoveSpeed);

        if (CanMove(moveDir)) {
            playerContext.Rb.velocity = velocity;
        }
    }

    private bool CanMove(Vector3 moveDir) {
        Terrain terrain = Terrain.activeTerrain;
        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(playerContext.Rb.position);
        float nextHeight = terrain.SampleHeight(playerContext.Rb.position + moveDir * 5);

        if (angle > walkData.MaxAngleMovement && nextHeight > currentHeight)
            return false;
        return true;
    }

    private Vector3 GetMapPos() {
        Vector3 pos = playerContext.Rb.position;
        Terrain terrain = Terrain.activeTerrain;

        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }
}
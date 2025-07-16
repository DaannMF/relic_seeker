using UnityEngine;

public class PlayerStateContext : BaseStateContext<EPlayerStates> {
    private PlayerInput input;
    private PlayerController playerController;

    private float gravity;
    private float maxFallSpeed;

    public PlayerInput Input => input;
    public PlayerController PlayerController => playerController;

    public PlayerStateContext(PlayerInput input, Animator animator, PlayerController playerController) {
        this.input = input;
        this._animator = animator;
        this.playerController = playerController;
        this.gravity = 20f;
        this.maxFallSpeed = 15f;
    }

    public void SetGravitySettings(float gravity, float maxFallSpeed) {
        this.gravity = gravity;
        this.maxFallSpeed = maxFallSpeed;
    }

    public void ApplyGravity() {
        Vector3 gravityForce = Vector3.down * gravity * playerController.Rb.mass;
        playerController.Rb.AddForce(gravityForce, ForceMode.Force);

        Vector3 velocity = playerController.Rb.velocity;
        if (velocity.y < -maxFallSpeed) {
            velocity.y = -maxFallSpeed;
            playerController.Rb.velocity = velocity;
        }
    }

    public bool IsGrounded(float checkDistance = 0.1f, LayerMask groundMask = default) {
        if (groundMask == default) groundMask = 1;

        Vector3 rayOrigin = playerController.Rb.position + Vector3.up * 0.1f;
        Ray ray = new Ray(rayOrigin, Vector3.down);

        bool isGrounded = Physics.Raycast(ray, checkDistance + 0.1f, groundMask);

#if UNITY_EDITOR
        Color rayColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, Vector3.down * (checkDistance + 0.1f), rayColor);
#endif

        return isGrounded;
    }

    public bool CanMove(Vector3 moveDir, float maxAngleMovement) {
        if (playerController.IsInInterior) return true;

        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return true;

        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(playerController.Rb.position);
        float nextHeight = terrain.SampleHeight(playerController.Rb.position + moveDir * 5);

        if (angle > maxAngleMovement && nextHeight > currentHeight)
            return false;

        return true;
    }

    public Vector3 GetMapPos() {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return Vector3.zero;

        Vector3 pos = playerController.Rb.position;
        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }
}
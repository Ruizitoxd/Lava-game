using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Scriptable Objects/PlayerMovementStats")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float groundAcceleration = 5f;
    [Range(0.25f, 50f)] public float groundDeceleration = 20f;
    [Range(0.25f, 50f)] public float airAcceleration = 5f;
    [Range(0.25f, 50f)] public float airDeceleration = 5f;

    [Header("Run")]
    [Range(1f, 100f)] public float maxRunSpeed = 25f;

    [Header("Grounded/Colission Checks")]
    public LayerMask groundLayer;
    public float colissionDetectionRayLenght = 0.02f;
    public float groundingForce = -1.5f;
    public float grounderDistance = 0.05f;

    [Header("Jump")]
    public float jumpPower = 6.5f;
    public float maxFallSpeed = 40;
    public float fallAcceleration = 110;
    public float JumpEndEarlyGravityModifier = 3;
    [Range(1, 5)] public int numberOfJumps = 1;

    [Header("Jump Apex")]
    [Range(0.05f, 1f)] public float apexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float apexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;

    [Header("Dash")]
    public float dashPower = 30f;
    public float dashTime = 0.2f;
    public float dashingCooldown = 1f;
}
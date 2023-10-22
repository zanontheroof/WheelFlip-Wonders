using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateboardController : MonoBehaviour
{
    public Transform m_skateboard;
    public float m_alignSpeed = 5;
    public float m_rayDistance = 5f;
    [Range(-1, 1)]
    public float m_Forward;
    public float m_FwdForce = 10;
    public float m_TurnForce = 5;
    public float jumpTiltAmount = 10.0f;
    public float jumpDuration = 0.2f;
    public float jumpForce = 1000.0f; // Jump force
    public float cooldownDuration = 2.0f;

    private Vector3 m_surfaceNormal = new Vector3();
    private Vector3 m_collisionPoint = new Vector3();
    private bool m_onSurface;
    private Collision m_surfaceCollisionInfo;
    private Rigidbody m_rb;
    private bool isJumping = false;
    private float jumpEndTime = 0f;
    private float lastTurnTime = 0f;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ProcessInputs();
    }

    private void FixedUpdate()
    {
        AlignToSurface();
        ProcessForce();
        TurnSkateboard();
    }

    private void ProcessInputs()
    {
        m_Forward = Input.GetAxis("Vertical");

        if (Input.GetKey("t") && Time.time - lastTurnTime >= cooldownDuration)
        {
            m_skateboard.Rotate(Vector3.up, 180f);
            lastTurnTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            // Perform a jump tilt when the space bar is pressed
            StartCoroutine(JumpTilt());
        }
    }

    private void ProcessForce()
    {
        if (!m_onSurface)
            return;

        m_rb.AddForce(m_skateboard.forward * m_FwdForce * m_Forward);

        if (isJumping)
        {
            // Apply jump force to lift the skateboard off the ground
            m_rb.AddForce(Vector3.up * jumpForce);
        }
    }

    private void TurnSkateboard()
    {
        float turnInput = Input.GetAxis("Horizontal");
        Quaternion turnRotation = Quaternion.Euler(0, turnInput * m_TurnForce, 0);
        m_skateboard.rotation *= turnRotation;
    }

    private IEnumerator JumpTilt()
    {
        isJumping = true;
        jumpEndTime = Time.time + jumpDuration;

        while (Time.time < jumpEndTime)
        {
            m_skateboard.Rotate(Vector3.right, -jumpTiltAmount * Time.deltaTime);
            yield return null;
        }

        while (Time.time < jumpEndTime + jumpDuration)
        {
            m_skateboard.Rotate(Vector3.right, jumpTiltAmount * Time.deltaTime);
            yield return null;
        }

        isJumping = false;
    }

    private void OnCollisionStay(Collision other)
    {
        m_onSurface = true;
        m_surfaceCollisionInfo = other;
        m_surfaceNormal = other.GetContact(0).normal;
        m_collisionPoint = other.GetContact(0).point;
    }

    private void OnCollisionExit(Collision other)
    {
        m_surfaceCollisionInfo = null;
        m_onSurface = false;
    }

    void AlignToSurface()
    {
        var hit = new RaycastHit();
        var onSurface = Physics.Raycast(transform.position, Vector3.down, out hit, m_rayDistance);
        if (onSurface)
        {
            var localRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            var euler = localRot.eulerAngles;
            euler.y = 0;
            localRot.eulerAngles = euler;
            m_skateboard.localRotation = Quaternion.LerpUnclamped(m_skateboard.localRotation, localRot, m_alignSpeed * Time.fixedDeltaTime);
        }
    }
}

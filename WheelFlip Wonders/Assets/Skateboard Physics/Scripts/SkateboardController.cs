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
    public float m_TurnForce = 5; // Add a turn force
    private Vector3 m_surfaceNormal = new Vector3();
    private Vector3 m_collisionPoint = new Vector3();
    public bool m_useRaycast = true;
    private bool m_onSurface;
    private Collision m_surfaceCollisionInfo;
    private Rigidbody m_rigidbody;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ProcessInputs();
    }

    private void FixedUpdate()
    {
        AlignToSurface();
        ProcessForce();
        TurnSkateboard(); // Call the function to apply turning
    }

    private void ProcessInputs()
    {
        m_Forward = Input.GetAxis("Vertical");
    }

    private void ProcessForce()
    {
        if (!m_onSurface)
            return;

        m_rigidbody.AddForce(m_skateboard.forward * m_FwdForce * m_Forward);
    }

    private void TurnSkateboard()
    {
        // Get the horizontal input for turning
        float turnInput = Input.GetAxis("Horizontal");

        // Calculate the rotation based on the input
        Quaternion turnRotation = Quaternion.Euler(0, turnInput * m_TurnForce, 0);

        // Apply the rotation to the skateboard
        m_skateboard.rotation *= turnRotation;
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
        if (m_useRaycast)
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
        else
        {
            if (m_onSurface)
            {
                var localRot = Quaternion.FromToRotation(transform.up, m_surfaceNormal) * transform.rotation;
                var euler = localRot.eulerAngles;
                euler.y = 0;
                localRot.eulerAngles = euler;
                m_skateboard.localRotation = Quaternion.LerpUnclamped(m_skateboard.localRotation, localRot, m_alignSpeed * Time.fixedDeltaTime);
            }
        }
    }
}

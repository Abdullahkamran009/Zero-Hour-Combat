using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class playermovement : MonoBehaviour
{
    public static playermovement instance;

    public CharacterController controller;
    public float speed = 12f;
    public float sprintspeed = 18f;
    public float gravity = -9.81f;
    public float jumpheight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    [Header("Footstep Settings")]
    public AudioClip[] footstepClips;
    public float walkStepRate = 0.5f;
    public float sprintStepRate = 0.3f;

    private float stepTimer;
    private AudioSource audioSource;



    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

        if (!UI.instance.pauseScreen.activeInHierarchy)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            //sprint implimented
            if (Input.GetKey(KeyCode.LeftShift))
            {
                controller.Move(move * sprintspeed * Time.deltaTime);
            }
            else
            {
                controller.Move(move * speed * Time.deltaTime);
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpheight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            HandleFootsteps(isSprinting);
        }

    }

    void HandleFootsteps(bool isSprinting)
    {
        float stepRate = isSprinting ? sprintStepRate : walkStepRate;

        if (isGrounded && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                stepTimer = stepRate;

                if (footstepClips.Length > 0 && audioSource != null)
                {
                    int rand = Random.Range(0, footstepClips.Length);
                    audioSource.PlayOneShot(footstepClips[rand]);
                }
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace MyCar {
    public class ArcadeVehicleController : MonoBehaviour {
        public enum GroundCheck { RayCast, Sphere };
        public GroundCheck groundCheck;
        public LayerMask drivableSurface;

        public float maxSpeed;
        public float accelaration;
        public float turn;
        public float gravity = 7f;
        public float downforce = 5f;

        public float decelerationMultiplier = 1.25f;

        public bool airControl = false;
        
        public bool driftMode = false;
        public float driftMultiplier = 1.5f;

        public bool aiMode = false;

        public Rigidbody rb, carBody;

        [HideInInspector]
        public RaycastHit hit;

        public AnimationCurve frictionCurve;
        public AnimationCurve turnCurve;
        public PhysicMaterial frictionMaterial;

        public Transform bodyMesh;
        public Transform[] frontWheels = new Transform[2];
        public Transform[] rearWheels = new Transform[2];

        public bool useEffects = false;
        public TrailRenderer RLSkid;
        public TrailRenderer RRSkid;
        public ParticleSystem RLSmoke;
        public ParticleSystem RRSmoke;

        [HideInInspector]
        public Vector3 carVelocity;

        [Range(0, 10)]
        public float bodyTilt;
        public AudioSource engineSound;
        [Range(0, 1)]
        public float minPitch;
        [Range(1, 3)]
        public float maxPitch;
        public AudioSource skidSound;

        private float radius, horizontalInput, verticalInput;
        private Vector3 origin;


        private void Start() 
        {
            radius = rb.GetComponent<SphereCollider>().radius;
        }

        private void Update() 
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            Visuals();
            AudioManager();
        }

        private void FixedUpdate() {
            carVelocity = carBody.transform.InverseTransformDirection(carBody.velocity);

            if (Mathf.Abs(carVelocity.x) > 0) 
                frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));

            if (Grounded()) {
                // Turning
                float sign = Mathf.Sign(carVelocity.z);
                float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / maxSpeed);

                // Drifting Turning
                if (!aiMode && driftMode && Input.GetAxis("Jump") > 0.1f) 
                    TurnMultiplyer *= driftMultiplier;

                if (verticalInput > 0.1f || carVelocity.z > 1)
                {
                    carBody.AddTorque(Vector3.up * horizontalInput * sign * turn * 100 * TurnMultiplyer);
                } 
                else if (verticalInput < -0.1f || carVelocity.z < -1) 
                {
                     carBody.AddTorque(Vector3.up * horizontalInput * sign * turn * 100 * TurnMultiplyer);
                }

                // Brake
                    if (!driftMode) 
                        rb.constraints = Input.GetAxis("Jump") > 0.1f ? RigidbodyConstraints.FreezeRotationX : RigidbodyConstraints.None;

                // Accelaration
                    if (Mathf.Abs(verticalInput) > 0.1f && Input.GetAxis("Jump") < 0.1f && !driftMode)
                        rb.velocity = Vector3.Lerp(rb.velocity, carBody.transform.forward * verticalInput * maxSpeed, accelaration / 10 * Time.deltaTime);
                    else if (Mathf.Abs(verticalInput) > 0.1f && driftMode)
                        rb.velocity = Vector3.Lerp(rb.velocity, carBody.transform.forward * verticalInput * maxSpeed, accelaration / 10 * Time.deltaTime);
               
                    if(Mathf.Abs(verticalInput) < 0.1f && Input.GetAxis("Jump") < 0.1f && decelerationMultiplier > 0f)
                        rb.velocity = rb.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
                

                // Down Force
                if(!aiMode) rb.AddForce(-transform.up * downforce * rb.mass);

                // Body Tilt
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));
            } 
            else 
            {
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
                
                if(useEffects) {
                    RLSkid.emitting = false;
                    RRSkid.emitting = false;
                    RLSmoke.Stop();
                    RRSmoke.Stop();
                }

                if(!aiMode) 
                {
                    if (airControl) 
                    {
                        float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / maxSpeed);
                        carBody.AddTorque(Vector3.up * horizontalInput * turn * 100 * TurnMultiplyer);
                    }
                    rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity + Vector3.down * gravity, Time.deltaTime * gravity);
                }
            }
        }

        private void Visuals() {
            // Wheels
            foreach (Transform wheel in frontWheels) 
            {
                wheel.localRotation = Quaternion.Slerp(wheel.localRotation, Quaternion.Euler(wheel.localRotation.eulerAngles.x, 30 * horizontalInput, wheel.localRotation.eulerAngles.z), 0.7f * Time.deltaTime / Time.fixedDeltaTime);
                wheel.GetChild(0).localRotation = rb.transform.localRotation;
            }
            rearWheels[0].localRotation = rb.transform.localRotation;
            rearWheels[1].localRotation = rb.transform.localRotation;

            // Body
            if (carVelocity.z > 1)
            {
                bodyMesh.localRotation = Quaternion.Slerp(bodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / maxSpeed), bodyMesh.localRotation.eulerAngles.y, bodyTilt * horizontalInput), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            } 
            else 
            {
                bodyMesh.localRotation = Quaternion.Slerp(bodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            }

            if (!aiMode && driftMode) 
            {
                Quaternion quaternion = Quaternion.Euler(0, 0, 0);
                if (Input.GetAxis("Jump") > 0.1f)
                    quaternion = Quaternion.Euler(0, 45 * horizontalInput * Mathf.Sign(carVelocity.z), 0);
                bodyMesh.parent.localRotation = Quaternion.Slerp(bodyMesh.parent.localRotation, quaternion, 0.1f * Time.deltaTime / Time.fixedDeltaTime);
            }

            // Effects
            if(useEffects && Grounded()) 
            {
                if(Mathf.Abs(carVelocity.x) > 10f)
                {
                    RLSkid.emitting = true;
                    RRSkid.emitting = true;
                } else 
                {
                    RLSkid.emitting = false;
                    RRSkid.emitting = false;
                }

                if(Mathf.Abs(carVelocity.x) > 10f && (Input.GetAxis("Jump") > 0.1f || aiMode))
                {
                    RLSmoke.Play();
                    RRSmoke.Play();
                } else 
                {
                    RLSmoke.Stop();
                    RRSmoke.Stop();
                }
            }
        }

        private void AudioManager() 
        {
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(carVelocity.z) / maxSpeed);
            skidSound.mute = Mathf.Abs(carVelocity.x) < 10 && Grounded();
        }
        private bool Grounded() {
            origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
            var direction = -transform.up;
            var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

            if (groundCheck == GroundCheck.RayCast)
                return Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface);
            else if (groundCheck == GroundCheck.Sphere)
                return Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface);
            else 
                return false; 
        }
    }
}
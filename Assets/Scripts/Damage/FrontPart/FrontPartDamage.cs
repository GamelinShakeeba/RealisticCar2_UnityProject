using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontPartDamage : MonoBehaviour
{
    public Door1 door1;
    public ParticleSystem door1Shards;
    private bool hasSetup1 = false;
    private bool isDoor1Detached = false;

    public Door2 door2;
    public ParticleSystem door2Shards;
    private bool hasSetup2 = false;
    private bool isDoor2Detached = false;

    public FrontBumper fb;
    public ParticleSystem fbL;
    public ParticleSystem fbR;
    public bool hasSetupFBJoint = false;
    private bool isFBDetached = false;

    public Hood hood;
    public GameObject smoke1;
    public GameObject smoke2;
    public GameObject fire;
    public bool hasSetupHoodJoint = false;
    private bool isHoodDetached = false;

    public WingL wingL;
    public ParticleSystem wingLShards;
    private bool hasSetupWingL = false;
    private bool isWingLDetached = false;

    public WingR wingR;
    public ParticleSystem wingRShards;
    private bool hasSetupWingR = false;
    private bool isWingRDetached = false;

    public DestroyCar destroyCar;
    public GameObject explosionFire;
    public AudioSource crashSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Building")
        {
            crashSound.Play();

            door1.UpdateMeshSolid1();
            door1.countToBreak++;

            door2.UpdateMeshSolid();
            door2.countToBreak2++;

            fb.DeformFrontBumper();
            fb.countToBreakFrontBumper++;

            hood.DeformHood();
            hood.countToBreakHood++;

            wingL.DeformWingL();
            wingL.countToBreakWingL++;

            wingR.DeformWingR();
            wingR.countToBreakWingR++;

            //DOOR1
            if (door1.countToBreak == 5 && !hasSetup1)
            {
                door1.SetupHingeJoint();
                hasSetup1 = true;
                door1.countToBreak++;
            }

            if (door1.countToBreak == 10 && !isDoor1Detached)
            {
                door1.DetachedDoor1();
                isDoor1Detached = true;
            }

            //DOOR2
            if (door2.countToBreak2 == 4 && !hasSetup2)
            {
                door2.SetupHingeJoint2();
                hasSetup2 = true;
                door2.countToBreak2++;
            }

            if (door2.countToBreak2 == 9 && !isDoor2Detached)
            {
                door2.DetachedDoor2();
                isDoor2Detached = true;
            }

            //FRONT BUMPER
            if (fb.countToBreakFrontBumper == 6 && !hasSetupFBJoint)
            {
                fb.FBHingeJoint();
                hasSetupFBJoint = true;
                StartCoroutine(DetachFrntBumper());
            }

            //HOOD
            if (hood.countToBreakHood == 5)
            {
                smoke1.SetActive(true);
                hood.countToBreakHood++;
            }

            if (hood.countToBreakHood == 8 && !hasSetupHoodJoint)
            {
                smoke1.SetActive(false); 
                smoke2.SetActive(true);
                hood.HoodHingeJoint();
                hasSetupHoodJoint = true;
                hood.countToBreakHood++;
            }

            if (hood.countToBreakHood == 12 && !isHoodDetached)
            {
                smoke2.SetActive(false);
                fire.SetActive(true);
                hood.DetachHood();
                isHoodDetached = true;
                StartCoroutine(CarDeath());
            }

            //LEFT SIDE WING
            if (wingL.countToBreakWingL == 5 && !hasSetupWingL)
            {
                wingL.WingLHingeJoint();
                hasSetupWingL = true;
                StartCoroutine(DetachWingL());
            }

            //RIGHT SIDE WING
            if (wingR.countToBreakWingR == 8 && !hasSetupWingR)
            {
                wingR.WingRHingeJoint();
                hasSetupWingR = true;
                StartCoroutine(DetachWingR());
            }
            ActivateDoorShards();
        }
    }

    private void ActivateDoorShards()
    {
        door1Shards.Play(); // Play the particle system
        door2Shards.Play();
        fbL.Play();
        fbR.Play();
        wingLShards.Play();
        wingRShards.Play(); 
    }

    IEnumerator DetachFrntBumper()
    {
        yield return new WaitForSeconds(2f);
        fb.DetachFB();
        isFBDetached = true;
    }

    IEnumerator DetachWingL()
    {
        yield return new WaitForSeconds(2f);
        wingL.DetachWingL();
        isWingLDetached = true;
    }

    IEnumerator DetachWingR()
    {
        yield return new WaitForSeconds(2f);
        wingR.DetachWingR();
        isWingRDetached = true;
    }

    IEnumerator CarDeath()
    {
        yield return new WaitForSeconds(5f);
        explosionFire.SetActive(true);
        yield return new WaitForSeconds(3f);
        destroyCar.CarDestroy();
    }
}

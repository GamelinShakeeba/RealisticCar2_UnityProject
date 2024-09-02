using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RearPartDamage : MonoBehaviour
{
    public Trunk t;
    public ParticleSystem trunkShards;
    private bool hasTrunkSetup = false;
    private bool isTrunkDetached = false;

    public RearBumper rbumper;
    public bool hasSetupRBJoint = false;
    private bool isRBDetached = false;

    public AudioSource crashSound;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Building")
        {
            crashSound.Play();

            t.DeformTrunk();
            t.countToBreakTrunk++;

            rbumper.DeformRearBumper();
            rbumper.countToBreakRearBumper++;

            //TRUNK
            if (t.countToBreakTrunk == 3 && !hasTrunkSetup)
            {
                t.TrunkHingeJoint();
                hasTrunkSetup = true;
                t.countToBreakTrunk++;
            }

            if (t.countToBreakTrunk == 6 && !isTrunkDetached)
            {
                t.DetachTrunk();
                isTrunkDetached = true;
            }

            //REAR BUMPER
            if (rbumper.countToBreakRearBumper == 3 && !hasSetupRBJoint)
            {
                rbumper.RBHingeJoint();
                hasSetupRBJoint = true;
                StartCoroutine(DetachRBumper());
            }
            ActivateDoorShards();
        }
    }
    private void ActivateDoorShards()
    {
        trunkShards.Play(); // Play the particle system
    }

    IEnumerator DetachRBumper()
    {
        yield return new WaitForSeconds(2f);
        rbumper.DetachRB();
        isRBDetached = true;
    }
}

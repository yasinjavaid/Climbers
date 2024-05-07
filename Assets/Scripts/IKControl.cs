using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

    [SerializeField] private Vector3 rotationRightHandOffset;
    [SerializeField] private Vector3 rotationLeftHandOffset;
    [SerializeField] private Vector3 rotationRightFootOffset;
    [SerializeField] private Vector3 rotationLeftFootOffset;
    //its only for testing
    public HumanBodyBones customBone;
    [SerializeField] private Vector3 rotationCustomBone;
    // keep protected, will use in further extension
    protected Animator animator;
    
    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform leftHandObj = null;
    public Transform leftfootObj = null;
    public Transform RightfootObj = null;
    public Transform lookObj = null;
    public Transform quaternion;
    public bool isKneeRotationControl = false;
    public Transform kneeSample;
    public float offsetDrag;
    public float offsetYYY;
    public float offsetZZZ;
    public float offsetXXX;
    public float offsetXXXLeft;
    public float offsetZZZLeft;
    //its only for testing
    public HumanBodyBones bone;

    private bool isFinalStone = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// its registered with event in IkControlObject class in inspector, bool para its for further extension not using 
    /// with left hand
    /// </summary>
    /// <param name="vvv"></param>
    /// <param name="b"></param>
    public void OnVectorRight(Vector3 vvv,bool b)
    {
        var offset = new Vector3(vvv.x - offsetXXX, vvv.y - offsetYYY, vvv.z + offsetZZZ);
        //transform.DOMove(offset, 0.01f);
        transform.localPosition = Vector3.MoveTowards (transform.localPosition, offset,  offsetDrag);
      //  animator.SetBoneLocalRotation(HumanBodyBones.Hips, Quaternion.LookRotation(vvv-transform.position,Vector3.up));
    }
    /// <summary>
    /// its registered with event in IkControlObject class in inspector, bool para its for further extension not using 
    /// with left hand
    /// </summary>
    /// <param name="vvv"></param>
    /// <param name="b"></param>
    public void OnVectorLeft(Vector3 vvv, bool b)
    {
        var offset = new Vector3(vvv.x - offsetXXXLeft, vvv.y - offsetYYY, vvv.z + offsetZZZLeft);
        //transform.DOMove(offset, 0.01f);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, offset, offsetDrag);
      //  var offset = new Vector3(vvv.x + 0.2f, vvv.y - offsetYYY, transform.localPosition.z);
       // transform.localPosition = Vector3.MoveTowards(transform.localPosition, offset, offsetDrag);
    }
    public void LeftHandOpen() 
    {
      //  animator.SetLayerWeight(1,1f);
    }
    public void LeftHandClose()
    {
     //   animator.SetLayerWeight(1, 0);
    }
    public void RightHandOpen()
    {
     //   animator.SetLayerWeight(2, 1f);
    }
    public void RightHandClose()
    {
     //   animator.SetLayerWeight(2, 0f);
    }
    /// <summary>
    /// look at object changing during run time. its register with event in IkControlObject with all IK bones. 
    /// </summary>
    /// <param name="tt"></param>
    public void ChangeLookObject(Transform tt) 
    {
        lookObj = tt;
    }
    RaycastHit hit;
 
 /*   private void LateUpdate()
    {
        ///Global raycast will use for further extension, if its causing GC then comment it.
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10))
        {
           // Debug.DrawRay(transform.position, -transform.forward * 10, Color.red);
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                var dist = Vector3.Distance(hit.point, transform.position);
                Debug.Log("" + dist);
            }
        }
    }*/
    public void FinalStoneGrabed() 
    {
        isFinalStone = true;
        animator.SetBool("ClimbEnd",true);
        TurnOffIK();
    }
    public void TurnOffIK() 
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);

        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

        animator.SetLookAtWeight(0);
    }
    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (isFinalStone) return;
        if (animator)
        {
            /// controling Ik bones with them
            animator.SetBoneLocalRotation(bone, Quaternion.Euler(rotationCustomBone));
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }
                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.5f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(rotationRightHandOffset));
                }
                //////
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.5f);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(rotationLeftHandOffset));
                }
                //////
                if (RightfootObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0.5f);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, RightfootObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.Euler(rotationRightFootOffset));
                }

                //////
                if (leftfootObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftfootObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.Euler(rotationLeftFootOffset));
                }
            /*  if (isKneeRotationControl)
                {
                    var t = animator.GetBoneTransform(HumanBodyBones.Hips);
                    kneeSample.position = t.position;
                    kneeSample.rotation = t.rotation;
                }
                else 
                {
                    animator.SetBoneLocalRotation(
                        HumanBodyBones.Hips,
                        kneeSample.rotation
                        );
                }*/

            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//부메랑형식의투사체
public class Projectile_Boomerang : ActorProjectile
{
    public enum MoveStepType
    {
        Forward,
        Stay,
        Back,
    }

    

    private Vector3 MoveDir = Vector3.zero;
    private float CurMoveSpeed = 0;

    protected ActorBase TargetActor = null;

    private float HitTime = 0;
    float StayTime = 0;
    private MoveStepType MoveStep = MoveStepType.Forward;

    private Vector3 StartPos = Vector3.zero;

    private List<int> DamagedActorID = new List<int>();

    public override void ReStartEx()
    {
        base.ReStartEx();

        TargetActor = null;
        CurMoveSpeed = this.TableDataInfo.moveSpeed;
        StayTime = 0;

        DamagedActorID.Clear();
    }

    public override void SetDataInfo(ActorBase _ownerActor, ActorBase _targetActor, string _firePosDummyName)
    {
        base.SetDataInfo(_ownerActor, _targetActor, _firePosDummyName);

        TargetActor = _targetActor;
        if (TargetActor != null)
        {
            MoveDir = UtilMath.GetDirection(TargetActor.TF.position, OwnerActor.TF.position);
        }
        else
        {
            MoveDir = OwnerActor.TF.forward;
        }
        if (TableDataInfo.launchAngle > 0)
        {
            Vector3 AngleDir = UtilMath.AxisVector(OwnerActor.TF.forward, TableDataInfo.launchAngle);
            MoveDir = AngleDir;
        }

        this.TF.forward = MoveDir;

        MoveStep = MoveStepType.Forward;

        CurMoveSpeed = this.TableDataInfo.moveSpeed;
        StartPos = this.TF.position;
        StayTime = 0;
    }

    public override void UpdateEx()
    {
        base.UpdateEx();

        if(WillbeDestroy == true)
        {
            return;
        }

        if (TableDataInfo.moveSpeed == 0)
        {
            return;
        }

        switch(MoveStep)
        {
            case MoveStepType.Forward:
                {//처음시전방향으로날아감
                    Vector3 _moveVec = MoveDir * CurMoveSpeed * Time.deltaTime;
                    TF.position += _moveVec;

                    if(Vector3.Distance(StartPos, TF.position) >=  this.TableDataInfo.lifeDistance)
                    {
                        if (this.TableDataInfo.holdPosTime >= 0)
                        {
                            StayTime = this.TableDataInfo.holdPosTime + Time.time;
                            MoveStep = MoveStepType.Stay;
                        }
                        else
                            MoveStep = MoveStepType.Back;
                        StartPos = this.TF.position;

                        DamagedActorID.Clear();
                    }
                }
                break;
            case MoveStepType.Stay:
                {//일정시간대기
                    if (StayTime <= Time.time)
                    {

                        MoveStep = MoveStepType.Back;
                        StartPos = this.TF.position;

                        DamagedActorID.Clear();
                    }
                }break;
            case MoveStepType.Back:
                {//대기후돌아옴
                    MoveDir = UtilMath.GetDirection(OwnerActor.TF.position, this.TF.position);
                    MoveDir.y = 0;

                    Vector3 _moveVec = MoveDir * CurMoveSpeed * Time.deltaTime;
                    TF.position += _moveVec;

                    if (Vector3.Distance(StartPos, TF.position) >= Vector3.Distance(StartPos, OwnerActor.TF.position))
                    {
                        DestroyFireObject();
                    }
                }
                break;

        }

        if (Time.time > HitTime)
        {
            if (MoveStep == MoveStepType.Stay)
                DamagedActorID.Clear();
            HitTime = Time.time + ApplyLevelInfo.damageTerm;

            SkillBase.ProcSkillApplyInfo(OwnerActor, SkillBaseInfo, ApplyLevelInfo, this, DamagedActorID);
        }

    }


}

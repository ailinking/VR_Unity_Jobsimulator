﻿﻿﻿﻿﻿﻿﻿using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class CashRegisterDrawer : MonoBehaviour
{
    [Header("设置")]
    public float snapThreshold = 0.02f; // 距离关闭点多近时自动吸附
    public float lockStrength = 1000f;  // 锁住时的力

    [Header("状态 (Debug)")]
    public bool isLocked = true;

    private ConfigurableJoint joint;
    private Rigidbody rb;
    private Vector3 initialPos;

    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        rb = GetComponent<Rigidbody>();
        initialPos = transform.localPosition;

        // 初始状态：锁定
        LockDrawer();
    }

    void FixedUpdate()
    {
        // 如果当前未被抓取，且非常接近关闭位置，则执行“吸附锁定”
        // 注意：这里需要配合XR Grab Interactable的事件来判断是否被抓取
        // 为简化代码，这里仅演示距离判断

        float dist = Vector3.Distance(transform.localPosition, initialPos);

        if (!isLocked && dist < snapThreshold)
        {
            // 只有当玩家没有用力拉的时候才吸附（可以通过检测速度实现）
            if (rb.linearVelocity.magnitude < 0.1f)
            {
                LockDrawer();
            }
        }
    }

    // 动作 1: 玩家抓取把手时调用 (On Select Enter)
    public void UnlockDrawer()
    {
        isLocked = false;
        // 解锁 Z 轴移动
        var drive = joint.zDrive;
        drive.positionSpring = 0; // 移除弹簧力，允许自由滑动
        joint.zDrive = drive;

        // 唤醒刚体
        rb.isKinematic = false;
    }

    // 动作 2: 锁定逻辑 (内部调用)
    private void LockDrawer()
    {
        isLocked = true;

        // 方法 A: 简单粗暴，直接 Kinematic (最稳，但完全失去物理)
        // rb.isKinematic = true; 

        // 方法 B: 物理锁定 (更高级，允许微小的晃动但打不开)
        // 将弹簧力设为极大，目标位置设为0
        var drive = joint.zDrive;
        drive.positionSpring = lockStrength;
        drive.positionDamper = 50f;
        joint.zDrive = drive;
        joint.targetPosition = Vector3.zero;

        // 播放 "咔哒" 音效
    }
}
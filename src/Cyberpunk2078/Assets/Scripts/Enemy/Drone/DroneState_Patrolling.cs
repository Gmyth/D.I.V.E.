﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "DroneState_Patrolling", menuName = "Enemy State/Drone/Patrolling")]
public class DroneState_Patrolling : ESPatrolling<Drone>
{
    private Animator gunAnimator;

    private bool isCharging = false;


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);

        gunAnimator = enemy.Gun.GetComponent<Animator>();

        isCharging = false;
    }


    protected override void Charge()
    {
        base.Charge();


        enemy.AdjustGunDirection(Mathf.Sign(enemy.transform.right.x) * enemy.currentTarget.transform.position);


        enemy.isGunCharging = true;


        gunAnimator.Play("L2Drone_Gun_Charging");
    }

    protected override void Fire(RangedWeaponConfiguration firingConfiguration)
    {
        LinearMovement bullet = ObjectRecycler.Singleton.GetObject<LinearMovement>(firingConfiguration.BulletID);
        bullet.speed = firingConfiguration.BulletSpeed;
        bullet.initialPosition = firingConfiguration.Muzzle ? firingConfiguration.Muzzle.position : enemy.transform.position;
        bullet.orientation = enemy.GetDeviatedBulletDirection(firingConfiguration.MinDeviationAngle, firingConfiguration.MaxDeviationAngle);

        bullet.GetComponent<Bullet>().isFriendly = false;
        bullet.transform.right = bullet.orientation;

        bullet.gameObject.SetActive(true);


        enemy.isGunCharging = false;


        gunAnimator.Play("L2Drone_Gun_Idle");
    }
}

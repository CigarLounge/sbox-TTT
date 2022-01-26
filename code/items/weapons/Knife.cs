using System;
using System.Collections.Generic;

using Sandbox;

using SWB_Base;

using TTT.Globalization;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
    [Library("weapon_knife")]
    [Weapon(SlotType = SlotType.Melee)]
    [Buyable(Price = 100)]
    [Precached("weapons/swb/hands/swat/v_hands_swat.vmdl", "weapons/swb/melee/bayonet/v_bayonet.vmdl", "weapons/swb/melee/bayonet/w_bayonet.vmdl")]
    [Hammer.EditorModel("weapons/swb/melee/bayonet/w_bayonet.vmdl")]
    public partial class Knife : TTTWeaponBaseEntity
    {
        public override int Bucket => 0;
        public override HoldType HoldType => HoldType.Fists; // just use fists for now
        public override string HandsModelPath => "weapons/swb/hands/swat/v_hands_swat.vmdl";
        public override string ViewModelPath => "weapons/swb/melee/bayonet/v_bayonet.vmdl";
        public override AngPos ViewModelOffset => new()
        {
            Angle = new Angles(0, -15, 0),
            Pos = new Vector3(-4, 0, 0)
        };
        public override string WorldModelPath => "weapons/swb/melee/bayonet/w_bayonet.vmdl";
        public override string Icon => "/swb_weapons/textures/bayonet.png";
        public override int FOV => 75;
        public override float WalkAnimationSpeedMod => 1.25f;
        public override Func<ClipInfo, bool, FiredEntity> CreateEntity => CreateThrowingKnifeEntity;
        public override string EntityModel => "weapons/swb/melee/bayonet/w_bayonet.vmdl";
        public override Vector3 EntityVelocity => new Vector3(0, 0, 3000);
        public override Angles EntityAngles => new Angles(0, 180, 0);
        public override Vector3 EntitySpawnOffset => new Vector3(0, 5, 42);
        public override float PrimaryEntitySpeed => 30;
        public override bool UseGravity => true;

        private static readonly string _stabAnimationHit = "stab";
        private static readonly string _stabAnimationMiss = "stab_miss";
        private static readonly string _stabSound = "bayonet.stab";
        private static readonly string _missSound = "bayonet.slash";
        private static readonly string _hitWorldSound = "bayonet.hitwall";
        private static readonly float _stabSpeed = 1f;
        private static readonly float _stabDamage = 50f;
        private static readonly float _stabForce = 50f;
        private static readonly float _damageDistance = 35f;
        private static readonly float _impactSize = 10f;


        public Knife()
        {

        }

        public override void AttackPrimary()
        {
            MeleeAttack(_stabDamage, _stabForce, _stabAnimationHit, _stabAnimationMiss, _stabSound);
        }

        public void MeleeAttack(float damage, float force, string hitAnimation, string missAnimation, string sound)
        {
            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

            var hitEntity = true;
            var pos = Owner.EyePos;
            var forward = Owner.EyeRot.Forward;
            var trace = Trace.Ray(pos, pos + forward * _damageDistance)
                .Ignore(this)
                .Ignore(Owner)
                .Size(_impactSize)
                .Run();

            if (!trace.Entity.IsValid() || trace.Entity.IsWorld)
            {
                hitAnimation = missAnimation;
                sound = !trace.Entity.IsValid() ? _missSound : _hitWorldSound;
                hitEntity = false;
            }

            DoMeleeEffects(hitAnimation, sound);
            (Owner as AnimEntity).SetAnimBool("b_attack", true);

            if (!hitEntity || !IsServer) return;

            using (Prediction.Off())
            {
                var damageInfo = DamageInfo.FromBullet(trace.EndPos, forward * force, damage)
                    .UsingTraceResult(trace)
                    .WithAttacker(Owner)
                    .WithWeapon(this);

                trace.Entity.TakeDamage(damageInfo);
            }
        }

        [ClientRpc]
        public void DoMeleeEffects(string animation, string sound)
        {
            ViewModelEntity?.SetAnimBool(animation, true);
            PlaySound(sound);
        }

        public override bool CanPrimaryAttack()
        {
            return CanMelee(TimeSincePrimaryAttack, _stabSpeed, InputButton.Attack1);
        }

        public override bool CanSecondaryAttack()
        {
            return CanMelee(TimeSincePrimaryAttack, _stabSpeed, InputButton.Attack2);
        }

        public override void AttackSecondary()
        {
            if (Host.IsServer)
            {
                using (Prediction.Off())
                {
                    FireEntity(Primary, true);
                    Owner.Inventory.Drop(this);
                }
            }
        }

        private FiredEntity CreateThrowingKnifeEntity(ClipInfo clipInfo, bool isPrimary)
        {
            ThrowingKnife knife = new();
            return knife;
        }

        private bool CanMelee(TimeSince lastAttackTime, float attackSpeed, InputButton inputButton)
        {
            if (IsAnimating) return false;
            if (!Owner.IsValid() || !Input.Down(inputButton)) return false;

            return lastAttackTime > attackSpeed;
        }
    }

    public class ThrowingKnife : FiredEntity, IEntityHint
    {
        public float HintDistance => 80f;

        public TranslationData TextOnTick => TTTWeaponBaseGeneric.PickupText("weapon_knife");

        public bool CanHint(TTTPlayer client)
        {
            return true;
        }

        public EntityHintPanel DisplayHint(TTTPlayer client)
        {
            return new Hint(TextOnTick);
        }

        public void Tick(TTTPlayer player)
        {
            if (Host.IsClient)
            {
                return;
            }

            if (player.LifeState != LifeState.Alive)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Use))
                {
                    player.Inventory.TryAdd(new Knife(), deleteIfFails: false, makeActive: true);
                    Delete();
                }
            }
        }
    }
}

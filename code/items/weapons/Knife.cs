using Sandbox;

using SWB_Base;

using TTT.Player;

namespace TTT.Items
{
    [Library("weapon_knife")]
    [Weapon(SlotType = SlotType.Melee)]
    [Buyable(Price = 100)]
    [Precached("weapons/swb/hands/swat/v_hands_swat.vmdl", "weapons/swb/melee/bayonet/v_bayonet.vmdl", "weapons/swb/melee/bayonet/w_bayonet.vmdl")]
    [Hammer.EditorModel("weapons/swb/melee/bayonet/w_bayonet.vmdl")]
    public class Knife : TTTWeaponBaseMelee
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

        public override string SwingAnimationHit => "stab";
        public override string SwingAnimationMiss => "stab_miss";
        public override string StabAnimationHit => "stab";
        public override string StabAnimationMiss => "stab_miss";
        public override string SwingSound => "bayonet.stab";
        public override string StabSound => "bayonet.stab";
        public override string MissSound => "bayonet.slash";
        public override string HitWorldSound => "bayonet.hitwall";
        public override float SwingSpeed => 1f;
        public override float StabSpeed => 1f;
        public override float SwingDamage => 200f;
        public override float StabDamage => 50f;
        public override float SwingForce => 25f;
        public override float StabForce => 50f;
        public override float DamageDistance => 35f;
        public override float ImpactSize => 10f;

        private readonly float _primaryEntitySpeed = 300f;
        private Vector3 _entityVelocity = new(0, 0, 30);
        private Vector3 _entitySpawnOffset = new(0, 5, 42);
        private Angles _entityAngles = new(0, 0, 0);

        public Knife()
        {

        }

        public override void MeleeAttack(float damage, float force, string hitAnimation, string missAnimation, string sound)
        {
            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

            var hitEntity = true;
            var pos = Owner.EyePos;
            var forward = Owner.EyeRot.Forward;
            var trace = Trace.Ray(pos, pos + forward * DamageDistance)
                .Ignore(this)
                .Ignore(Owner)
                .Size(ImpactSize)
                .Run();

            if (!trace.Entity.IsValid() || trace.Entity.IsWorld)
            {
                hitAnimation = missAnimation;
                sound = !trace.Entity.IsValid() ? MissSound : HitWorldSound;
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

                if (trace.Entity is TTTPlayer)
                {
                    Owner.Inventory.Drop(this);
                    Delete();
                }
            }
        }

        public override void AttackSecondary()
        {
            if (Host.IsServer)
            {
                using (Prediction.Off())
                {
                    ThrowKnife(new ThrownKnife());
                    Owner.Inventory.Drop(this);
                }
            }
        }

        public void ThrowKnife(ThrownKnife knife)
        {
            if (!string.IsNullOrEmpty(WorldModelPath))
                knife.SetModel(WorldModelPath);

            knife.Owner = Owner;
            knife.Position = MathUtil.RelativeAdd(Position, _entitySpawnOffset, Owner.EyeRot);
            knife.Rotation = Owner.EyeRot * Rotation.From(_entityAngles);
            knife.RemoveDelay = -1;
            knife.UseGravity = true;
            knife.Speed = _primaryEntitySpeed;
            knife.IsSticky = false;
            knife.Damage = Primary.Damage;
            knife.Force = Primary.Force;
            knife.StartVelocity = MathUtil.RelativeAdd(Vector3.Zero, _entityVelocity, Owner.EyeRot);
            knife.Start();
        }

        public class ThrownKnife : FiredEntity
        {
            protected override void OnPhysicsCollision(CollisionEventData eventData)
            {
                if (eventData.Entity == Owner) return;

                Log.Info("hit");
            }
        }
    }
}

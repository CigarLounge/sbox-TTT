﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SWB_Base
{
    [UseTemplate]
    public class StatsDisplay : Panel
    {
        public Panel AccuracyDefaultP { get; set; }
        public Panel AccuracyDiffP { get; set; }
        public Panel DamageDefaultP { get; set; }
        public Panel DamageDiffP { get; set; }
        public Panel RangeDefaultP { get; set; }
        public Panel RangeDiffP { get; set; }
        public Panel RecoilDefaultP { get; set; }
        public Panel RecoilDiffP { get; set; }
        public Panel RateDefaultP { get; set; }
        public Panel RateDiffP { get; set; }

        private WeaponBase weapon;

        public StatsDisplay(WeaponBase weapon) : base()
        {
            this.weapon = weapon;
        }

        public override void Tick()
        {
            var player = Local.Pawn as PlayerBase;
            if (player == null) return;

            if (weapon == null)
            {
                Delete(true);
                return;
            }

            // Accuracy
            var scaleMultiplier = 4; // Better display
            var accuracy = weapon.Primary.Spread * scaleMultiplier;
            var diffAccuray = Math.Abs(weapon.InitialStats.Spread * scaleMultiplier - accuracy);
            var isPosDiff = weapon.InitialStats.Spread * scaleMultiplier > accuracy;
            var defaultAccuracy = isPosDiff ? accuracy + diffAccuray : accuracy;
            AccuracyDefaultP.Style.Width = Length.Fraction(Math.Clamp(1 - defaultAccuracy, 0, 1));
            AccuracyDiffP.Style.Width = Length.Fraction(diffAccuray);
            AccuracyDiffP.SetClass("pos", isPosDiff);
            AccuracyDiffP.SetClass("neg", !isPosDiff);

            // Damage
            var damage = weapon.Primary.Damage;
            var diffDamage = Math.Abs(weapon.InitialStats.Damage - damage);
            isPosDiff = weapon.InitialStats.Damage < damage;
            var defaultDamage = isPosDiff ? damage - diffDamage : damage;
            DamageDefaultP.Style.Width = Length.Fraction(defaultDamage / 100);
            DamageDiffP.Style.Width = Length.Fraction(diffDamage / 100);
            DamageDiffP.SetClass("pos", isPosDiff);
            DamageDiffP.SetClass("neg", !isPosDiff);

            // Range (WIP)
            RangeDefaultP.Style.Width = Length.Fraction(1f);

            // Recoil
            scaleMultiplier = 3;
            var recoil = weapon.Primary.Recoil;
            var diffRecoil = Math.Abs(weapon.InitialStats.Recoil - recoil);
            isPosDiff = weapon.InitialStats.Recoil < recoil;
            var defaultRecoil = isPosDiff ? recoil - diffRecoil : recoil;
            RecoilDefaultP.Style.Width = Length.Fraction(defaultRecoil / scaleMultiplier);
            RecoilDiffP.Style.Width = Length.Fraction(diffRecoil / scaleMultiplier);
            RecoilDiffP.SetClass("pos", isPosDiff);
            RecoilDiffP.SetClass("neg", !isPosDiff);

            // RPM
            scaleMultiplier = 1600;
            float rpm = weapon.Primary.RPM;
            float diffRPM = Math.Abs(weapon.InitialStats.RPM - rpm);
            isPosDiff = weapon.InitialStats.RPM < rpm;
            float defaultRPM = isPosDiff ? rpm - diffRPM : rpm;
            RateDefaultP.Style.Width = Length.Fraction(defaultRPM / scaleMultiplier);
            RateDiffP.Style.Width = Length.Fraction(diffRPM / scaleMultiplier);
            RateDiffP.SetClass("pos", isPosDiff);
            RateDiffP.SetClass("neg", !isPosDiff);
        }
    }
}

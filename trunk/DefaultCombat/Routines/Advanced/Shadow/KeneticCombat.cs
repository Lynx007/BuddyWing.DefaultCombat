﻿using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    class KeneticCombat : RotationBase
    {
        public override string Name { get { return "Shadow Kenetic Combat"; } }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Combat Technique"),
                    Spell.Buff("Force Valor"),
                    Spell.Cast("Guard", on => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard")),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Kinetic Ward", ret => Me.BuffCount("Kinetic Ward") <= 1 || Me.BuffTimeLeft("Kinetic Ward") < 3),
                    Spell.Buff("Force of Will"),
                    Spell.Buff("Battle Readiness", ret => Me.HealthPercent <= 85),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Resilience", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Force Potency")
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new LockSelector(
                    Spell.Buff("Force Speed", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Rotation
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent < 25),
                    Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && !DefaultCombat.MovementDisabled),
                    Spell.Cast("Telekinetic Throw", ret => Me.BuffCount("Harnessed Shadows") == 3),
                    Spell.Cast("Slow Time"),
                    Spell.Cast("Project", ret => Me.HasBuff("Particle Acceleration")),
                    Spell.Cast("Shadow Strike", ret => Me.HasBuff("Shadow Wrap")),
                    Spell.Cast("Spinning Strike", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Force Breach"),
                    Spell.Cast("Double Strike"),
                    Spell.Cast("Force Speed", ret => Me.CurrentTarget.Distance >= 1.1f && Me.IsMoving && Me.InCombat)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPBAOE,
                    new LockSelector(
                        Spell.Cast("Slow Time"),
                        Spell.Cast("Force Breach"),
                        Spell.Cast("Whirling Blow", ret => Me.ForcePercent >= 60 && Me.CurrentTarget.Distance <= 0.5f)
                    ));
            }
        }
    }
}
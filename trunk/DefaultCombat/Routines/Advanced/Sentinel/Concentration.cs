﻿// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Concentration : RotationBase
	{
		public override string Name
		{
			get { return "Sentinel Concentration"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Force Might")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Rebuke", ret => Me.HealthPercent <= 50),
					Spell.Buff("Guarded by the Force", ret => Me.HealthPercent <= 10),
					Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					//Movement
					CombatMovement.CloseDistance(Distance.Melee),
					
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

					//Rotation
					Spell.Cast("Force Leap"),
					Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Force Sweep", ret => Me.HasBuff("Singularity") && Me.HasBuff("Felling Blow")),
					Spell.Cast("Force Exhaustion"),
					Spell.Cast("Zealous Leap", ret => Me.HasBuff("Singularity")),
					Spell.Cast("Blade Storm", ret => Me.HasBuff("Battle Cry") || Me.Energy >= 5),
					Spell.Cast("Dual Saber Throw"),
					Spell.Cast("Blade Barrage"),
					Spell.Cast("Force Stasis"),
					Spell.Cast("Slash", ret => Me.HasBuff("Zen")),
					Spell.Cast("Zealous Strike"),
					Spell.Cast("Strike")
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldPbaoe,
					new PrioritySelector(
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 4f) //--will only be active when user initiates Heroic Moment--
						));
			}
		}
	}
}

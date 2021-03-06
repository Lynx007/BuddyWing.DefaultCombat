﻿// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	public class InnovativeOrdnance : RotationBase
	{
		public override string Name
		{
			get { return "Mercenary Innovative Ordnance"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Hunter's Boon")
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Determination", ret => Me.IsStunned),
					Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
					Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 50),
					Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),
					Spell.Cast("Responsive Safeguards", ret => Me.HealthPercent <= 20),
					Spell.Cast("Unity", ret => Me.HealthPercent <= 15),
					Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(

					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),
					new Decorator(ret => Me.ResourcePercent() > 40,
					new PrioritySelector(Spell.Cast("Rapid Shots"))),
							
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),
					
					//Solo Mode
					Spell.Cast("Kolto Shot", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 70),
					Spell.Cast("Emergency Scan", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 60),
					Spell.Cast("Rapid Scan", ret => CombatHotkeys.EnableSolo && Me.HealthPercent <= 50),

					//Rotation
					Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.DoT("Incendiary Missile", "", 12000),
					Spell.Cast("Thermal Detonator"),
					Spell.Cast("Electro Net"),
					Spell.Cast("Unload"),
					Spell.Cast("Power Shot", ret => Me.HasBuff("Speed to Burn")),
					Spell.Cast("Mag Shot", ret => Me.HasBuff("Innovative Particle Accelerator")),
					Spell.Cast("Rapid Shots"),

					//HK-55 Mode Rotation
					Spell.Cast("Charging In", ret => Me.CurrentTarget.Distance >= .4f && Me.InCombat && CombatHotkeys.EnableHK55),
					Spell.Cast("Blindside", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Assassinate", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Rail Blast", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Rifle Blast", ret => CombatHotkeys.EnableHK55),
					Spell.Cast("Execute", ret => Me.CurrentTarget.HealthPercent <= 45 && CombatHotkeys.EnableHK55)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
						Spell.CastOnGround("Death from Above"),
						Spell.Cast("Fusion Missle", ret => Me.HasBuff("Thermal Sensor Override")),
						Spell.Cast("Explosive Dart"))
					);
			}
		}
	}
}

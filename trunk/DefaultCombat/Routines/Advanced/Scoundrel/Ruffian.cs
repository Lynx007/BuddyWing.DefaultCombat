﻿// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    internal class Ruffian : RotationBase
    {
        public override string Name
        {
            get
            {
                return "Scoundrel Ruffian";
            }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Lucky Shots"),
                    Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled));
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned), 
                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 45), 
                    Spell.Buff("Pugnacity", ret => !Me.HasBuff("Upper Hand") && Me.InCombat), 
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 75), Spell.Buff("Dodge", ret => Me.HealthPercent <= 50), 
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15), 
                    Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5));
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
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    new Decorator(ret => Me.EnergyPercent < 60, new PrioritySelector(Spell.Cast("Flurry of Bolts"))),

                    //Heals
                    Spell.Cast("Diagnostic Scan", ret => Me.HealthPercent <= 70),
                    Spell.Cast("Slow-release Medpac", ret => Me.HealthPercent <= 60),
                    Spell.Cast("Kolto Pack", ret => Me.HealthPercent <= 50),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Brutal Shots", ret => Me.CurrentTarget.HasDebuff("Vital Shot") && Me.CurrentTarget.HasDebuff("Shrap Bomb") && Me.HasBuff("Upper Hand")),
                    Spell.Cast("Sanguinary Shot", ret => Me.CurrentTarget.HasDebuff("Vital Shot") && Me.CurrentTarget.HasDebuff("Shrap Bomb")),
                    Spell.DoT("Vital Shot", "Vital Shot"), Spell.DoT("Shrap Bomb", "Shrap Bomb"),
                    Spell.Cast("Blaster Whip", ret => Me.BuffCount("Upper Hand") < 2 || Me.BuffTimeLeft("Upper Hand") < 6),
                    Spell.Cast("Point Blank Shot", ret => Me.Level >= 57),
                    Spell.Cast("Back Blast", ret => Me.IsBehind(Me.CurrentTarget) && Me.Level < 57),
                    Spell.Cast("Quick Shot"),

                    //HK-55 Mode Rotation
                    Spell.Cast("Charging In", ret => Me.CurrentTarget.Distance >= .4f && Me.InCombat && CombatHotkeys.EnableHK55),
                    Spell.Cast("Blindside", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Assassinate", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rail Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Rifle Blast", ret => CombatHotkeys.EnableHK55),
                    Spell.Cast("Execute", ret => Me.CurrentTarget.HealthPercent <= 45 && CombatHotkeys.EnableHK55));
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe, new PrioritySelector(
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")), //--will only be active when user initiates Heroic Moment--
                    Spell.CastOnGround("Terminate", ret => CombatHotkeys.EnableHK55), //--will only be active when user initiates HK-55 Mode
                    Spell.DoT("Shrap Bomb", "Shrap Bomb"), Spell.Cast("Thermal Grenade"), Spell.Cast("Bushwhack", ret => !Me.HasBuff("Upper Hand")), Spell.Cast("Lacerating Blast")));
            }
        }
    }
}
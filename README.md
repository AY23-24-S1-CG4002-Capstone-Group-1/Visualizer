# Lazer Tag AR Game Visualizer

The Lazer Tag AR Game Visualizer is a Unity-based interface designed to bring augmented reality (AR) interactions to the classic game of laser tag. It utilizes Vuforia Engine for AR functionalities, enabling players to engage with virtual elements overlaid on real-world environments. This project contains scripts and assets necessary for creating a fully immersive AR laser tag experience, where players can use virtual weapons, shields, and special effects within the game.

Please note that due to the size constraints of Git, the complete project with all assets and libraries is not hosted on this repository. This repository contains essential scripts and assets that demonstrate the core functionality of the visualizer.

## Scripts Overview
1. CustomAREffects.cs: Handles the instantiation and behavior of AR effects.<br>
2. GunController.cs, GrenadeController.cs, ShieldController.cs: Manage the player's interactions with the respective weapons and shields.<br>
3. PlayerState.cs: Manages the player's health and status within the game.<br>
4. ScoreboardController.cs: Updates and displays the player's score.<br>
5. SpecialAttacksAREffects.cs: Controls the special attack animations and effects.(Not used anymore) <br>

## Prefabs
1. GunWrapper: The gun object with associated shooting mechanics.<br>
2. GrenadeWrapper: The grenade object used for throwing grenades in the game.<br>
3. ShieldWrapper: The shield object that players can activate for protection.<br>
4. SpearWrapper: The spear object that can be thrown at opponents.<br>
5. SpiderWeb: The prefab used for the web shooting effect in special attacks.<br>
6. PortalWrapper: The portal effect used for teleporting or special entries.<br>
7. MuzzleFlashLight: Visual effect for gun muzzle flash.<br>
8. OpponentShieldWrapper: Shield object for the opponent player.<br>
9. ExplosionPrefab: Visual effect prefab for grenade explosions.<br>
10. BloodSprayPs: Particle system prefab for blood spray effect on hit.<br>
11. HammerWrapper: The prefab for a throwable hammer.

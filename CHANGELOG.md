
<!-- TOC -->

- [v1.3.1](#v131)
- [v1.3.0](#v130)
- [v1.2.1](#v121)
- [v1.2.0](#v120)
- [v1.1.0](#v110)
- [v1.0.0](#v100)

<!-- /TOC -->

## v1.3.1
* German/Deutsch translation, courtesy Kazel
* Bugfix for #3, "Cannot upgrade other buildings while mod is installed" 
## v1.3.0 
* Content Patcher token now available
* Moved to using a Translation framework
  * French now available
  * Portuguese now available
* Option added to only allow so many upgrades
* Now Uses Harmony
* Got rid of the "Time To Upgrade" setting in config
  * Wasn't working well with changes required to make Harmony work
* Now works with Instant Build mods!
* Now uses the `modData` dictionary provided to modders by ConcernedApe
* Two new Console Commands:
  *  `ghs_setlevel [value]` Sets the greenhouse to the provided level
  * `ghs_getlevel` prints out the current Greenhouse level (great for debugging)
* `ModData` has been slimmed down.
  * Will be completely removed in future release

## v1.2.1
 * Can Now Turn off watering on the sandy areas of the Beach Farm

## v1.2.0

* Upgrades are now applied to the Greenhouse and not a Silo. 
  * Breaks SDV 1.4 Compatibility
* Visual Upgrades, when viewed from the outside sprinklers are visible in the rafters of the greenhouse.
* Additional config option "ShowVisualUpgrades" set this to false to only use the gameplay effects of this mod.

## v1.1.0

* Upgrades are now gated behind friendship with Wizard (as he's translating ideas the Junimos had)
* More difficult to unlock if you went the Joja Route, cannot get first upgrade until you have the required hearts AND have at least 1 Junimo Hut (you did kinda mess up the Community Center)
* Wizard now sends you letters telling you upgrades are available.
* Upgrades now take the correct amount of time to be built
* Upgrades cannot be purchased until Greenhouse unlocked

## v1.0.0
* Initial Release
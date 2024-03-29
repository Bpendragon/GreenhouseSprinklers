<!-- TOC -->

- [v3.0.1](#v301)
- [v3.0.0 - SDV 1.6/SMAPI 4.0 Update](#v300---sdv-16smapi-40-update)
- [v2.1.1](#v211)
- [v2.1.0](#v210)
- [v2.0.0](#v200)
- [v1.4.3](#v143)
- [v1.4.2](#v142)
- [v1.4.1](#v141)
- [v1.4.0 - Translations Update](#v140---translations-update)
- [v1.3.3](#v133)
- [v1.3.2](#v132)
- [v1.3.1](#v131)
- [v1.3.0 - Harmony Update](#v130---harmony-update)
- [v1.2.1](#v121)
- [v1.2.0 - SDV 1.5 Update](#v120---sdv-15-update)
- [v1.1.0 - Friendship Update](#v110---friendship-update)
- [v1.0.0](#v100)

<!-- /TOC -->

## v3.0.1
* Add simple forgotten check that the greenhouse is even built before sending the Wizard mail.

## v3.0.0 - SDV 1.6/SMAPI 4.0 Update
* Rewrote 60%+ of the mod
* Now Compatible with SDV 1.6 (and incompatible with SDV 1.5 or lower)
* Functionality is the same, just using way less jank means of adding the items to Robin's Shop (which may make it more stable)
* Old Saves should work, but this is not guaranteed.
## v2.1.1
* Fixed letters not getting added to the dictionary and thus never getting sent.
## v2.1.0
* Updated i18n generator to most recent version
* Updated Asset handling for SMAPI 4.0 Update and SDV 1.6
## v2.0.0
* Fully removed `ModData` file
  * This should stop any "failed on GameLoop Save" issues. 
* Technically a breaking change if you only played with v1.2.1 and earlier.
  * Any save that played at least once with 1.3.x+ should be good to go.
## v1.4.3
* Added Hungarian Translation from [Martin66789](https://www.nexusmods.com/stardewvalley/users/27323031)
## v1.4.2
* Added Korean Translation from [Wally232](https://github.com/Wally232)
* Added Russian Translation from  [CatMartin](https://github.com/CatMartin)
## v1.4.1
* Fixed issue where Robin would claim the Greenhouse Upgrade was a prefab.
## v1.4.0 - Translations Update
* Added Translations
  * Italian - [Leecanit](https://github.com/LeecanIt)
  * Chinese - [Cccchelsea226](https://github.com/Cccchelsea226)
  * Spanish - [ManHeII](https://github.com/ManHeII)
* Added console command `ghs_waternow`
* Removed `DaysToConstruct` from blueprints. (will still exist in config files for now)
* Added documentation for console commands
* Added update token for github

## v1.3.3
* More stable method of using and converting ModData used.
  * ModData is 
* Fixed issue where upgrades would disappear on mod upgrade.
* Fixed issue where the Wizard would not send letters as needed.

## v1.3.2
* Changed method of getting the Greenhouse
## v1.3.1
* German/Deutsch translation, courtesy Kazel
* Bugfix for #3, "Cannot upgrade other buildings while mod is installed" 
## v1.3.0 - Harmony Update
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

## v1.2.0 - SDV 1.5 Update

* Upgrades are now applied to the Greenhouse and not a Silo. 
  * Breaks SDV 1.4 Compatibility
* Visual Upgrades, when viewed from the outside sprinklers are visible in the rafters of the greenhouse.
* Additional config option "ShowVisualUpgrades" set this to false to only use the gameplay effects of this mod.

## v1.1.0 - Friendship Update

* Upgrades are now gated behind friendship with Wizard (as he's translating ideas the Junimos had)
* More difficult to unlock if you went the Joja Route, cannot get first upgrade until you have the required hearts AND have at least 1 Junimo Hut (you did kinda mess up the Community Center)
* Wizard now sends you letters telling you upgrades are available.
* Upgrades now take the correct amount of time to be built
* Upgrades cannot be purchased until Greenhouse unlocked

## v1.0.0
* Initial Release
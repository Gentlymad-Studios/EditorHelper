# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - 2022-11-02
### Added
- Initial release

## [0.1.1] - 2022-12-05
### Added
- generic utility method to create scriptable object based settings files in a predefined folder (Tools/_Settings)
- added path consts to folders we expect to create or to exist

## [0.1.2] - 2022-12-07
### Added
- changes to meta files

## [0.1.3] - 2023-03-29
### Added
- settingsprovider for scriptable singletons

## [0.1.4] - 2023-06-07
### Added
- method to get custom attribute from property

## [0.1.5] - 2023-06-23
### Added
- added ParticleSystemUtility.cs
- added editor folder and new assembly definition for editor only classes

## [0.1.6] - 2023-07-19
### Added
- added AudioUtility.cs

## [0.1.7] - 2023-07-25
### Added
- added ObjectSelectorWrapper.cs

## [0.1.8] - 2023-08-04
### Added
- added HandleUtilityWrapper
- added SceneHierarchyWrapper
- added MeshUtility
- added AssetUtility
- added SceneUtility
- added MeshRendererExtension
### Changed
- rename AudioUtility to AudioUtilWrapper

## [0.1.9] - 2023-08-09
### Added
- added Mesh List to SceneUtility

## [0.2.0] - 2023-08-10
### Added
- added MeshDrawer
### Changed
- SceneUtility now stores MeshData in combination with an world transform

## [0.2.1] - 2023-08-15
### Changed
- MeshDrawer now recognize scale

## [0.2.2] - 2023-09-13
### Changed
- MeshDrawer got an scale override

## [0.2.3] - 2023-09-19
### Added
- add update method to MeshVisual

## [0.2.4] - 2023-09-19
### Added
- add materialPropertyBlock support to MeshVisual

## [0.2.5] - 2023-11-09
### Added
- MeshInstanceDrawer

## [0.2.6] - 2023-11-10
### Added
- add AnnotationUtilityWrapper

## [0.2.7] - 2023-11-22
## Added
- added AdvancedSettings and AdvancedSingletonProviderBase to further minimize the amount of boilerplate code needed to create custom project settings
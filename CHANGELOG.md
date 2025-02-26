Changelog
=========

[1.0.6] - 2025.02.26
--------------------
* **Added**
    * OptionsProviderBase added method DeletePref
* **Changed** -
    * Changed access for OptionsProviderBase members
* **Fixed** -

[1.0.5] - 2024.08.19
--------------------
* **Added** -
* **Changed** -
* **Fixed**
    * Fixed popups closing flow

[1.0.4] - 2023.12.14
--------------------
* **Added** -
* **Changed**
    * Changed access for OptionsProviderBase.ContainsPref
* **Fixed** -

[1.0.3] - 2023.12.04
--------------------
* **Added** -
    * [Editor]**SceneAssetPostprocessor** - detect scene rename and call SceneNameDependentScriptableObject.AnySceneNameChanged
    * **SceneNameDependentScriptableObject** - Scene Name Dependent ScriptableObject 
* **Changed**
    * [Editor] PlayerPrefsEditor and PlayerPrefStore moved to namespace UAppToolKit.Core.Editor.PlayerPrefsTool
* **Fixed** -

[1.0.2] - 2023.11.30
--------------------
* **Added** -
* **Changed**
    * EntryPointBase initializing logic
* **Fixed** -

[1.0.1] - 2023.11.30
--------------------
* **Added** -
* **Changed**
    * Access permission for EntryPointBase.InitializeApplication
* **Fixed** -

[1.0.0] - 2023.11.27
--------------------
* **Added**
    * **EntryPointBase** (MonoBehaviour) as base abstract class of application entry point (Singleton) 
    * **NavigationController** (NavigationControllerBase > MonoBehaviour) - application navigation service
    * **PageBase** (Navigator > MonoBehaviour) - page controller
    * **PageBaseLink** (ScriptableObject) - link to page
    * **PopUpBase** (PopUpAnimationController > MonoBehaviour) - base class for popup controller
    * **LoadingScreen** (MonoBehaviour) - loading screen for application initializing
    * **SplashScreenPageBase** (MonoBehaviour) - splash screen for pages navigation
    * **DispatcherTimer** (MonoBehaviour) - runs code with a delay
    * **Storyboard** (MonoBehaviour), DoubleAnimation, Vector2Animation, Vector3Animation, Vector4Animation, QuaternionAnimation, BackEase, BounceEase, CircleEase, CubicEase, CurveEase, ElasticEase, ExponentialEase, Linear, PowerEase, QuadraticEase, QuarticEase, QuinticEase, SineEase, EasingModes - code-base animation system
    * **MediaPlayerBase** (MonoBehaviour) - abstract class for media player system
    * **OptionsProviderBase** - abstract class for application state loading/saving (based on ***PlayerPrefs***)
    * **PointerManipulationReporter** (MonoBehaviour), Pointer, PointerActionEnum - low level input manipulation system
    * **GameObjectExtendedMethods** - GameObject extended methods
    * **ListExtendedMethods** - C# List extended methods
    * Assembly definitions for ***Runtime*** and ***Editor*** package part
    * **StartApplicationSample** - example on using the entry point and navigation between pages
* **Changed** -
* **Fixed** -
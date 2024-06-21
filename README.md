# AddButtonToUI

A library mod for [Phantom Brigade](https://braceyourselfgames.com/phantom-brigade/)  that demonstrates how to add a button to the pilot screen in the mobile base.

It is compatible with game release version **1.3.3**. It works with both the Steam and Epic installs of the game. All library mods are fragile and susceptible to breakage whenever a new version is released.

![Custom button in the mobile base pilot screen]()

The screens in the mobile base are an obvious thing to want to modify but it can be hard to understand where to start with the UI code. This mod is a simple example that introduces some of the key concepts to the UI.

The UI is custom made for this game. It uses [NGUI 3](http://www.tasharen.com/forum/index.php?board=12.0) for its basic widget set and builds more complete UI components with those widgets. Classes that start with a `UI` prefix are from NGUI while classes that start with a `CI` prefix are game-specific components.

There are two separate hierarchies you will need to keep track of. One is the standard Unity transform hierarchy for game objects. UI components are usually `MonoBehaviours` so they're components on a `GameObject` and therefore have a transform and are found somewhere in the game object hierarchy. NGUI maintains a separate widget hierarchy that doesn't necessarily mirror the game object hierarchy. This widget hierarchy is used to group widgets for drawing and placement purposes.

All UI objects should be in layer 5. The NGUI widgets use normal Unity colliders for hit testing and these need to be kept separate from the colliders used by the rest of the game. Each NGUI widget has a depth value which determines the relative top-to-bottom ordering of the widgets. NGUI widgets can be contained by `UIPanel` objects and in that case the widget's depth is relative to other widgets in the panel.

At the top of the UI hierarchy is `UI_Prototype` which has a `UIRoot` component. All NGUI widgets must be descendants of `UIRoot`. The screen is split into eight major regions: `Anchor_Top`, `Anchor_Left`, `Anchor_Right`, `Anchor_Bottom`, `Anchor_TopLeft`, `Anchor_TopRight`, `Anchor_BottomLeft` and `Anchor_BottomRight`. They all start with `Anchor_` because NGUI uses an unusual layout system based around anchors. The idea is that you can place your widgets relative to one another by anchoring (or attaching) a side of a new widget to a side of an existing one and then computing the rest of the dimensions for the new widget relative to the attached side. The eight screen regions give you initial widgets to anchor your new widgets to. This makes it easy to add widgets to specific edges of the screen without having to calculate the screen coordinates.

This mod takes a shortcut instead of building up a button from scratch. There is an edit button already on the screen so the mod clones that button and then changes the position and text of the clone. Buttons are fairly complicated to configure in code so this is a smart way to quickly get a fully functional button with minimal effort.

One last technique demonstrated in the code is how to use the text handling system built into the game to avoid hardcoding English strings for things like the button text. This game has an international audience and setting up the UI for localization is easy so there's no reason to bake in English text.

<div align="center">

<h1>Board Pet AR</h1>

[![Unity][badge-unity]](https://unity.com/)
[![Vuforia][badge-vuforia]](https://developer.vuforia.com/)
[![Android][badge-android]](#android)
[![iOS][badge-ios]](#ios-sideload)
[![Git LFS][badge-lfs]](https://git-lfs.com/)

An **Augmented Reality** virtual pet that lives on a whiteboard. Point your phone at a marker card taped to the board, and a small cave opens on the marker ledge. A cat hops out of a glowing portal, walks along the ledge on its own, gets hungry over time, and answers to three buttons: **Feed**, **Play** and **Sleep**.

</div>

> [!NOTE]
> Individual assignment for **Virtual and Augmented Reality (IMT02207611)**, Universitas Ciputra Makassar (Unity + Vuforia Engine).
>
> **Only want to try the app?** Jump to [Install on Your Phone](#install-on-your-phone). You don't need Unity for that.

<details>
<summary><b>Features</b></summary>

Tracking uses **natural feature tracking** on a printed card, so no special marker type is needed.

| Feature | How it works |
| --- | --- |
| **Image target tracking** | Vuforia recognizes the card and anchors everything to it. The content is rotated so that its up axis is real-world up for a card that stands **upright** on a wall. |
| **Cave and portal intro** | When the card is found, a rock cave grows on the ledge, a cyan portal opens in its arch, and the cat hops out in a puff of smoke. About 2.8 seconds, played **once per app session**. |
| **Intro only while you can see it** | The intro only runs while the card is really in view. If you look away before it finishes, it resets and plays again from the start when the card is back. After it finishes, nothing is reset. |
| **Wandering cat** | The cat walks back and forth along the ledge, pauses at each end and turns so its face passes the camera. |
| **Hunger** | Hunger goes down over time (20 seconds in this demo build). When the cat is hungry it stops and shows a salmon icon above its head. |
| **Feed** | The cat eats a salmon slice bite by bite, shows a heart, and hunger resets. The phone vibrates when the meal is finished. |
| **Play** | The cat dances with a happy hop, star particles, a star and a happy face, and two sounds. |
| **Sleep and Wake** | The cat lies down with a Zzz emote and closed eyes. Press the button again (it becomes **Wake**) to wake it up. |
| **3D sound** | The cat, cave and portal sounds are 3D: they get quieter when you move away and shift left or right when you turn the phone. Headphones make it easier to hear. |
| **Cave occlusion** | An invisible depth mask behind the cave wall hides the cat until it comes out through the arch. |
| **Saved feed time** | The time of the last meal is saved on the phone (`PlayerPrefs`). No account, no backend. |
| **Scan hint** | A card with a picture of the marker, a scan line and the line "Hold your phone sideways" shows until the marker is found. |

</details>

## Controls

| Action | On a phone | In the Unity Editor (webcam) |
| --- | --- | --- |
| **Feed** | Tap `Feed` (when the cat is walking or hungry) | Click the button |
| **Play** | Tap `Play` (only when the cat is walking) | Click the button |
| **Sleep / Wake** | Tap `Sleep`, tap again (`Wake`) to wake the cat | Click the button |

> [!TIP]
> Buttons that cannot be used right now are **greyed out**. For example, all three are locked while the cat is eating, playing or standing up.

## Target Card

The target database is `BoardPetAR` and the scene tracks the image **`boardpetmarker`**. The card is **A4 landscape (29.7 x 21.0 cm)** and the target width in Vuforia is `0.297 m`.

<div align="center">
<img src="Assets/Editor/Vuforia/ImageTargetTextures/BoardPetAR/boardpetmarker_scaled.jpg" alt="The Board Pet marker card" width="320">
</div>

Print it at **100% scale** and tape it **upright** on a wall or whiteboard, or show the image above on a laptop screen.

> [!IMPORTANT]
> The cave, rocks and cat sit on a ledge **just below the bottom edge of the card**, and the content is built for a card that stands **upright**. If you lay the card flat on a table, everything is tilted. Hold your phone **sideways** (landscape).

<details>
<summary>Tips for stable tracking</summary>

- Use bright, even lighting and avoid reflections on paper or screens.
- Matte paper works better than glossy paper.
- Scan **slowly** and keep the card in view until the cat is out, so you see the whole intro.
- Don't cover more than half of the card. The more of the picture is hidden, the fewer feature points can be matched.
- Fast movement blurs the camera image and breaks tracking.
- The cat stays in place for a moment when the card leaves the view. Vuforia keeps estimating its position (the *Extended Tracked* status).

</details>

## Install on Your Phone

Grab the latest build with the direct links below, or browse every version on the **[Releases](https://github.com/rifk7s/BoardPetAR/releases)** page.

| Platform | File | Requirements |
| --- | --- | --- |
| **Android** | [`BoardPetAR.apk`](https://github.com/rifk7s/BoardPetAR/releases/latest/download/BoardPetAR.apk) | Android 10 (API 29) or newer, ARM64. **Not tested on a real device.** |
| **iOS** | [`BoardPetAR-unsigned.ipa`](https://github.com/rifk7s/BoardPetAR/releases/latest/download/BoardPetAR-unsigned.ipa) | **Must be sideloaded**, Developer Mode turned on. Tested on an iPhone 13 (iOS 27.2). |

> [!WARNING]
> The **Android APK has been built but never run on a phone.** The settings follow Vuforia's requirements, but it may not work on your device. Two known risks: the cave mask (a Vuforia depth shader) is reported to render black on some Android devices, and camera autofocus may need extra setup on ARCore. **iOS is the tested platform.**

> [!NOTE]
> The deployment target is iOS 15.0, but Vuforia Engine 11.4 lists iOS 16 as its minimum, so use **iOS 16 or newer**.

<a id="android"></a>

### Android

1. Download the APK from the table above to your phone.
2. Open the file. If asked, allow **Install unknown apps** for the browser or file manager you are using.
3. Install it, open **Board Pet AR** and allow camera access.
4. Hold the phone sideways and point the camera at the marker card.

> [!NOTE]
> This APK does not come from the Play Store, so Google Play Protect may show a warning. That is normal for APKs installed manually.

<a id="ios-sideload"></a>

### iOS (Sideload)

iOS builds must be sideloaded with your own Apple ID. Open the guide below.

<details>
<summary><b>iOS sideload guide</b></summary>

The IPA in Releases is **intentionally unsigned**. Apple doesn't allow installing apps from outside the App Store directly, so you need a sideloading tool that **re-signs the IPA with your own Apple ID**. A free Apple ID is enough.

> **Warning:** With a free Apple ID, a sideloaded app is only valid for **7 days**. After that it won't open until you refresh or reinstall it. A free Apple ID is also limited to about **3 active sideloaded apps** at a time.

**Turn on Developer Mode first** (required on iOS 16 and later):

1. Open **Settings → Privacy & Security** and scroll to the bottom to find **Developer Mode**.
2. Switch it on. The phone will restart.
3. After it restarts, tap **Turn On** and enter your passcode.

> **Tip:** If the Developer Mode option isn't there, it usually shows up after the phone has run an app signed for development, for example after your first sideload.

<table width="100%">
<tr>
<td width="50%" valign="top">

#### With Sideloadly

[Sideloadly](https://sideloadly.io/) signs and installs the IPA over a USB cable. It runs on macOS and Windows.

1. Install Sideloadly on your computer.
2. Connect the iPhone with a USB cable and tap **Trust This Computer**.
3. Drag `BoardPetAR-unsigned.ipa` into the Sideloadly window.
4. Enter your **Apple ID** and click **Start**.
5. When it finishes, go to **Settings → General → VPN & Device Management**, pick your Apple ID and tap **Trust**.
6. Open the app and allow camera access.

</td>
<td width="50%" valign="top">

#### With AltStore

[AltStore](https://altstore.io/) installs and refreshes apps through **AltServer**, which runs on your computer.

1. Install **AltServer** on a Mac or Windows PC.
2. Install AltStore on the iPhone through AltServer (USB cable, or the same Wi-Fi network).
3. Open AltStore on the iPhone, go to **My Apps**, tap **+** and pick `BoardPetAR-unsigned.ipa`.
4. Trust your Apple ID profile under **Settings → General → VPN & Device Management**.
5. AltStore tries to refresh the app in the background before the 7 days run out, as long as the iPhone and the computer are on the same network.

</td>
</tr>
</table>

<details>
<summary>Why is the IPA unsigned?</summary>

A code signature contains the **name of the signing certificate**, and that name includes the email of the Apple ID that owns it. The `embedded.mobileprovision` file also holds a device ID and the Team ID. If an IPA signed with a personal Apple ID were uploaded to a public repo, anyone could read all of that.

So the IPA in Releases has its signature stripped: no `_CodeSignature`, no `embedded.mobileprovision`, and no signature on each binary. Sideloading tools re-sign it with the user's own Apple ID anyway, so the end result is the same for users.

An IPA signed by its author wouldn't work on other phones either, because its provisioning profile only allows registered devices.

</details>

<details>
<summary>Common problems</summary>

| Problem | Cause and fix |
| --- | --- |
| **Untrusted Developer** when opening the app | Go to **Settings → General → VPN & Device Management**, pick your Apple ID and tap **Trust**. |
| **Developer Mode** option is missing | Make sure you are on iOS 16 or newer. The option often only appears once a development app is installed. |
| App suddenly won't open | The 7 days are up. Refresh it in AltStore, or reinstall with Sideloadly. |
| Install fails because of the app limit | A free Apple ID is limited to about 3 active apps. Remove or deactivate another sideloaded app. |
| Black camera screen | Allow the camera under **Settings → Board Pet AR → Camera**. |
| The cat is already out and there is no cave animation | The intro plays **once per app session**. Close the app completely and open it again. |
| Nothing appears | Hold the phone sideways, use even light, and make sure the whole card is in view. |

</details>

</details>

## For Developers

<details>
<summary><b>Build from source</b></summary>

**You need:** Unity **6000.6.5f1** with the **iOS** and **Android** build support modules, **Git LFS** (install it before you clone), a free Vuforia Developer account for a license key, and Xcode for iOS.

```bash
git lfs install
git clone https://github.com/rifk7s/BoardPetAR.git
```

> [!CAUTION]
> **Never commit your license key.** It is stored in `Assets/Resources/VuforiaConfiguration.asset`, which is in `.gitignore`. After cloning, open the project in Unity and enter your own key under **Window → Vuforia Engine → Configuration**.

The Vuforia package (`Packages/com.ptc.vuforia.engine-11.4.4.tgz`) is proprietary SDK software from PTC and is stored through Git LFS in this repo. If the package is missing after cloning, run `git lfs pull`.

<table width="100%">
<tr>
<td width="50%" valign="top">

### Android

1. **File → Build Profiles**, pick **Android**, then **Switch Profile**.
2. Check the Player settings (already set in the repo):

| Setting | Value |
| --- | --- |
| Product Name | `Board Pet AR` |
| Package name | `com.rifky.boardpetar` |
| Minimum API Level | Android 10 (API 29) |
| Scripting Backend | IL2CPP |
| Target Architectures | ARM64 |
| Graphics API | OpenGLES3 only |
| Orientation | Landscape Left |

3. Keep **Build App Bundle** off, then click **Build** to get an APK.
4. In **Window → Vuforia Engine → Configuration**, ARCore is set to **Optional**.

</td>
<td width="50%" valign="top">

### iOS

1. **File → Build Profiles**, pick **iOS**, then **Switch Profile** and **Build** into `Builds/iOS`.
2. Open `Unity-iPhone.xcodeproj` in Xcode.
3. Under **Signing & Capabilities**, pick a **Team** and keep the Bundle Identifier `com.rifky.boardpetar` (or change it to your own).
4. Pick your iPhone as the destination, then **Run**.
5. Deployment target is iOS 15.0. The camera usage text is already set: "Camera is used for AR Tracking".
6. Orientation is **Landscape Left**.

</td>
</tr>
</table>

<details>
<summary>Make an unsigned IPA from the Xcode build</summary>

Once the app builds, take `BoardPetAR.app` from `DerivedData` (the `Build/Products/ReleaseForRunning-iphoneos/` folder) and run the following from the folder that contains it.

```bash
# 1. Create the Payload folder and copy the app into it
mkdir -p ipa/Payload
cp -R BoardPetAR.app ipa/Payload/

# 2. Remove the provisioning profile and every _CodeSignature folder
rm -f ipa/Payload/BoardPetAR.app/embedded.mobileprovision
find ipa/Payload -name _CodeSignature -type d -prune -exec rm -rf {} +

# 3. Strip the signature from every Mach-O binary
find ipa/Payload -type f -exec sh -c 'file "$1" | grep -q Mach-O && codesign --remove-signature "$1"' _ {} \;

# 4. Zip it into an IPA
(cd ipa && zip -qry ../BoardPetAR-unsigned.ipa Payload)
```

Then check that none of your account data is left. Replace `your-apple-id-email` with your own Apple ID email. Every command below should print nothing, except the last one, which should only print `not signed` lines:

```bash
unzip -q BoardPetAR-unsigned.ipa -d check

# No signing leftovers
find check -name embedded.mobileprovision -o -name _CodeSignature

# No Apple ID email, certificate names or entitlements
grep -rl -a "your-apple-id-email" check
grep -rl -a -E "Apple Development|Apple Distribution|iPhone Developer" check
grep -rl -a -E "TeamIdentifier|application-identifier|get-task-allow" check

# Every Mach-O binary should say "not signed"
find check -type f -exec sh -c 'file "$1" | grep -q Mach-O && codesign -d "$1" 2>&1' _ {} \;
```

Delete the `check` folder when you are done. The `Builds/` folder is in `.gitignore`, so `.apk` and `.ipa` files are never committed.

</details>

<details>
<summary>Test in the Unity Editor</summary>

1. Open the project and enter your Vuforia license key (see above).
2. Open `Assets/Scenes/SampleScene.unity` and press **Play**. Vuforia uses your webcam.
3. Hold the marker card (or the image on a screen) **upright** in front of the webcam.
4. To test hunger quickly, select `Pet` during Play and set **Hunger Duration Seconds** on **Pet Hunger** to a small value. It resets when Play stops.
5. To start over with no saved feed time, use **Edit → Clear All PlayerPrefs**.

If the Console keeps printing a Vuforia `MissingReferenceException` about a destroyed material, restart Unity. It happens after many Play sessions in a row.

</details>

</details>

<details>
<summary><b>How it works</b></summary>

**Vuforia stages:**

1. **Offline.** The marker image is analyzed into feature points and saved as a database (`Assets/StreamingAssets/Vuforia/BoardPetAR.dat`).
2. **Detection, every frame.** The app finds feature points in the camera image and matches them against the database.
3. **Pose estimation.** Vuforia computes the card's position and rotation (6DoF) relative to the camera.
4. **Tracking.** On phones with ARKit or ARCore, device tracking keeps the content in place briefly after the card leaves the view (the *Extended Tracked* status). The intro ignores that status and only plays while the card is really in view (*Tracked*).

**Scene hierarchy:**

```
SampleScene
├── ARCamera
├── Directional Light              (shadows off: the thin cave wall flickered in AR)
├── ImageTarget                    (Vuforia, Default Observer Event Handler)
│   └── LedgeAnchor                (rotated 90 degrees on X so its up is real-world up)
│       ├── Cave, RockA, RockB, StoneA
│       ├── CaveMask               (invisible depth mask behind the cave wall)
│       ├── CavePortal             (glow, rim, swirl and sparkles)
│       ├── CaveIntro              (plays the intro)
│       ├── SmokePuff, StarParticles
│       ├── Food                   (the salmon slice)
│       └── Pet                    (the cat: animator, wander, state machine, hunger, actions, eyes, emote)
├── PetUI                          (screen space overlay canvas)
│   ├── ScanHint                   (marker picture, scan line, "Hold your phone sideways")
│   └── ButtonBar                  (Feed, Play, Sleep)
└── EventSystem                    (Input System UI module)
```

**State machine.** `PetStateMachine` is the only owner of the pet state. Other scripts only call `RequestState(...)`, and a transition that is not in the table is refused.

| From | To |
| --- | --- |
| `Intro` | `Wandering` |
| `Wandering` | `Hungry`, `Eating`, `Playing`, `Sleeping` |
| `Hungry` | `Eating` |
| `Eating`, `Playing`, `WakingUp` | `Wandering` |
| `Sleeping` | `WakingUp` |

Feed works in `Wandering` and `Hungry`, Play only in `Wandering`, Sleep in `Wandering` and Wake in `Sleeping`. Hunger keeps counting while the cat sleeps. If it gets hungry in its sleep, it stays asleep and becomes hungry right after waking up.

**Cave mask.** `CaveMask` is a quad behind the cave that uses Vuforia's `DepthMask` material. It writes depth but no color, so it is invisible and hides anything behind it. The cat starts behind it and walks forward through the arch. This is **static occlusion** only: a hand in front of the cat does not hide it. Hand occlusion needs ARKit human segmentation through AR Foundation, which Vuforia 11.4.4 cannot run with.

**Performance.** Frame rate is set to **30 fps** at startup. Measured on an iPhone 13 with the Unity Profiler (Development Build) and the Metal Performance HUD (Release build):

| Measure | Result |
| --- | --- |
| Frame time | 33.3 ms (30 fps) in every scenario: scan hint, intro, walking, Feed, Play and Sleep |
| Triangles | about 880 while walking, 905 to 920 at most during the intro and Play |
| SetPass calls | 8 to 15 |
| Frame rate in a Release build (Metal HUD log) | 29.99 fps, after a short hitch of about one second when the intro starts (loading and creating the cave, portal and cat) |

The cause of that hitch is not verified.

</details>

## Known Limitations

- **Android is untested.** The APK builds, but nobody has run it on a phone yet.
- **No hand occlusion.** Only the cave hides the cat (see above).
- **One pet, one marker, one scene.** The demo build gets hungry after 20 seconds.
- Free Apple ID sideloads expire after 7 days.

## Credits

Everything 3D, 2D and audio comes from **[Kenney](https://kenney.nl/)** under **[CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/)** (public domain). Attribution is not required, but it is appreciated. The one exception is the font.

| Pack | Used for |
| --- | --- |
| **[Cube Pets](https://kenney.nl/assets/cube-pets)** | The cat and its animations |
| **[Nature Kit](https://kenney.nl/assets/nature-kit)** | The cave and the small rocks |
| **[Food Kit](https://kenney.nl/assets/food-kit)** | The salmon slice (rice removed) and the hungry icon |
| **[Particle Pack](https://kenney.nl/assets/particle-pack)** | Stars, portal glow, rim, swirl and sparkles, smoke puff |
| **[Emotes Pack](https://kenney.nl/assets/emotes-pack)** | Heart, Zzz, star, happy face and alert bubbles |
| **[UI Pack](https://kenney.nl/assets/ui-pack)** | The Feed, Play and Sleep buttons and the hint card |
| **[Interface Sounds](https://kenney.nl/assets/interface-sounds)** | Button taps and the Play sounds |
| **[Impact Sounds](https://kenney.nl/assets/impact-sounds)** | The cave appearing and the cat hopping out |
| **[Sci-Fi Sounds](https://kenney.nl/assets/sci-fi-sounds)** | The portal opening and closing |

Some files are edited copies (the salmon slice without rice, the cat body split from its eyes, the hungry icon). They live in `Assets/_BoardPet`, and the originals stay untouched in `Assets/ThirdParty`.

The button font is **[Poppins](https://fonts.google.com/specimen/Poppins) SemiBold** by The Poppins Project Authors, under the [SIL Open Font License 1.1](https://openfontlicense.org/). The marker card was made in Canva. Tracking is by [Vuforia Engine](https://developer.vuforia.com/) (PTC).

<details>
<summary>References</summary>

- [Vuforia Developer Library: Getting Started with Vuforia Engine in Unity](https://developer.vuforia.com/library/getting-started/getting-started-vuforia-engine-unity)
- [Vuforia Engine supported versions](https://developer.vuforia.com/library/vuforia-engine/platform-support/supported-versions/)
- [Using ARCore with Vuforia Engine in Unity](https://developer.vuforia.com/library/vuforia-engine/platform-support/platform-frameworks/using-arcore-vuforia-engine/)
- [Apple: Monitoring your Metal app's graphics performance](https://developer.apple.com/documentation/xcode/monitoring-your-metal-apps-graphics-performance)
- [Unity: Rendering Profiler module reference](https://docs.unity.com/en-us/engine/6000.6/manual/analysis/graphics-performance-profiling/profile-rendering/profiler-rendering)
- [AltStore FAQ](https://faq.altstore.io/)
- [Sideloadly](https://sideloadly.io/)
- [Expo: iOS Developer Mode](https://docs.expo.dev/guides/ios-developer-mode/)
- [Git LFS](https://git-lfs.com/)

</details>

<!-- Badge images (kept here so the top of the file stays readable) -->

[badge-unity]: https://img.shields.io/badge/Unity-6000.6-black?style=flat&labelColor=1a1a1a&logo=unity
[badge-vuforia]: https://img.shields.io/badge/Vuforia-11.4.4-blue?style=flat&labelColor=1a1a1a
[badge-android]: https://img.shields.io/badge/Android-10%2B%20untested-3DDC84?style=flat&labelColor=1a1a1a&logo=android&logoColor=3DDC84
[badge-ios]: https://img.shields.io/badge/iOS-15%2B-lightgrey?style=flat&labelColor=1a1a1a&logo=apple&logoColor=white
[badge-lfs]: https://img.shields.io/badge/Git%20LFS-required-orange?style=flat&labelColor=1a1a1a&logo=git&logoColor=F05032

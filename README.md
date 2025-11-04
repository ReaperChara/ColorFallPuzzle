# ColorFallPuzzle

A simple falling block puzzle game for Android, built with .NET MAUI in C#. Features pentomino shapes with colored blocks; match 3 same colors to burst and score. Includes AdMob test ads.

## Setup
- .NET 8+ required for build.
- Use GitHub Actions (build-android.yml) to build APK automatically on push.

## AdMob Test IDs
- Android App ID: `ca-app-pub-3940256099942544~3347511713`
- Banner Ad Unit ID: `ca-app-pub-3940256099942544/6300978111`
- Interstitial Ad Unit ID: `ca-app-pub-3940256099942544/1033173712`
- Rewarded Ad Unit ID: `ca-app-pub-3940256099942544/5224354917`

## How to Build
- `dotnet build` compiles the project but does not create a distributable APK.
- The project is now targeted at `.NET 9` for Android.
- To build a release APK locally, run:
  - `dotnet publish -f net9.0-android35.0 -c Release /p:AndroidPackageFormat=apk`

## Restore and recovery
Use the provided helper script from the repo root to install JDK 21, Android SDK command-line tools, and build the APK:

```bash
chmod +x ./build-android.sh
./build-android.sh
```

If you want to run the commands manually, use this block instead:

```bash
export ANDROID_SDK_ROOT="$HOME/Android/Sdk"
export ANDROID_HOME="$ANDROID_SDK_ROOT"
export JAVA_HOME="$HOME/jdk-21"
export PATH="$JAVA_HOME/bin:$PATH:$ANDROID_SDK_ROOT/cmdline-tools/latest/bin:$ANDROID_SDK_ROOT/platform-tools"

mkdir -p "$ANDROID_SDK_ROOT"

if [ ! -x "$JAVA_HOME/bin/java" ]; then
  curl -L -o /tmp/jdk21.tar.gz https://github.com/adoptium/temurin21-binaries/releases/latest/download/OpenJDK21U-jdk_x64_linux_hotspot.tar.gz
  mkdir -p "$JAVA_HOME"
  tar -xzf /tmp/jdk21.tar.gz -C "$JAVA_HOME" --strip-components=1
  rm -f /tmp/jdk21.tar.gz
fi

if [ ! -x "$ANDROID_SDK_ROOT/cmdline-tools/latest/bin/sdkmanager" ]; then
  curl -L -o /tmp/commandlinetools.zip https://dl.google.com/android/repository/commandlinetools-linux-9477386_latest.zip
  rm -rf /tmp/cmdline-tools
  mkdir -p /tmp/cmdline-tools
  unzip -q /tmp/commandlinetools.zip -d /tmp/cmdline-tools
  mkdir -p "$ANDROID_SDK_ROOT/cmdline-tools/latest"
  mv /tmp/cmdline-tools/cmdline-tools/* "$ANDROID_SDK_ROOT/cmdline-tools/latest/"
  rm -rf /tmp/cmdline-tools /tmp/commandlinetools.zip
fi

yes | sdkmanager --sdk_root="$ANDROID_SDK_ROOT" \
  "platform-tools" \
  "platforms;android-35" \
  "build-tools;35.0.0" \
  "cmdline-tools;latest"

rm -rf obj bin

dotnet workload install maui-android
dotnet workload restore
dotnet restore

dotnet build -f net9.0-android35.0 -c Release

dotnet publish -f net9.0-android35.0 -c Release /p:AndroidPackageFormat=apk
```

If the build succeeds, the APK will be created under `bin/Release/net9.0-android35.0/android-arm64/publish` or similar.

1. Push changes.
2. Go to Actions tab, download APK from artifacts.

## License
Apache 2.0

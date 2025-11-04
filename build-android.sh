#!/usr/bin/env bash
set -euo pipefail

export ANDROID_SDK_ROOT="${ANDROID_SDK_ROOT:-$HOME/Android/Sdk}"
export ANDROID_HOME="${ANDROID_HOME:-$ANDROID_SDK_ROOT}"
export JAVA_HOME="$HOME/jdk-21"
export PATH="$JAVA_HOME/bin:$PATH:$ANDROID_SDK_ROOT/cmdline-tools/latest/bin:$ANDROID_SDK_ROOT/platform-tools"

mkdir -p "$ANDROID_SDK_ROOT"

# Remove old JDK if exists and reinstall fresh
if [ -d "$JAVA_HOME" ]; then
  echo "Removing old JDK..."
  rm -rf "$JAVA_HOME"
fi

echo "Installing JDK 21 to $JAVA_HOME..."
mkdir -p "$JAVA_HOME"
curl -L -o /tmp/jdk21.tar.gz https://github.com/adoptium/temurin21-binaries/releases/latest/download/OpenJDK21U-jdk_x64_linux_hotspot.tar.gz
tar -xzf /tmp/jdk21.tar.gz -C "$JAVA_HOME" --strip-components=1
rm -f /tmp/jdk21.tar.gz

if [ ! -x "$JAVA_HOME/bin/java" ]; then
  echo "ERROR: JDK 21 installation failed."
  exit 1
fi

echo "JDK version: $($JAVA_HOME/bin/java -version 2>&1 | head -1)"

if [ ! -x "$ANDROID_SDK_ROOT/cmdline-tools/latest/bin/sdkmanager" ]; then
  echo "Installing Android command-line tools..."
  curl -L -o /tmp/commandlinetools.zip https://dl.google.com/android/repository/commandlinetools-linux-9477386_latest.zip
  rm -rf /tmp/cmdline-tools
  mkdir -p /tmp/cmdline-tools
  unzip -q /tmp/commandlinetools.zip -d /tmp/cmdline-tools
  mkdir -p "$ANDROID_SDK_ROOT/cmdline-tools/latest"
  mv /tmp/cmdline-tools/cmdline-tools/* "$ANDROID_SDK_ROOT/cmdline-tools/latest/"
  rm -rf /tmp/cmdline-tools /tmp/commandlinetools.zip
fi

if [ ! -x "$ANDROID_SDK_ROOT/cmdline-tools/latest/bin/sdkmanager" ]; then
  echo "ERROR: sdkmanager not found after installation."
  exit 1
fi

echo "Installing Android SDK packages..."

yes | sdkmanager --sdk_root="$ANDROID_SDK_ROOT" \
  "platform-tools" \
  "platforms;android-35" \
  "build-tools;35.0.0" \
  "cmdline-tools;latest"

rm -rf obj bin

dotnet workload install maui-android
dotnet workload restore
 dotnet restore -r android-arm64
 dotnet build -f net9.0-android35.0 -c Release -r android-arm64
 dotnet publish -f net9.0-android35.0 -c Release -r android-arm64 /p:AndroidPackageFormat=apk

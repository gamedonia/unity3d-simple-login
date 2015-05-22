///////////////////////////
Gamedonia Unity SDK Readme
///////////////////////////

Release 1.13.0 Changelog:
-------------------------

- Added support for push notifications from background in Unity 5 for Android.

  IMPORTANT: The AndroidManifest.xml has been modified and will be replaced with a new one. If you don't want to lose your changes, deselect the AndroidManifest.xml when importing this version of the SDK and replace the activity called "com.gamedonia.sdk.GamedoniaUnityPlayerProxyActivity" with this one:

    <activity android:name="com.gamedonia.sdk.GamedoniaUnityPlayerActivity"
              android:label="@string/app_name"
              android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen">
        <intent-filter>
            <action android:name="android.intent.action.MAIN" />
            <category android:name="android.intent.category.LAUNCHER" />
        </intent-filter>
    </activity>

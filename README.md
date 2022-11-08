ActivityResultApp

This app provides an example of using the ActivityResult API’s first introduced in AndroidX.Activity and Fragment.

Traditionally, we used StartActivityForResult() and OnActivityResult() APIs to start another activity and receive a result back that is available on the Activity classes on all API levels. These are now deprecated in favour of the ActivityResult APIs.

The important pieces of these new APIs

RegisterForActivityResult – for registering the result callback

ActivityResultContract – a contract specifying that an activity can be called with an input of type a and produce an output of b, which makes calling an activity for a result type safe. 

ActivityResultCallback – the callback to be called when an activity result is available. It is a single method interface with an OnActivityResult method that takes an object of the output type defined in the ActivityResultContract.

ActivityResultLauncher — A launcher for a previously prepared call to start the process of executing an ActivityResultContract.

You can declare RegisterForActivityResult in the OnCreate of any activity or fragment, however, you can only call ActivityResultLauncher when the activity’s Lifecycle has reached Created.

ActivityResultApp

This app example:

Android 12 introduced two new permissions for Bluetooth i.e. Manifest.Permission.Bluetooth.Connect and Manifest.Permission.Bluetooth.Scan. 

My apps connect to Bluetooth scan tools, therefore for Android 12 and above I need to request permission from the user for those two permissions. If the permissions are granted, I then need to ensure that Bluetooth is turned on by prompting the user to allow the app to turn on Bluetooth if it is not already on. 

In the OnCreate of the MainActivity, we have the following a new activityResultCallback and a handler for the callback. We then use the previously declared ActivityResultLaucher to RegisterForActivityResult by passing it the contract and the activityResultCallback.

ActivityResultCallback activityResultCallback = new ActivityResultCallback();

activityResultCallback.OnActivityResultCalled += ActivityResultCallback_ActivityResultCalled;

activityResultLauncher = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), activityResultCallback);

Therefore, the app after getting both Bluetooth permissions then calls
activityResultLauncher.Launch(new Intent(BluetoothAdapter.ActionRequestEnable));

The ActivityResultCallback_ActivityResultCalled handler then processes the result by displaying a dialog that it has been successful or not.

To test the app you need at least an Android 12 device and before launching the app you need to ensure that Bluetooth is toggled off.

Please note that this is just a test app, in the real app on the first start-up no scan tool could have been selected, therefore it would be impossible to connect to a scan tool even if the user hits the connect button on the displayed fragment. Therefore, the first interaction with Bluetooth will be when the user opens the SettingsFragment to select a particular scan tool. Selecting a scan tool from a list will also trigger the requirement of permissions, (Bluetooth.Scan) which could result in a bit of a shock for the user being asked for Bluetooth permissions when previous versions of our app didn’t. 

We attempt to overcome this on app start-up by explaining to the user via a BottomSheetDialogFragment that Android 12 and above have these new Bluetooth permission requirements before they open the SettingsFragment and then only launch the SettingsFragment after obtaining the permissions. Android 13 also requires permission to send Notifications, so again you need to find a suitable point in the app where it is more likely that the user will be keen to accept your request. With the Bluetooth permission required so early in our app, there is no ideal point that you can ask for the permissions, hence the need to present an explanation via the BottomSheetDialog.

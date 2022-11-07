using Android.App;
using Android.OS;
using AndroidX.Activity.Result.Contract;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using Android.Bluetooth;
using Android.Content;
using Android.Widget;
using Android.Runtime;
using Android.Content.PM;
using AndroidX.Fragment.App;
using com.companyname.ActivityResultApp.Dialogs;
using AndroidX.Core.Content;
using Android;

namespace com.companyname.ActivityResultApp
{
    [Activity(Label = "@string/app_name", /*Theme = "@style/AppTheme",*/ MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ActivityResultLauncher activityResultLauncher;
        private BluetoothAdapter bluetoothAdapter;

        private const int BLUETOOTH_PERMISSIONS_REQUEST_CODE = 9999;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            ActivityResultCallback activityResultCallback = new ActivityResultCallback();
            activityResultCallback.OnActivityResultCalled += ActivityResultCallback_ActivityResultCalled;

            activityResultLauncher = RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), activityResultCallback);

            BluetoothManager manager = Application.Context.GetSystemService(Application.BluetoothService) as BluetoothManager;
            bluetoothAdapter = manager.Adapter;
            if (bluetoothAdapter == null)
                Toast.MakeText(this, "This device doesn't support Bluetooth.", ToastLength.Long).Show();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S) // Bluetooth permissions are required for Android 12+
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothConnect) == Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothScan) == Permission.Granted)
                    EnsureBluetoothEnabled();
                else
                    RequestPermissions(new string[] { Manifest.Permission.BluetoothConnect, Manifest.Permission.BluetoothScan }, BLUETOOTH_PERMISSIONS_REQUEST_CODE);
            }
            else
                EnsureBluetoothEnabled();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == BLUETOOTH_PERMISSIONS_REQUEST_CODE)
            {
                // Two Bluetooth permissions - Connect and Scan - see AndroidManifest
                if (grantResults.Length == 2 && grantResults[0] == Permission.Granted && grantResults[1] == Permission.Granted)
                {
                    // Permissions are granted, so make sure Bluetooth is on 
                    EnsureBluetoothEnabled();
                    //navController.Navigate(Resource.Id.settingsFragment);
                }
                else
                    ShowMissingPermissionDialog(this, GetString(Resource.String.missing_permission_title), GetString(Resource.String.missing_permission_explanation));

                return;
            }
        }

        private void ActivityResultCallback_ActivityResultCalled(object sender, ActivityResult result)
        {
            if (result.ResultCode == (int)Result.Ok)
            {
                if (bluetoothAdapter.State == State.On)
                    ShowBluetoothConfirmationDialog(true);
            }
            else if (result.ResultCode == (int)Result.Canceled)
                ShowBluetoothConfirmationDialog(false);
        }

        private void EnsureBluetoothEnabled()
        {
            if ((bluetoothAdapter != null) && (!bluetoothAdapter.IsEnabled))
                activityResultLauncher.Launch(new Intent(BluetoothAdapter.ActionRequestEnable));
        }

        #region ShowBluetoothConfirmationDialog
        internal void ShowBluetoothConfirmationDialog(bool isEnabled)
        {
            // 22/06/2020 This could be a BasicDialogFragment, nothing special here. But its passed a boolean value - therefore leave for the moment
            string message = isEnabled ? GetString(Resource.String.bluetooth_is_enabled) : GetString(Resource.String.bluetooth_not_enabled);
            string tag = "BluetoothConfirmationDialogFragment";
            AndroidX.Fragment.App.FragmentManager fm = SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                AndroidX.Fragment.App.Fragment fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    BluetoothConfirmationDialogFragment.NewInstance(message).Show(fm, tag);
            }
        }
        #endregion

        #region ShowMissingPermissionDialog
        public void ShowMissingPermissionDialog(FragmentActivity activity, string title, string explanation)
        {
            string tag = "ShowMissingPermissionDialog";
            AndroidX.Fragment.App.FragmentManager fm = activity.SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                AndroidX.Fragment.App.Fragment fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    BasicDialogFragment.NewInstance(title, explanation).Show(fm, tag);
            }
        }
        #endregion

        #region DismissBluetoothDialogConfirmation
        internal void DismissBluetoothConfirmationDialog()
        {
            BluetoothConfirmationDialogFragment bluetoothConfirmationDialogFragment = (BluetoothConfirmationDialogFragment)SupportFragmentManager.FindFragmentByTag("BluetoothConfirmationDialogFragment");
            if (bluetoothConfirmationDialogFragment != null)
            {
                if (bluetoothConfirmationDialogFragment.Dialog.IsShowing)
                    bluetoothConfirmationDialogFragment.Dialog.Dismiss();
            }
        }
        #endregion
    }
}
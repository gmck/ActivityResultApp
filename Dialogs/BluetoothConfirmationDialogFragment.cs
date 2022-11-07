using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Dialog;

namespace com.companyname.ActivityResultApp.Dialogs
{
    internal class BluetoothConfirmationDialogFragment : AppCompatDialogFragment
    {
        public BluetoothConfirmationDialogFragment() { }  // Required Parameter less ctor

        internal static BluetoothConfirmationDialogFragment NewInstance(string message)
        {
            Bundle arguments = new Bundle();
            arguments.PutString("Message", message);

            BluetoothConfirmationDialogFragment fragment = new BluetoothConfirmationDialogFragment
            {
                Cancelable = false,
                Arguments = arguments
            };
            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            LayoutInflater inflater = LayoutInflater.From(Activity);
            View view = inflater.Inflate(Resource.Layout.generic_dialog, null);

            TextView textViewExplanation = view.FindViewById<TextView>(Resource.Id.textView_explanation);
            textViewExplanation.Text = Arguments.GetString("Message");


            MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(Activity);
            builder.SetTitle(GetString(Resource.String.bluetooth_title));
            builder.SetView(view);
            builder.SetPositiveButton(Android.Resource.String.Ok, (sender, e) => { ((MainActivity)Activity).DismissBluetoothConfirmationDialog(); });
            return builder.Create();
        }
    }
}
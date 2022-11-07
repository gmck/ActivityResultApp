using AndroidX.Activity.Result;
using System;

namespace com.companyname.ActivityResultApp
{
    public class ActivityResultCallback : Java.Lang.Object, IActivityResultCallback
    {
        public EventHandler<ActivityResult> OnActivityResultCalled;

        public void OnActivityResult(Java.Lang.Object result)
        {
            ActivityResult activityResult = result as ActivityResult;
            OnActivityResultCalled?.Invoke(this, activityResult);
        }
    }
}
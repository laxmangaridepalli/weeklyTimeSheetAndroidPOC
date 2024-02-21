using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.OS;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.AppUpdate.Install.Model;

namespace weeklyTimeSheetAndroidPOC;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const int AppUpdateRequestCode = 1001;
    private readonly Activity _activity;

    public MainActivity() { }

    public MainActivity(Activity activity)
    {
        _activity = activity;
    }
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CheckForUpdates();
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode == AppUpdateRequestCode)
        {
            if (resultCode != Result.Ok)
            {
                // Handle update failure
            }
        }
    }

    private async void CheckForUpdates()
    {
        string[] VersionArray = { "23.09.0", "23.12.0", "24.03.0" };

        int versionCount = 0;
        string versionCodeNumber;

        var packageName = "com.ineight.hdweeklytimesheet"; // Replace with your app's package name
        var httpClient = new HttpClient();

        // Get the latest version code from the Play Store
        var response = await httpClient.GetAsync($"https://play.google.com/store/apps/details?id=com.ineight.weeklytimesheet&hl=en_US&gl=US");
        var html = await response.Content.ReadAsStringAsync();
        foreach (string i in VersionArray)
        {
            if (html.Contains(i))
            {
                versionCount++;
                if(versionCount >= 1)
                {
                    versionCodeNumber = i;
                }
                else
                {

                }
            }
        }
        //var versionCodeIndex = html.IndexOf("Current Version");
        //var versionCodeStartIndex = html.IndexOf(">", versionCodeIndex) + 1;
        //var versionCodeEndIndex = html.IndexOf("<", versionCodeStartIndex);
        //var versionCode = html.Substring(versionCodeStartIndex, versionCodeEndIndex - versionCodeStartIndex);

        // Download the APK
        var apkUrl = $"https://play.google.com/store/apps/details?id=com.ineight.weeklytimesheet&hl=en_US&gl=US";
        var apkResponse = await httpClient.GetAsync(apkUrl);
        var apkStream = await apkResponse.Content.ReadAsStreamAsync();
        //var apkPath = $"{packageName}.apk";
        //using (var fileStream = System.IO.File.Create(apkPath))
        //{
        //    await apkStream.CopyToAsync(fileStream);
        //}
       
        var appUpdateManager = AppUpdateManagerFactory.Create(this);
        var appUpdateInfoTask = appUpdateManager.GetAppUpdateInfo();
        appUpdateInfoTask.AddOnSuccessListener(new AppUpdateInfoSuccessListener(appUpdateManager, _activity));
        appUpdateInfoTask.AddOnFailureListener(new AppUpdateInfoFailureListener());
    }
}

public class AppUpdateInfoSuccessListener : Java.Lang.Object, IOnSuccessListener
{
    private readonly IAppUpdateManager _appUpdateManager;
    private readonly Activity _activity;

    public AppUpdateInfoSuccessListener(IAppUpdateManager appUpdateManager, Activity activity)
    {
        _appUpdateManager = appUpdateManager;
        _activity = activity;
    }

    [Obsolete]
    public void OnSuccess(Java.Lang.Object result)
    {
        var appUpdateInfo = (AppUpdateInfo)result;
        if (appUpdateInfo.UpdateAvailability() == UpdateAvailability.UpdateAvailable && appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Immediate))
        {
            // Request the update
            var appUpdateRequest = _appUpdateManager?.StartUpdateFlowForResult(appUpdateInfo, AppUpdateType.Immediate, _activity, 1001);
        }
    }
}

public class AppUpdateInfoFailureListener : Java.Lang.Object, IOnFailureListener
{
    public void OnFailure(Java.Lang.Exception e) { }
}

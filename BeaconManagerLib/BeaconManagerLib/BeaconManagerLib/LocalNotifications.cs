using System;

#if __IOS__
using UIKit;
#endif

#if __ANDROID__
using Android.App;
using Android.Content;
#endif

namespace BeaconManagerApp
{
	public static class LocalNotification
	{
		public static void SendNotification(string title, string message) 
		{
			#if __ANDROID__
			// Instantia
			Notification.Builder builder = new Notification.Builder (Application.Context)
				.SetContentTitle (title)
				.SetContentText (message)
				.SetSmallIcon(Android.Resource.Drawable.IcDialogMap);

			// Build the notification
			Notification notification = builder.Build ();

			// Get the notification manager
			NotificationManager notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

			// Publish the notifications
			const int notificationId = 0;
			notificationManager.Notify (notificationId, notification);

			#endif

			#if __IOS__
			// create the notification
			UILocalNotification notification = new UILocalNotification();

			// configure the alert
			notification.AlertAction = title;
			notification.AlertBody = message;

			// modify the badge
			notification.ApplicationIconBadgeNumber = UIApplication.SharedApplication.ApplicationIconBadgeNumber + 1;

			// set the sound to be the default sound
			notification.SoundName = UILocalNotification.DefaultSoundName;

			// schedule it
			UIApplication.SharedApplication.PresentLocalNotificationNow(notification);
			#endif
		}
	}
}


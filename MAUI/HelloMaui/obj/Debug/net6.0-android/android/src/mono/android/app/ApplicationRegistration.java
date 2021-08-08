package mono.android.app;

public class ApplicationRegistration {

	public static void registerApplications ()
	{
				// Application and Instrumentation ACWs must be registered first.
		mono.android.Runtime.register ("MAUI.MainApplication, MAUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc64ec48d0839f84f159.MainApplication.class, crc64ec48d0839f84f159.MainApplication.__md_methods);
		mono.android.Runtime.register ("Microsoft.Maui.MauiApplication`1, Microsoft.Maui, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc6488302ad6e9e4df1a.MauiApplication_1.class, crc6488302ad6e9e4df1a.MauiApplication_1.__md_methods);
		mono.android.Runtime.register ("Microsoft.Maui.MauiApplication, Microsoft.Maui, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc6488302ad6e9e4df1a.MauiApplication.class, crc6488302ad6e9e4df1a.MauiApplication.__md_methods);
		
	}
}

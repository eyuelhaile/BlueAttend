package md5ca9eb323144fe5f8c50fa2a79e1b0117;


public class NotifyBeacon
	extends android.bluetooth.le.AdvertiseCallback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onStartFailure:(I)V:GetOnStartFailure_IHandler\n" +
			"n_onStartSuccess:(Landroid/bluetooth/le/AdvertiseSettings;)V:GetOnStartSuccess_Landroid_bluetooth_le_AdvertiseSettings_Handler\n" +
			"";
		mono.android.Runtime.register ("BlueAttend.NotifyBeacon, BlueAttend", NotifyBeacon.class, __md_methods);
	}


	public NotifyBeacon ()
	{
		super ();
		if (getClass () == NotifyBeacon.class)
			mono.android.TypeManager.Activate ("BlueAttend.NotifyBeacon, BlueAttend", "", this, new java.lang.Object[] {  });
	}

	public NotifyBeacon (org.altbeacon.beacon.BeaconTransmitter p0)
	{
		super ();
		if (getClass () == NotifyBeacon.class)
			mono.android.TypeManager.Activate ("BlueAttend.NotifyBeacon, BlueAttend", "AltBeaconOrg.BoundBeacon.BeaconTransmitter, AndroidAltBeaconLibrary", this, new java.lang.Object[] { p0 });
	}


	public void onStartFailure (int p0)
	{
		n_onStartFailure (p0);
	}

	private native void n_onStartFailure (int p0);


	public void onStartSuccess (android.bluetooth.le.AdvertiseSettings p0)
	{
		n_onStartSuccess (p0);
	}

	private native void n_onStartSuccess (android.bluetooth.le.AdvertiseSettings p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

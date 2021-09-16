package com.crossp.client;

import android.content.Context;
import android.content.Intent;
import android.media.MediaPlayer;
import android.net.Uri;
import android.util.Log;
import android.view.View;
import android.widget.Toast;
import android.widget.VideoView;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;

public class Crossp {

    public static final String CROSSP_LOG_TAG = "crossp";
    private static final String SERVER_URL = "https://crossp.azurewebsites.net/";
    private static final String API_VERSION = "1.0";
    private static final String EXTERNAL_ID = "9b0731aa-5966-453f-9d26-042c6c192400";

    private static Crossp instance = null;
    public static Crossp getInstance() {
        if (instance == null)
            instance = new Crossp();
        return instance;
    }

    private Context context;
    private ArrayList<AvailableAd> availableAds;
    private AvailableAd activeAd;

    private boolean initialized;
    private boolean initializing;
    public boolean getInitialized() { return initialized; }

    private Crossp() {
        context = null;
        availableAds = null;
        activeAd = null;
        initialized = false;
        initializing = false;
    }

    public void initialize(Context context, OnInitializationCompletedListener onInitializationCompletedListener, OnInitializationFailedListener onInitializationFailedListener) {
        if (initialized || initializing)
            return;
        this.context = context;
        initializing = true;

        loadAvailableAds(new OnAdsLoadedListener() {
            @Override
            public void onAdsLoaded(ArrayList<AvailableAd> loadedAvailableAds) {
                initialized = true;
                initializing = false;
                availableAds = loadedAvailableAds;
                onInitializationCompletedListener.onInitializationCompleted();
            }
        }, new OnAdsFailedToLoadListener() {
            @Override
            public void OnAdsFailedToLoad(String errorMessage) {
                initialized = false;
                initializing = false;
                onInitializationFailedListener.onInitializationFailed(errorMessage);
            }
        });
    }

    private void loadAvailableAds(OnAdsLoadedListener onAdsLoadedListener, OnAdsFailedToLoadListener onAdsFailedToLoadListener) {
        new Thread(new Runnable() {
            public void run() {
                String url = SERVER_URL + "Clients/availableads?api-version=" + API_VERSION + "&externalId=" + EXTERNAL_ID;
                LoadAdsThread loadAdsThread = new LoadAdsThread( url );
                loadAdsThread.start();
                try{
                    loadAdsThread.join();
                }
                catch(InterruptedException e){
                    String errorMessage = e.getMessage();
                    Log.e(CROSSP_LOG_TAG, errorMessage);
                    onAdsFailedToLoadListener.OnAdsFailedToLoad(errorMessage);
                    return;
                }

                Ad[] ads = loadAdsThread.getAds();

                if (ads == null || ads.length == 0)
                {
                    String errorMessage = loadAdsThread.getErrorMessage();
                    Log.e(CROSSP_LOG_TAG, errorMessage);
                    onAdsFailedToLoadListener.OnAdsFailedToLoad(errorMessage);
                    return;
                }

                LoadFileThread[] loadFileThreads = new LoadFileThread[ads.length];
                for (int i = 0; i < loadFileThreads.length; i++)
                {
                    String requestUrl = SERVER_URL + "Clients/adfile?api-version=" + API_VERSION + "&adFileId=" + ads[i].getFile().getId();
                    loadFileThreads[i] = new LoadFileThread(requestUrl);
                    loadFileThreads[i].start();
                }
                for (int i = 0; i < loadFileThreads.length; i++)
                {
                    try{
                        loadFileThreads[i].join();
                    }
                    catch(InterruptedException e){
                        String errorMessage = e.getMessage();
                        Log.e(CROSSP_LOG_TAG, errorMessage);
                        onAdsFailedToLoadListener.OnAdsFailedToLoad(errorMessage);
                        return;
                    }
                }

                ArrayList<AvailableAd> availableAds = new ArrayList<AvailableAd>();
                for(int i = 0; i < ads.length; i++)
                {
                    byte[] fileData = loadFileThreads[i].getData();

                    if (fileData == null || fileData.length == 0)
                    {
                        String errorMessage = loadFileThreads[i].getErrorMessage();
                        Log.e(CROSSP_LOG_TAG, errorMessage);
                        continue;
                    }

                    availableAds.add(new AvailableAd(ads[i], fileData, ads[i].getFile().getExtension().equals(".mp4") ? AvailableAd.FileType.Video : AvailableAd.FileType.Image));
                }

                if (availableAds.size() == 0) {
                    String errorMessage = "AvailableAds count = 0";
                    Log.e(CROSSP_LOG_TAG, errorMessage);
                    onAdsFailedToLoadListener.OnAdsFailedToLoad(errorMessage);
                }

                onAdsLoadedListener.onAdsLoaded(availableAds);
            }
        }).start();
    }

    public void showRandomAd(View clickEventCaller, VideoView view) {

        AvailableAd ad = getRandomAvailableAd();

        new Thread(new Runnable() {
            @Override
            public void run() {
                String fileName = "crossptemp.mp4";
                File externalDir = context.getExternalFilesDir(null);
                File file = new File(externalDir, fileName);
                if(!file.exists()) {
                    try {
                        file.createNewFile();
                    } catch (IOException e) {
                        e.printStackTrace();
                        return;
                    }
                }
                FileOutputStream out = null;
                try {
                    out = new FileOutputStream(file);
                } catch (FileNotFoundException e) {
                    e.printStackTrace();
                    return;
                }
                try {
                    out.write(ad.getFileData());
                    out.flush();
                    out.close();
                } catch (IOException e) {
                    e.printStackTrace();
                    return;
                }

                view.post(new Runnable() {
                    @Override
                    public void run() {
                        activeAd = ad;

                        clickEventCaller.setVisibility(View.VISIBLE);
                        clickEventCaller.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View view) {
                                adClickReport(activeAd);

                                Intent intent = new Intent(Intent.ACTION_VIEW);
                                intent.setData(Uri.parse(activeAd.getAd().getUrl()));
                                intent.setPackage("com.android.vending");
                                context.startActivity(intent);
                            }
                        });

                        Uri uri = Uri.fromFile(file);
                        view.setVisibility(View.VISIBLE);
                        view.setVideoURI(uri);
                        view.setOnPreparedListener(new MediaPlayer.OnPreparedListener() {
                            @Override
                            public void onPrepared(MediaPlayer mp) {
                                mp.setLooping(true);
                            }
                        });
                        view.start();

                        adShowReport(activeAd);
                    }
                });
            }
        }).start();
    }

    private int getRandomNumber(int min, int max) {
        return (int) ((Math.random() * (max - min)) + min);
    }

    private AvailableAd getRandomAvailableAd() {
        return availableAds.get(getRandomNumber(0, availableAds.size()));
    }

    private void adShowReport(AvailableAd ad) {
        String requestUrl = SERVER_URL + "Clients/adshowreport?api-version=" + API_VERSION + "&externalId=" + EXTERNAL_ID + "&adId=" + ad.getAd().getId();
        new ReportAdShowThread(requestUrl).start();
    }

    private void adClickReport(AvailableAd ad) {
        String requestUrl = SERVER_URL + "Clients/adclickreport?api-version=" + API_VERSION + "&externalId=" + EXTERNAL_ID + "&adId=" + ad.getAd().getId();
        new ReportAdShowThread(requestUrl).start();
    }
}
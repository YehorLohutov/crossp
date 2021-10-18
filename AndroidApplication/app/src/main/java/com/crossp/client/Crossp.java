package com.crossp.client;

import android.content.Context;
import android.content.Intent;
import android.media.MediaPlayer;
import android.net.Uri;
import android.util.Log;
import android.view.View;
import android.widget.VideoView;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonArrayRequest;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;

public class Crossp {

    private static Crossp instance = null;
    public static Crossp getInstance() {
        if (instance == null)
            instance = new Crossp();
        return instance;
    }

    private Context context;
    private ArrayList<Ad> availableAds;
    private AdData activeAdData;
    private AdData nextAdData;

    public enum State { NotInitialized, Initializing, Initialized, PreparingToShow, ReadyToShow }
    private State currentState;
    public State getCurrentState() { return currentState; }

    public boolean readyToShow() { return currentState == State.ReadyToShow; }

    private Crossp() {
        context = null;
        availableAds = null;
        activeAdData = null;
        nextAdData = null;
        currentState = State.NotInitialized;
    }

    public void initialize(Context context, OnInitializationCompletedListener onInitializationCompletedListener, OnInitializationFailedListener onInitializationFailedListener) {
        if (currentState == State.Initialized || currentState == State.Initializing)
            return;
        this.context = context;
        currentState = State.Initializing;

        loadAvailableAds(new OnAdsLoadedListener() {
            @Override
            public void onAdsLoaded(ArrayList<Ad> ads) {
                availableAds = ads;
                currentState = State.Initialized;
                //prepareNextAdData(null);
                onInitializationCompletedListener.onInitializationCompleted();
            }
        }, new OnAdsFailedToLoadListener() {
            @Override
            public void OnAdsFailedToLoad(String errorMessage) {
                currentState = State.NotInitialized;
                onInitializationFailedListener.onInitializationFailed(errorMessage);
            }
        });
    }

    private void loadAvailableAds(OnAdsLoadedListener onAdsLoadedListener, OnAdsFailedToLoadListener onAdsFailedToLoadListener) {


        RequestQueue mainQueue = Volley.newRequestQueue(context);
        String url = Settings.SERVER_URL + "Clients/availableads?api-version=" + Settings.API_VERSION + "&externalId=" + Settings.EXTERNAL_ID;

        JsonArrayRequest jsonArrayRequest = new JsonArrayRequest(url,
                new Response.Listener<JSONArray>() {
                    @Override
                    public void onResponse(JSONArray response) {
                        ArrayList<Ad> result = null;
                        try {
                            result = parseAdsFrom(response);
                        } catch (Exception ex) {
                            ex.printStackTrace();
                            onAdsFailedToLoadListener.OnAdsFailedToLoad(ex.getMessage());
                        }
                        //
                        if (result == null || result.size() == 0)
                        {
                            onAdsFailedToLoadListener.OnAdsFailedToLoad("Loaded ads array is null or empty.");
                            return;
                        }
                        else
                        {
                            onAdsLoadedListener.onAdsLoaded(result);
                            return;
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        onAdsFailedToLoadListener.OnAdsFailedToLoad(error.getMessage());
                    }
        });
        mainQueue.add(jsonArrayRequest);
    }

    private void loadAdData(Ad ad, OnAdDataLoadedListener onAdDataLoadedListener, OnAdsFailedToLoadListener onAdsFailedToLoadListener) {
       try {
           RequestQueue loadAdDataQueue = Volley.newRequestQueue(context);

           String requestUrl = Settings.SERVER_URL + "Clients/adfile?api-version=" + Settings.API_VERSION + "&adFileId=" + ad.getFile().getId();
           FileDataRequest fileDataRequest = new FileDataRequest(requestUrl, response -> {
               AdData adData = new AdData(ad, response, ad.getFile().getExtension().equals(".mp4") ? AdData.FileType.Video : AdData.FileType.Image);
               onAdDataLoadedListener.onAdDataLoaded(adData);
           }, error -> {
               onAdsFailedToLoadListener.OnAdsFailedToLoad(error.getMessage());
           });
           loadAdDataQueue.add(fileDataRequest);
       } catch (Exception exception) {
           onAdsFailedToLoadListener.OnAdsFailedToLoad(exception.getMessage());
       }
    }


    private ArrayList<Ad> parseAdsFrom (JSONArray jsonArray) throws Exception {
        ArrayList<Ad> result = new ArrayList<>();
        for (int i = 0; i < jsonArray.length(); i++) {
            try {
                result.add(parseAdFrom(jsonArray.getJSONObject(i)));
            } catch (JsonSyntaxException e) {
                throw new Exception(e.getMessage() + " Error while parsing ads json array.");
            }
        }
        return result;
    }

    private Ad parseAdFrom (JSONObject jsonObject) throws Exception{
        Ad result = null;
        Gson gson = new Gson();
        try {
            result = gson.fromJson(jsonObject.toString(), Ad.class);
        } catch (JsonSyntaxException e) {
            throw new Exception(e.getMessage() + " Error while parsing ad json object.");
        }
        return result;
    }

    public void showRandomAd(View clickEventCaller, VideoView view) {
        if (currentState == State.NotInitialized || currentState == State.Initializing) {
            return;
        }
        else if (currentState == State.Initialized)
        {
            prepareNextAdData(new OnAdDataPreparedListener() {
                @Override
                public void onAdDataPrepared() {
                    showRandomAd(clickEventCaller, view);
                }
            });
            return;
        }
        else if (currentState == State.PreparingToShow)
        {
            return;
        }

        activeAdData = nextAdData;
        prepareNextAdData(null);

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
            out.write(activeAdData.getFileData());
            out.flush();
            out.close();
        } catch (IOException e) {
            e.printStackTrace();
            return;
        }

        clickEventCaller.setVisibility(View.VISIBLE);
        clickEventCaller.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                adClickReport(activeAdData);

                Intent intent = new Intent(Intent.ACTION_VIEW);
                intent.setData(Uri.parse(activeAdData.getAd().getUrl()));
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

        adShowReport(activeAdData);
    }

    private int getRandomNumber(int min, int max) {
        return (int) ((Math.random() * (max - min)) + min);
    }

    private Ad getRandomAvailableAd() {
        return availableAds.get(getRandomNumber(0, availableAds.size()));
    }

    public void prepareNextAdData(OnAdDataPreparedListener onAdDataPreparedListener) {
        if (currentState == State.NotInitialized || currentState == State.Initializing || currentState == State.PreparingToShow)
            return;
        currentState = State.PreparingToShow;
        Ad nextAd = getRandomAvailableAd();
//        if (activeAdData != null && activeAdData.getAd().getId() == nextAd.getId())
//        {
//            nextAdData = activeAdData;
//            return;
//        }
        loadAdData(nextAd, new OnAdDataLoadedListener() {
            @Override
            public void onAdDataLoaded(AdData adData) {
                nextAdData = adData;
                currentState = State.ReadyToShow;
                if (onAdDataPreparedListener != null) {
                    onAdDataPreparedListener.onAdDataPrepared();
                }
            }
        }, new OnAdsFailedToLoadListener() {
            @Override
            public void OnAdsFailedToLoad(String errorMessage) {
                currentState = State.Initialized;
            }
        });
    }

    private void adShowReport(AdData ad) {
        String requestUrl = Settings.SERVER_URL + "Clients/adshowreport?api-version=" + Settings.API_VERSION + "&externalId=" + Settings.EXTERNAL_ID + "&adId=" + ad.getAd().getId();

        RequestQueue queue = Volley.newRequestQueue(context);

        HttpResultCodeRequest stringRequest = new HttpResultCodeRequest(requestUrl,
                new Response.Listener<Integer>() {
                    @Override
                    public void onResponse(Integer response) {
                        if (response == 200) {
                            Log.i(Settings.CROSSP_LOG_TAG, "Ad id: " + ad.getAd().getId() + " show report successful.");
                        } else {
                            Log.i(Settings.CROSSP_LOG_TAG, "Ad id: " + ad.getAd().getId() + " show report not successful.");
                        }
                    }
                }, new Response.ErrorListener() {
                     @Override
                     public void onErrorResponse(VolleyError error) {
                         Log.i(Settings.CROSSP_LOG_TAG, "Ad id: " + ad.getAd().getId() + " show report not successful. " + error.getMessage());
                     }
        });

        queue.add(stringRequest);
    }

    private void adClickReport(AdData ad) {
        String requestUrl = Settings.SERVER_URL + "Clients/adclickreport?api-version=" + Settings.API_VERSION + "&externalId=" + Settings.EXTERNAL_ID + "&adId=" + ad.getAd().getId();

        RequestQueue queue = Volley.newRequestQueue(context);

        HttpResultCodeRequest stringRequest = new HttpResultCodeRequest(requestUrl,
                new Response.Listener<Integer>() {
                    @Override
                    public void onResponse(Integer response) {
                        if (response == 200) {
                            Log.i(Settings.CROSSP_LOG_TAG, "Ad id: " + ad.getAd().getId() + " click report successful.");
                        } else {
                            Log.i(Settings.CROSSP_LOG_TAG, "Ad id: " + ad.getAd().getId() + " click report not successful.");
                        }
                    }
                }, new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                Log.i(Settings.CROSSP_LOG_TAG, "Ad id: " + ad.getAd().getId() + " click report not successful. " + error.getMessage());
            }
        });

        queue.add(stringRequest);
    }
}
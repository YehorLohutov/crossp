package com.crossp.client;

import android.util.Log;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;

import javax.net.ssl.HttpsURLConnection;

public class ReportAdClickThread extends Thread {
    private String requestUrl;

    public ReportAdClickThread(String requestUrl) {
        this.requestUrl = requestUrl;
    }

    public void run() {
        URL url = null;
        try {
            url = new URL(requestUrl);
        } catch (MalformedURLException e) {
            Log.e(Crossp.CROSSP_LOG_TAG, e.getMessage() + " Request url string: " + requestUrl + ".");
            return;
        }

        HttpsURLConnection connection = null;
        int responseCode = 0;
        try {
            connection = (HttpsURLConnection) url.openConnection();
            connection.setRequestMethod("GET");
            connection.connect();
            responseCode = connection.getResponseCode();
        } catch (IOException e) {
            Log.e(Crossp.CROSSP_LOG_TAG, e.getMessage());
        }

        if(responseCode != HttpsURLConnection.HTTP_OK)
        {
            Log.e(Crossp.CROSSP_LOG_TAG, "Error while report ad click.");
        }
    }
}

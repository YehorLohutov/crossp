package com.crossp.client;

import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.MalformedURLException;
import java.net.URL;

import javax.net.ssl.HttpsURLConnection;

public class LoadAdsThread extends Thread {

    private Ad[] ads;
    public Ad[] getAds() { return ads; }

    private String errorMessage;
    public String getErrorMessage() { return errorMessage; }

    private String requestUrl;

    public LoadAdsThread(String requestUrl) {
        ads = null;
        errorMessage = null;
        this.requestUrl = requestUrl;
    }

    public void run() {
        String jsonArray = null;
        try {
            jsonArray = loadJsonArray();
        } catch (Exception e) {
            errorMessage = e.getMessage() + " Error while loading ads from server.";
            Log.e(Crossp.CROSSP_LOG_TAG, errorMessage);
            return;
        }

        try {
            ads = parseAdsFrom(jsonArray);
        } catch (Exception e) {
            errorMessage = e.getMessage();
            Log.e(Crossp.CROSSP_LOG_TAG, errorMessage);
            return;
        }
    }

    private String loadJsonArray() throws Exception {
        StringBuilder result = new StringBuilder();

        URL url = null;
        try {
            url = new URL(requestUrl);
        } catch (MalformedURLException e) {
            throw new Exception(e.getMessage() + " Request url string: " + requestUrl + ".");
        }

        HttpsURLConnection connection = null;
        try {
            connection = (HttpsURLConnection) url.openConnection();
        } catch (IOException e) {
            throw new Exception(e.getMessage() + " Error while url open connection.");
        }

        InputStream stream = null;
        try {
            stream = connection.getInputStream();
        } catch (IOException e) {
            throw new Exception(e.getMessage() + " Error while getting InputStream.");
        }

        BufferedReader reader = new BufferedReader(new InputStreamReader(stream));
        String line = null;

        try {
        while ((line = reader.readLine()) != null) {
                result.append(line);
        }
        } catch (IOException e) {
            throw new Exception(e.getMessage() + " Error while reading json from input.");
        }

        try {
            if (reader != null) {
                reader.close();
            }
        } catch (IOException e) {
            throw new Exception(e.getMessage() + " Error while closing BufferedReader.");
        }

        try {
            if (stream != null) {
                stream.close();
            }
        } catch (IOException e) {
            throw new Exception(e.getMessage() + " Error while closing InputStream.");
        }

        if (connection != null) {
            connection.disconnect();
        }

        return result.toString();
    }

    private Ad[] parseAdsFrom (String json) throws Exception {
        Ad[] result = null;
        Gson gson = new Gson();
        try {
            result = gson.fromJson(json.toString(), Ad[].class);
        } catch (JsonSyntaxException e) {
            throw new Exception(e.getMessage() + " Error while parsing ads json array.");
        }
        return result;
    }
}

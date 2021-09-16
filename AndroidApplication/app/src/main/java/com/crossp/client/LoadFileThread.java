package com.crossp.client;

import android.util.Log;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.MalformedURLException;
import java.net.URL;

import javax.net.ssl.HttpsURLConnection;

public class LoadFileThread extends Thread{

    private String requestUrl;

    private byte[] data;
    public byte[] getData() { return data; }

    private String errorMessage;
    public String getErrorMessage() { return errorMessage; }

    public LoadFileThread(String requestUrl) {
        this.requestUrl = requestUrl;
        data = null;
        errorMessage = null;
    }

    public void run() {
        try {
            data = loadFile();
        } catch (Exception e) {
            errorMessage = e.getMessage() + " Error while loading ad file from server.";
            Log.e(Crossp.CROSSP_LOG_TAG, errorMessage);
            return;
        }
    }

    private byte[] loadFile() throws Exception {
        byte[] result = null;

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

        BufferedInputStream reader = null;
        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
        reader = new BufferedInputStream(stream);
        byte dataBuffer[] = new byte[1024];
        int bytesRead;

        try {
            while ((bytesRead = reader.read(dataBuffer, 0, 1024)) != -1) {
                outputStream.write(dataBuffer, 0, bytesRead);
            }
        } catch (IOException e) {
            throw new Exception(e.getMessage() + " Error while reading file data from input.");
        }

        result = outputStream.toByteArray();


        try {
            if (outputStream != null) {
                outputStream.close();
            }
        } catch (IOException e) {
            throw new Exception(e.getMessage() + " Error while closing ByteArrayOutputStream.");
        }

        try {
            if (reader != null) {
                reader.close();
            }
        } catch (IOException e) {
            throw new Exception(e.getMessage() + " Error while closing BufferedInputStream.");
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

        return  result;
    }
}

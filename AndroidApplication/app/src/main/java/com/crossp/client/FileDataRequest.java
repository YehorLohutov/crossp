package com.crossp.client;

import android.util.Pair;

import com.android.volley.NetworkResponse;
import com.android.volley.Request;
import com.android.volley.Response;
import com.android.volley.toolbox.HttpHeaderParser;


public class FileDataRequest extends Request<byte[]> {

    private Response.Listener<byte[]> listener;
    private Response.ErrorListener errorListener;


    public FileDataRequest(String url, Response.Listener<byte[]> listener, Response.ErrorListener errorListener) {
        super(Method.GET, url, errorListener);
        this.listener = listener;
        this.errorListener = errorListener;
    }

    @Override
    protected void deliverResponse(byte[] response) {
        listener.onResponse(response);
    }

    @Override
    protected Response<byte[]> parseNetworkResponse(NetworkResponse response) {
            byte[] fileData = response.data;
            //if (response.statusCode == Response.)
            return Response.success(fileData, HttpHeaderParser.parseCacheHeaders(response));
    }
}

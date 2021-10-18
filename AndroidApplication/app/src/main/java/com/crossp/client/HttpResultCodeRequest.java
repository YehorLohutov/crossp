package com.crossp.client;

import com.android.volley.NetworkResponse;
import com.android.volley.Request;
import com.android.volley.Response;
import com.android.volley.toolbox.HttpHeaderParser;

public class HttpResultCodeRequest extends Request<Integer> {
    private Response.Listener<Integer> listener;
    private Response.ErrorListener errorListener;


    public HttpResultCodeRequest(String url, Response.Listener<Integer> listener, Response.ErrorListener errorListener) {
        super(Method.GET, url, errorListener);
        this.listener = listener;
        this.errorListener = errorListener;
    }

    @Override
    protected void deliverResponse(Integer response) {
        listener.onResponse(response);
    }

    @Override
    protected Response<Integer> parseNetworkResponse(NetworkResponse response) {
        return Response.success(response.statusCode, HttpHeaderParser.parseCacheHeaders(response));
    }
}

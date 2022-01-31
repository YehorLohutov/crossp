package com.crossp.androidapplication;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.text.Layout;
import android.view.View;
import android.widget.Toast;
import android.widget.VideoView;

import java.util.ArrayList;
import com.crossp.client.*;

public class MainActivity extends AppCompatActivity {

    private View stickerLayout = null;
    private VideoView videoView = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        videoView = findViewById(R.id.videoView);
        stickerLayout = findViewById(R.id.layout);

        Crossp.getInstance().initialize(this,
                new OnInitializationCompletedListener() {
                    @Override
                    public void onInitializationCompleted() {
                        Crossp.getInstance().showRandomAd(stickerLayout, videoView);
                    }
                },
                new OnInitializationFailedListener() {
                    @Override
                    public void onInitializationFailed(String errorMessage) {
                    }
                });

    }

    public void onClick(View v) {
        if (!Crossp.getInstance().readyToShow())
            return;
        Crossp.getInstance().showRandomAd(stickerLayout, videoView);
    }

    public void hide(View v) {
        videoView.stopPlayback();
        stickerLayout.setVisibility(View.INVISIBLE);
    }
}
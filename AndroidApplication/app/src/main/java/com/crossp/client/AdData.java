package com.crossp.client;

public class AdData {
    public enum FileType { Image, Video }

    private Ad ad;
    public Ad getAd() { return ad; }

    public byte[] fileData;
    public byte[] getFileData() { return fileData; }

    private FileType type;
    public FileType getType() { return type; }

    {
        ad = null;
        fileData = null;
        type = null;
    }

    public AdData(Ad ad, byte[] fileData, FileType type)
    {
        this.ad = ad;
        this.fileData = fileData;
        this.type = type;
    }
}

package com.crossp.client;

public class Ad {
    private int id;
    public int getId() { return id; }

    private String name;
    public String getName() { return name; }

    private String url;
    public String getUrl() { return url; }

    private File file;
    public File getFile() { return file; }

    {
        id = -1;
        name = null;
        url = null;
        file = null;
    }

    public Ad(int id, String name, String url, File file) {
        this.id = id;
        this.name = name;
        this.url = url;
        this.file = file;
    }
}

package com.crossp.client;

public class File {
    private int id;
    public int getId() { return id; }

    private String name;
    public String getName() { return name; }

    private String path;
    public String getPath() { return path; }

    private String extension;
    public String getExtension() { return extension; }

    {
        id = -1;
        name = null;
        path = null;
        extension = null;
    }

    public File(int id, String name, String path, String extension) {
        this.id = id;
        this.name = name;
        this.path = path;
        this.extension = extension;
    }
}

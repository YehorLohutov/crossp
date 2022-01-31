package com.crossp.client;

public class Ad {
    private String externalId;
    public String getExternalId() { return externalId; }

    private String name;
    public String getName() { return name; }

    private String url;
    public String getUrl() { return url; }

    private String projectExternalId;
    public String getProjectExternalId() { return projectExternalId; }

    private String fileExternalId;
    public String getFileExternalId() { return fileExternalId; }

    {
        externalId = null;
        name = null;
        url = null;
        projectExternalId = null;
        fileExternalId = null;
    }

    public Ad(String externalId, String name, String url, String projectExternalId, String fileExternalId) {
        this.externalId = externalId;
        this.name = name;
        this.url = url;
        this.projectExternalId = projectExternalId;
        this.fileExternalId = fileExternalId;
    }
}

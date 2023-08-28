

using System;

[Serializable]
public class Resource
{
    public bool async;
    public bool ws;
    public string taskName;
    public bool repeatTask;
    public bool fileExists;
}

[Serializable]
public class Root
{
    public Resource resource;
}

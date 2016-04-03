﻿// <auto-generated/>
using System;
using System.Collections.Generic;

namespace Fenton.TeamServices.BuildRestApi
{
    public class GroupedBuild
    {
        public string Name { get; set; }

        public List<BuildChangeset> Builds { get; set; }
    }

    public class Builds
    {
        public int count { get; set; }
        public List<BuildChangeset> value { get; set; }
    }

    // Auto Generated from json2csharp
    public class Self
    {
        public string href { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }

    public class Timeline
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
        public Web web { get; set; }
        public Timeline timeline { get; set; }
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
    }

    public class Definition
    {
        public string type { get; set; }
        public int revision { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public Project project { get; set; }
    }

    public class Pool
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Queue
    {
        public Pool pool { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class RequestedFor
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }

    public class RequestedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
        public bool isContainer { get; set; }
    }

    public class LastChangedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
        public bool isContainer { get; set; }
    }

    public class OrchestrationPlan
    {
        public string planId { get; set; }
    }

    public class Logs
    {
        public int id { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Repository
    {
        public string id { get; set; }
        public string type { get; set; }
        public object clean { get; set; }
        public bool checkoutSubmodules { get; set; }
    }

    public class BuildChangeset
    {
        public Links _links { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public Definition definition { get; set; }
        public string buildNumber { get; set; }
        public int buildNumberRevision { get; set; }
        public Project project { get; set; }
        public string uri { get; set; }
        public string sourceBranch { get; set; }
        public string sourceVersion { get; set; }
        public string status { get; set; }
        public Queue queue { get; set; }
        public DateTime queueTime { get; set; }
        public string priority { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public string reason { get; set; }
        public string result { get; set; }
        public RequestedFor requestedFor { get; set; }
        public RequestedBy requestedBy { get; set; }
        public DateTime lastChangedDate { get; set; }
        public LastChangedBy lastChangedBy { get; set; }
        public OrchestrationPlan orchestrationPlan { get; set; }
        public Logs logs { get; set; }
        public Repository repository { get; set; }
        public bool keepForever { get; set; }
        public string parameters { get; set; }
    }
}
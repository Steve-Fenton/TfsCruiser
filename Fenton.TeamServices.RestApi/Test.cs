﻿using System;
using System.Collections.Generic;

namespace Fenton.TeamServices.TestRestApi
{
    public class GroupedTest
    {
        public string Name { get; set; }

        public IList<Value> TestRuns { get; set; }
    }

    public class TestRuns
    {
        public List<Value> value { get; set; }
        public int count { get; set; }
    }

    // Auto Generated from json2csharp

    public class Owner
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Plan
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class LastUpdatedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }

    public class Value
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public bool isAutomated { get; set; }
        public string iteration { get; set; }
        public Owner owner { get; set; }
        public Project project { get; set; }
        public DateTime startedDate { get; set; }
        public DateTime completedDate { get; set; }
        public string state { get; set; }
        public Plan plan { get; set; }
        public string postProcessState { get; set; }
        public int totalTests { get; set; }
        public int passedTests { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public LastUpdatedBy lastUpdatedBy { get; set; }
        public int revision { get; set; }
        public string comment { get; set; }
        public int? unanalyzedTests { get; set; }
    }
}

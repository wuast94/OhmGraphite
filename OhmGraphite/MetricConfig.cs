﻿using System;

namespace OhmGraphite
{
    public class MetricConfig
    {
        public MetricConfig(TimeSpan interval, GraphiteConfig graphite, InfluxConfig influx)
        {
            Interval = interval;
            Graphite = graphite;
            Influx = influx;
        }

        public TimeSpan Interval { get; }
        public GraphiteConfig Graphite { get; }
        public InfluxConfig Influx { get; }

        public static MetricConfig ParseAppSettings(IAppConfig config)
        {
            if (!int.TryParse(config["interval"], out int seconds))
            {
                seconds = 5;
            }

            var interval = TimeSpan.FromSeconds(seconds);

            var type = config["type"] ?? "graphite";
            GraphiteConfig gconfig = null;
            InfluxConfig iconfig = null;

            switch (type.ToLowerInvariant())
            {
                case "graphite":
                    gconfig = GraphiteConfig.ParseAppSettings(config);
                    break;
                case "influxdb":
                case "influx":
                    iconfig = InfluxConfig.ParseAppSettings(config);
                    break;
            }

            return new MetricConfig(interval, gconfig, iconfig);
        }
    }
}
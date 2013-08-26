﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHgMirror.Runner
{
    public class Settings
    {
        public Uri ApiEndpointUrl { get; set; }
        public string ApiPassword { get; set; }

        /// <summary>
        /// Must be an absolute path
        /// </summary>
        public string RepositoriesDirectoryPath { get; set; }
        public int BatchSize { get; set; }
        public int SecondsBetweenConfigurationCountChecks { get; set; }


        public Settings()
        {
            BatchSize = 50;
            SecondsBetweenConfigurationCountChecks = 60;
        }
    }
}

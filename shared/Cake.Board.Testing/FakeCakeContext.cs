// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Board.Testing
{
    /// <summary>
    /// Fake implementation of <see cref="ICakeContext"/>.
    /// </summary>
    public class FakeCakeContext : ICakeContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeCakeContext"/> class.
        /// </summary>
        /// <param name="fileSystemBehaviour">Todo1.</param>
        /// <param name="environmentBehaviour">Todo2.</param>
        /// <param name="globberBehaviour">Todo3.</param>
        /// <param name="logBehaviour">Todo4.</param>
        /// <param name="argumentsBehaviour">Todo5.</param>
        /// <param name="processRunnerBehaviour">Todo6.</param>
        /// <param name="registryBehaviour">Todo7.</param>
        /// <param name="toolsBehaviour">Todo8.</param>
        /// <param name="dataBehaviour">Todo9.</param>
        /// <param name="configurationBehaviour">Todo10.</param>
        public FakeCakeContext(
            Func<IFileSystem> fileSystemBehaviour = null,
            Func<ICakeEnvironment> environmentBehaviour = null,
            Func<IGlobber> globberBehaviour = null,
            Func<ICakeLog> logBehaviour = null,
            Func<ICakeArguments> argumentsBehaviour = null,
            Func<IProcessRunner> processRunnerBehaviour = null,
            Func<IRegistry> registryBehaviour = null,
            Func<IToolLocator> toolsBehaviour = null,
            Func<ICakeDataResolver> dataBehaviour = null,
            Func<ICakeConfiguration> configurationBehaviour = null)
        {
            this.FileSystemBehaviour = fileSystemBehaviour;
            this.EnviromentBehaviour = environmentBehaviour;
            this.GlobberBehaviour = globberBehaviour;
            this.LogBehaviour = logBehaviour;
            this.ArgumentsBehaviour = argumentsBehaviour;
            this.ProcessRunnerBehaviour = processRunnerBehaviour;
            this.RegistryBehaviour = registryBehaviour;
            this.ToolsBehaviour = toolsBehaviour;
            this.DataBehaviour = dataBehaviour;
            this.ConfigurationBehaviour = configurationBehaviour;
        }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<IFileSystem> FileSystemBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<ICakeEnvironment> EnviromentBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<IGlobber> GlobberBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<ICakeLog> LogBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<ICakeArguments> ArgumentsBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<IProcessRunner> ProcessRunnerBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<IRegistry> RegistryBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<IToolLocator> ToolsBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<ICakeDataResolver> DataBehaviour { get; set; }

        /// <summary>
        /// Gets or sets todo.
        /// </summary>
        public Func<ICakeConfiguration> ConfigurationBehaviour { get; set; }

        /// <inheritdoc/>
        public IFileSystem FileSystem => this.FileSystemBehaviour.Invoke();

        /// <inheritdoc/>
        public ICakeEnvironment Environment => this.EnviromentBehaviour.Invoke();

        /// <inheritdoc/>
        public IGlobber Globber => this.GlobberBehaviour.Invoke();

        /// <inheritdoc/>
        public ICakeLog Log => this.LogBehaviour.Invoke();

        /// <inheritdoc/>
        public ICakeArguments Arguments => this.ArgumentsBehaviour.Invoke();

        /// <inheritdoc/>
        public IProcessRunner ProcessRunner => this.ProcessRunnerBehaviour.Invoke();

        /// <inheritdoc/>
        public IRegistry Registry => this.RegistryBehaviour.Invoke();

        /// <inheritdoc/>
        public IToolLocator Tools => this.ToolsBehaviour.Invoke();

        /// <inheritdoc/>
        public ICakeDataResolver Data => this.DataBehaviour.Invoke();

        /// <inheritdoc/>
        public ICakeConfiguration Configuration => this.ConfigurationBehaviour.Invoke();
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace TinyPng.Tools
{
    public sealed class ImageCompressor
    {
        private readonly Object _syncRoot = new Object();

        private IReadOnlyDictionary<String, FileEndpoints> ImageEndpoints { get; set; }

        private Queue<IEnumerable<String>> UploadBatches { get; set; }

        private Int32 FreeJobs { get; set; }

        private Int32 MaxJobs { get; set; }

        private Boolean IsStarted { get; set; } = false;

        public ImageCompressor(ImageCompressorSettings settings)
        {
            this.ImageEndpoints = settings.ImageEndpoints;
            this.FreeJobs = settings.MaxParallelJobs;
            this.MaxJobs = settings.MaxParallelJobs;
            this.UploadBatches = new Queue<IEnumerable<String>>(
                this.ImageEndpoints.Keys
                    .Select((v, i) => new { v = v, g = i / settings.MaxFilesPerJob })
                    .GroupBy(x => x.g)
                    .Select(g => g.Select(x => x.v)));
        }

        public void Start() 
        {
            if (this.IsStarted)
            {
                throw new InvalidOperationException("Already started");
            }
            else
            {
                this.IsStarted = true;
                this.OnStarted();
                this.StartJobs();
            }
        }

        private void StartJobs()
        {
            lock (this._syncRoot)
            {
                if (this.UploadBatches.Count == 0)
                {
                    if (this.MaxJobs == this.FreeJobs)
                    {
                        this.OnStopped();
                    }
                }
                else
                {
                    var jobCount = Math.Min(this.FreeJobs, this.UploadBatches.Count);
                    for (var i = 0; i < jobCount; i++)
                    {
                        this.AcquireJob();
                    }
                }
            }
        }

        private void AcquireJob()
        {
            this.FreeJobs--;

            var batch = this.UploadBatches.Dequeue();
            var job = new CompressorJob(this.ImageEndpoints, batch);

            job.Completed += this.OnImageCompressCompleted;
            job.ProgressChanged += this.OnImageProgressChanged;
            job.Disposed += this.OnJobDisposed;

            ThreadPool.QueueUserWorkItem(o =>
                {
                    job.Start();
                    job.Dispose();
                });
        }

        private void ReleaseJob() 
        {
            lock (this._syncRoot)
            {
                this.FreeJobs++;
            }
        }

        public event CompressProgressChanged ImageProgressChanged = (o, e) => {};

        public event CompressCompleted ImageCompressCompleted = (o, e) => {};

        public event EventHandler Stopped = (o, e) => {};

        public event EventHandler Started = (o, e) => {};

        private void OnImageProgressChanged(Object sender, CompressProgressChangedEventArgs e)
        {
            this.ImageProgressChanged(sender, e);
        }

        private void OnImageCompressCompleted(Object sender, CompressCompletedEventArgs e)
        {
            this.ImageCompressCompleted(sender, e);
        }

        private void OnJobDisposed(Object sender, EventArgs e)
        {
            this.ReleaseJob();
            this.StartJobs();
        }

        private void OnStarted()
        {
            this.Started(this, EventArgs.Empty);
        }

        private void OnStopped()
        {
            this.Stopped(this, EventArgs.Empty);
        }
    }

    public sealed class CompressProgressChangedEventArgs : EventArgs
    {
        public String Id { get; private set; }

        public Single ProgressPercentage { get; private set; }

        public FileEndpoints Endpoints { get; private set; }

        public CompressProgressChangedEventArgs(String id, FileEndpoints endpoints, Single progressPercentage)
        {
            this.Id = id;
            this.Endpoints = endpoints;
            this.ProgressPercentage = progressPercentage;
        }
    }

    public sealed class CompressCompletedEventArgs : EventArgs
    {
        public String Id { get; private set; }

        public FileEndpoints Endpoints { get; private set; }

        public CompressCompletedEventArgs(String id, FileEndpoints endpoints)
        {
            this.Id = id;
            this.Endpoints = endpoints;
        }
    }

    public delegate void CompressProgressChanged(object sender, CompressProgressChangedEventArgs args);

    public delegate void CompressCompleted(object sender, CompressCompletedEventArgs args);
}


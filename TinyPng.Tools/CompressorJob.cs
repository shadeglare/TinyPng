using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace TinyPng.Tools
{
    internal sealed class CompressorJob : IDisposable
    {
        private IReadOnlyDictionary<String, FileEndpoints> ImageEndpoints { get; set; }

        private IEnumerable<String> ImageIds { get; set; }

        private IWebDriver WebDriver { get; set; }

        private IJavaScriptExecutor JavaScriptExecutor { get; set; }

        private WebClient WebClient { get; set; }

        private IWebElement UploadElement { get; set; }

        public CompressorJob(IReadOnlyDictionary<String, FileEndpoints> imageEndpoints, IEnumerable<String> imageIds)
        {
            this.ImageEndpoints = imageEndpoints;
            this.ImageIds = imageIds;

            this.WebDriver = new FirefoxDriver();
            this.JavaScriptExecutor = this.WebDriver as IJavaScriptExecutor;
            this.WebClient = new WebClient();
        }

        public void Start()
        {
            this.OpenPage();
            this.AdjustUploadElement();
            this.ProcessAllImages();
        }

        private void OpenPage()
        {
            this.WebDriver.Navigate().GoToUrl("https://tinypng.com");
        }

        private void AdjustUploadElement()
        {
            this.UploadElement = this.WebDriver.FindElement(By.XPath(@"/html/body/header/section/input"));
            this.JavaScriptExecutor.ExecuteScript("arguments[0].style.top='0';arguments[0].style.zIndex=100;", this.UploadElement);
        }

        private String ExtractUploadedFileUrl(Int32 position)
        {
            var xpath = String.Format(@"//*[@id=""{0}""]/div[3]/a", position);
            return this.WebDriver.FindElement(By.XPath(xpath)).GetAttribute("href");
        }

        private void ProcessAllImages()
        {
            this.ImageIds.ForEach(this.ProcessImage);
        }

        private void ProcessImage(String id, Int32 position)
        {
            var endpoints = this.ImageEndpoints[id];
            this.UploadFile(id, endpoints);
            this.DownloadFile(id, endpoints, position);
        }

        private void UploadFile(String id, FileEndpoints endpoints)
        {
            this.OnProgressChanged(new CompressProgressChangedEventArgs(id, endpoints, 10));
            this.UploadElement.SendKeys(endpoints.Source.FilePath);
            this.OnProgressChanged(new CompressProgressChangedEventArgs(id, endpoints, 50));
        }

        private void DownloadFile(String id, FileEndpoints endpoints, Int32 position)
        {
            var uploadedFileUrl = this.ExtractUploadedFileUrl(position);
            DirectoryHelper.GetOrCreateDirectory(endpoints.Target.DirectoryName);
            this.OnProgressChanged(new CompressProgressChangedEventArgs(id, endpoints, 60));
            this.WebClient.DownloadFile(uploadedFileUrl, endpoints.Target.FilePath);
            this.OnProgressChanged(new CompressProgressChangedEventArgs(id, endpoints, 100));
            this.OnCompleted(new CompressCompletedEventArgs(id, endpoints));
        }

        public event CompressProgressChanged ProgressChanged = (o, e) => {};

        public event CompressCompleted Completed = (o, e) => {};

        public event EventHandler Disposed = (o, e) => {};

        public void OnProgressChanged(CompressProgressChangedEventArgs e)
        {
            this.ProgressChanged(this, e);
        }

        public void OnCompleted(CompressCompletedEventArgs e)
        {
            this.Completed(this, e);
        }

        private void OnDisposed()
        {
            this.Disposed(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            this.OnDisposed();
            this.WebDriver.Dispose();
            this.WebClient.Dispose();
        }
    }
}
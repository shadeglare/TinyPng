using System;

namespace TinyPng.Frontend
{

    public sealed class AppViewModel
    {
        private String _sourcePath = null;

        public String SourcePath
        {
            get
            { 
                return this._sourcePath; 
            }
            set
            { 
                this._sourcePath = value; 
                this.SourcePathChanged(this, this._sourcePath);
            }
        }

        private String _targetPath = null;

        public String TargetPath
        {
            get
            { 
                return this._targetPath; 
            }
            set
            { 
                this._targetPath = value;
                this.TargetPathChanged(this, this._targetPath);
            }
        }

        private Boolean _canStart = false;

        public Boolean CanStart
        {
            get
            {
                return this._canStart;
            }
            set
            {
                this._canStart = value;
                this.CanStartChanged(this, this._canStart); 
            }
        }

        private CompressorState _compressorState;
        public CompressorState CompressorState
        {
            get
            {
                return this._compressorState;
            }
            set
            {
                this._compressorState = value;
                switch (this._compressorState)
                {
                    case CompressorState.Idle:
                        this.CompressIdled(this);
                        break;
                    case CompressorState.Working:
                        this.CompressStarted(this);
                        break;
                    case CompressorState.Stopped:
                        this.CompressStopped(this);
                        break;
                }
            }
        }

        public event Action<Object, String> SourcePathChanged = (o, e) => {};
        public event Action<Object, String> TargetPathChanged = (o, e) => {};
        public event Action<Object, Boolean> CanStartChanged = (o, e) => {};

        public event Action<Object> CompressStarted = (o) => {};
        public event Action<Object> CompressStopped = (o) => {};
        public event Action<Object> CompressIdled = (o) => {};

        public AppViewModel()
        {
            this.SourcePathChanged += (o, e) => this.UpdateCanStart();
            this.TargetPathChanged += (o, e) => this.UpdateCanStart();
            this.CompressStarted += (o) => this.UpdateCanStart();
            this.CompressStopped += (o) => this.UpdateCanStart();
            this.CompressIdled += (o) => this.UpdateCanStart();
        }

        private void UpdateCanStart()
        {
            this.CanStart = 
                !String.IsNullOrEmpty(this.SourcePath) &&
                !String.IsNullOrEmpty(this.TargetPath) &&
                this.CompressorState == CompressorState.Idle;
        }
    }

    public enum CompressorState
    {
        Idle,
        Working,
        Stopped,
    }
}


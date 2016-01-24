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

        public event Action<Object, String> SourcePathChanged = (o, e) => {};
        public event Action<Object, String> TargetPathChanged = (o, e) => {};
        public event Action<Object, Boolean> CanStartChanged = (o, e) => {};

        public AppViewModel()
        {
            this.SourcePathChanged += (o, e) =>
                {
                    this.CanStart = 
                        !String.IsNullOrEmpty(this.SourcePath) &&
                        !String.IsNullOrEmpty(this.TargetPath);
                };
            this.TargetPathChanged += (o, e) =>
                {
                    this.CanStart = 
                        !String.IsNullOrEmpty(this.SourcePath) &&
                        !String.IsNullOrEmpty(this.TargetPath);
                };
        }
    }
}


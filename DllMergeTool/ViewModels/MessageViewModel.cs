using Caliburn.Micro;
using DllMergeTool.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllMergeTool.ViewModels
{
    public class MessageViewModel : Screen, IModule
    {
        public MessageViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        private string content;
        private readonly IWindowManager windowManager;

        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }
        public bool OnSubmit()
        {
            this.TryClose(true);
            return true;
        }


        public string Title { get; set; }

    }
}

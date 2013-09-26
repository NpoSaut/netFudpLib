using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlokFramesCodegen
{
    public class DevFileInfo
    {
        
    }
    

    public enum PropertyKind : int { MajorVersion = 1, MinorVersion = 2 }

    public class CanProg : IDisposable
    {
       
        public void RefreshProperties()
        {
            throw new NotImplementedException();
        }

        public void SetProperty(PropertyKind property, int value)
        {
            throw new NotImplementedException();
        }

        


        public void Dispose()
        {
            // Эта функция вызовется тогда, когда нужно будет закрыть соединение.
            // Тут нужно будет сказать "досвидания"
            throw new NotImplementedException();
        }

      
    }
}
